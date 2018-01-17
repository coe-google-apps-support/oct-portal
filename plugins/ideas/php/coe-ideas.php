<?php
/*
Plugin Name: City of Edmonton Ideas
Description: City of Edmonton Ideas
Author: Dan Chenier
Author URI: https://github.com/dchenier
*/

// based on https://wpshout.com/writing-wordpress-reactjs-app-plugin/

/** Require the JWT library. */
use \Firebase\JWT\JWT;

class coe_ideas {
	private static $instance = null;
	public static function get_instance() {
		if (null == self::$instance) {
			self::$instance = new self;
		}
		return self::$instance;
    }

    private $mode = 'New'; // Can be 'New', 'ViewAll', 'ViewSingle'
    
    /**
     * Constructor; performs initialization
     */
    protected function __construct() {
        // do initialization here
        $this->add_actions();
    }

    /**
     * Add all the hooks into WordPress and google-apps-login functionality
     */
    protected function add_actions() {

        // Shortcode to output needed markup
        $this->add_shortcodes();

        add_action('init', array($this, 'enqueue_scripts'));

    }

    protected function add_shortcodes() {
        add_shortcode( 'coe_ideas',  array($this, 'show_new' ));

    }

    function show_new() {
        return '<div id="coe-idea-new"></div>';
    }

    public function enqueue_scripts() {
        // Only load for the specific posts
        // It testing I can't seem to use any of the regular methods like 
        // is_page or is_single or even $GLOBALS['wp_the_query'].
        // I suspect Divi is doing something to the pages but I don't 
        // have time to investigate, so we'll drop done to good old Request
        // URL inspection with regular expressions
        // if( !is_page( 'new-idea' ) ) {
        // 	return;
        // }
        if ( preg_match('/\/new-idea(\/|#|\?|\z)/i', $_SERVER['REQUEST_URI'])) {
            $mode = 'New';
        } elseif ( preg_match('/\/view-ideas(\/|#|\?|\z)/i', $_SERVER['REQUEST_URI']) ){
            $mode = 'ViewAll';
        } else {
            return;
        }

        // CoeIdeasWebpackBuildFiles.php is dynamically built by webpack
        require_once( 'CoeIdeasWebpackBuiltFiles.php' );

    
        // order needs to be like this 'manifest, vendor, app, others'
        $scripts = CoeIdeasWebpackBuiltFiles::$jsFiles;
        usort($scripts, array($this, 'get_compare_script_order' ));
        $lastScriptHandle = null;
        foreach ($scripts as $jsFile) {
            wp_enqueue_script( $jsFile, plugin_dir_url( __FILE__ ) . $jsFile, null, null, true );
            $lastScriptHandle = $jsFile;
        }
    
        // Google fonts
        $protocol  = is_ssl() ? 'https' : 'http';
        wp_enqueue_style( "coe-gf-roboto", "$protocol://fonts.googleapis.com/css?family=Roboto:400,500,700,400italic|Material+Icons" );
        
    
        foreach ( CoeIdeasWebpackBuiltFiles::$cssFiles as $cssFile) {
            wp_enqueue_style( $cssFile, plugin_dir_url( __FILE__ ) . $cssFile );
        }

        // last, let's enqueue the code to add the auth key
        wp_add_inline_script($lastScriptHandle, $this->get_auth_key_script(), 'after');
    }

    protected function get_compare_script_order($a, $b) {
        // order needs to be like this 'manifest, vendor, app, others'
        return $this->get_compare_script_order_get_arg_weight($a) 
                > $this->get_compare_script_order_get_arg_weight($b);
    }

    
    protected function get_compare_script_order_get_arg_weight($a) {
        // order needs to be like this 'manifest, vendor, app, others'
        if (strpos($a, 'manifest') !== false)
            return 1;
        else if (strpos($a, 'vendor') !== false)
            return 2;
        else if (strpos($a, 'app') !== false)
            return 3;
        else 
            return 4;
    }

    protected function get_auth_key_script() {
        $authKeyInfo = $this->get_auth_key();
        $authKey = $authKeyInfo[0];
        $newCookieExpires = $authKeyInfo[1]; // either false or a date when it expires

        $str = 'var ideaApps = (window.coe || {}).ideas;
if (ideaApps) {
  for (var key in ideaApps) {
    if (ideaApps.hasOwnProperty(key) && typeof ideaApps[key].initialize === "function") {
      ideaApps[key].initialize({          
        ';

        $ideaApi = defined('COE_IDEA_API') ? COE_IDEA_API : false;
        if ($ideaApi) {
            $str = $str . 'ideaApi: "' . $ideaApi . '",
        ';
        }

        if ( preg_match('/\/new-idea(\/|#|\?|\z)/i', $_SERVER['REQUEST_URI'])) {
            $mode = 'New';
        } elseif ( preg_match('/\/view-ideas(\/|#|\?|\z)/i', $_SERVER['REQUEST_URI']) ){
            $mode = 'ViewAll';
        }        

        if ($mode == 'ViewAll') {
            $str = $str . 'route: "ViewIdeas",
        ';
        }

        $str = $str . '
        userInfo: {
          "auth": "' . $authKey . '"
        }
      });
    }
  }
}';

        if ($newCookieExpires ) {
            $str = $str . '
document.cookie = "coeauth=' . $authKey . '; expires=' . date('D M d Y H:i:s O', $newCookieExpires) . ';path=' . COOKIEPATH . '";';
        }
    
        return $str;
    }

    protected function get_auth_key() {
        $authToken = isset($_COOKIE['coeauth']) ? $_COOKIE['coeauth'] : false;
        $newCookieExpires = false;

        if ($authToken) {
            // somebody is turning + into spaces. Let's fix that!
            $authToken = str_replace(' ', '+', $authToken);
        } else {
            // based on class-jwt-auth-public::generate_token
            $secret_key = defined('JWT_AUTH_SECRET_KEY') ? JWT_AUTH_SECRET_KEY : false;
            $user = wp_get_current_user();
            if ($secret_key && $user) {
                $encryptKey = defined('COE_SECRET_AUTH_KEY') ? COE_SECRET_AUTH_KEY : false;
                $encryptIv = defined('COE_SECRET_AUTH_IV') ? COE_SECRET_AUTH_IV : false;
                if (!$encryptKey || !$encryptIv) {
                    return new WP_Error(
                        'jwt_auth_bad_config',
                        __('JWT is not configurated properly, please contact the admin', 'wp-api-jwt-auth'),
                        array(
                            'status' => 403,
                        )
                    );
                }

                $issuedAt = time();
                $notBefore = apply_filters('jwt_auth_not_before', $issuedAt, $issuedAt);
                $expire = apply_filters('jwt_auth_expire', $issuedAt + (DAY_IN_SECONDS * 7), $issuedAt);

                $token = array(
                    'iss' => get_bloginfo('url'),
                    'iat' => $issuedAt,
                    'nbf' => $notBefore,
                    'exp' => $expire,
                    'data' => array(
                        'user' => array(
                            'id' => $user->data->ID,
                        ),
                    ),
                );

                /** Let the user modify the token data before the sign. */
                $token = JWT::encode(apply_filters('jwt_auth_token_before_sign', $token, $user), $secret_key);

                /** The token is signed, now create the object with no sensible user data to the client*/
                $data = array(
                    'token' => $token,
                    'user_email' => $user->data->user_email,
                    'user_nicename' => $user->data->user_nicename,
                    'user_display_name' => $user->data->display_name
                );

                /** Let the user modify the data before encoding */
                $finalTokenArray = apply_filters('jwt_auth_token_before_dispatch', $data, $user);

                // Must be exact 32 chars (256 bit)
                $encryptKey = substr(hash('sha256', $encryptKey, true), 0, 32);
                $encryptIv  = substr(hash('sha256', $encryptIv, true), 0, 16);
                $authToken = base64_encode(openssl_encrypt($finalTokenArray["token"], 'AES-256-CBC', $encryptKey, OPENSSL_RAW_DATA, $encryptIv));

                $newCookieExpires = $expire;
            }
        }

        return array($authToken, $newCookieExpires);
    }
}

// Global accessor function to singleton
function coeIdeasPlugin() {
	return coe_ideas::get_instance();
}

// ensure called at least once
coeIdeasPlugin();
