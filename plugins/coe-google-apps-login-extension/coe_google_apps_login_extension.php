<?php
/**
 * @package coe_google_apps_login_extension
 * @version 0.1
 */
/*
Plugin Name: CoE Google Apps Login Extension
Plugin URI: TBD
Description: Extends the free version of Google Apps Login with custom logic. 
    Specifically ensures Google Apps is always used to sign in, and auto creates users if they don't exist yet.
Author: Dan Chenier
Notes: This depends on a slightly modified version of the google-apps-login plugin. 
    Specifically, the google_apps_login.php file needs to override the createUserOrError function and in that function call
    apply_filters( 'register_coe_user', null, $userinfo, $options );
    Alternatively, I considered that it would be possible to override the pluggable function 'get_user_by', because
    the google-apps-login calls that function after getting a Google verified email to see if the user exists.
    It seems that we could create the user at that time.  Unfortunately, in that method all we would have is the
    email, so we'd have to do a bit of work to call Google again somehow to get details on that user.
    By overriding createUserOrError we already have to user info so it's much easier that way. 
    Unfortunately we have to modify other people's code. It is specifically GPL licensed but maintenance will be awful.
    Recommendation for the future is to but the enterprise version of the google-apps-login plugin.
*/



// based on https://core.trac.wordpress.org/browser/tags/4.9/src/wp-includes/pluggable.php#L98
// if ( !function_exists('get_user_by') ) :

//     function get_user_by( $field, $value ) {
//         $userdata = WP_User::get_data_by( $field, $value );

//         if ( !$userdata ) {
//             if ($field === 'email' && isset($_GET['code'])) {
//                 // see if it ends with our domain
//                 $domain = "edmonton.ca";
//                 $length = strlen( $domain );
//                 if ( substr( $value, -$length) === $domain ) {
//                     // create the user here, hopefully someone is listening to this action (us)
//                     do_action( 'register_coe_user', $value );

//                     // try to get the user again
//                     $userdata = WP_User::get_data_by( $field, $value );
//                 }
//             }

//             if ( !$userdata )
//                 return false;
//         }

//         $user = new WP_User;
//         $user->init( $userdata );

//         return $user;
//     }
    
// endif;


/**
 * Singleton class for handling the coe-google-apps-login-extension plugin
 */
class coe_google_apps_login_extension {

	private static $instance = null;
	public static function get_instance() {
		if (null == self::$instance) {
			self::$instance = new self;
		}
		return self::$instance;
    }
    
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
        // plugin settings
		add_action( 'admin_init', array($this, 'ext_admin_init'), 6, 0 );
        add_action( is_multisite() ? 'network_admin_menu' : 'admin_menu', array($this, 'ext_admin_menu') );
        
        // ensure we always directly login with google instead of giving users the choice of google/forms auth
		add_filter('gal_options', array($this, 'override_ga_options'), 50, 1);        

        // filter for automatically creating users
        add_filter('register_coe_user', array($this, 'ext_register_coe_user'), 10, 3);

        // filter for choosing what page users go to after login in
        //add_filter( 'login_redirect', array($this, 'ext_login_redirect' ), 10, 3 );
    }

    /**
     * Hook into google-apps-login plugin to set default options, 
     * including automatically logging in with Google rather than 
     * giving userse a choice how to log in.
     * 
     * @param array $options Options used by the google-apps-login plugin 
     */
    public function override_ga_options($options) {
        // ensure we always use google to log in
        $options['ga_auto_login'] = true;
        $options['ga_poweredby'] = false;
        return $options;
    }

    /**
     * Hook to automatically create users.
     * 
     * @param object $user 
     * @param object $userInfo {
     *   User data returned by the Google OAuth service.
     * 
     *   @type string      $id                   The Google id of the user                        e.g. "115886881859296909934"
     *   @type string      $email                The email addrses of the user                    e.g. "dan@danlester.com"
     *   @type bool        $verified_email       Indicates users have verfied their email address e.g. true
     *   @type string      $name                 Display name                                     e.g. "Dan Lester"
     *   @type string      $given_name           First name                                       e.g. "Dan"
     *   @type string      $family_name          Last name                                        e.g. "Lester"
     *   @type string      $link                 Google+ page                                     e.g. "https://plus.google.com/115886881859296909934"
     *   @type string      $picture              The URL of the person's profile photo            e.g. "https://lh3.googleusercontent.com/-r4WThnaSX8o/AAAAAAAAAAI/AAAAAAAAABE/pEJQwH5wyqM/photo.jpg"
     *   @type string      $gender               male, female, or other                           e.g. "male"
     *   @type string      $locale               2 letter language code - 2 letter country code   e.g. "en-GB"
     *   @type string      $hd                   hosted domain                                    e.g. "danlester.com"
     * }
     * @return WP_User The newly created user.
     */
    public function ext_register_coe_user($user, $userInfo, $options) {

        // build "nicename", the username used in urls
        $nice_name = str_replace(' ', '-', strstr($userInfo->email, '@', true)); // Removes domain and replaces all spaces with hyphens.
        $nice_name = preg_replace('/[^A-Za-z0-9\-\.]/', '', $nice_name); // Removes special chars.

        // build array of data required by the built in wp_insert_user function
        $user_data = array(            
            'user_pass' => wp_generate_password(),
            'user_login' => $userInfo->email,
            'user_nicename' => $nice_name,
            'user_url' => $userInfo->link,
            'user_email' => $userInfo->email,
            'display_name' => $userInfo->name,
            'nickname' => $userInfo->given_name,
            'first_name' => $userInfo->given_name,
            'last_name' => $userInfo->family_name,
            'user_registered' => time(),
            'role' => 'subscriber'   // get_option('default_role') // Use default role or another role, e.g. 'editor'
        );
        
        // actually insert the user into the WordPress system here.
        // Note a random password will be generated, but we'll never use it.
        $user_id = wp_insert_user( $user_data );

        // return an actual user object because google-apps-login requires it 
        return  get_user_by('id', $user_id);
    }

    
    // /**
    //  * Redirect to url configured in settings after log in, or homepage if not specified.
    //  * Note can be overriden by a redirect_url query parameter
    //  * 
    //  * Removed because now we no longer allow anonymous access to the site, and we auto
    //  * login and provide a redirect_url when auto logging in.
    //  */
    // public function ext_login_redirect( $redirect_to, $original_redirect_to, $user ) {
    //     $options = get_option($this->get_options_name());
    //     $redirectUrl = $options["coe_gal_ext_redirect_url_default"];
    //     if (isset($redirectUrl)) {
    //         return $redirectUrl;
    //     } else {
    //         return get_site_url();
    //     }
    // }


 
 // <editor-fold defaultstate="collapsed" desc="Plugin Settings">
    /**
     * Provides a unique name for the WordPress system to identify this plugin settings menu
     */
    protected function get_options_menuname() {
        return 'coe_galogin_ext_list_options';
    }

    /**
     * Provides a unique name for the WordPress system to identify the plugin settings page
     */
    protected function get_options_pagename() {
        return 'coe_galogin_ext_options';
    }

    /**
     * Provides a unique name for the WordPress system to store options for this plugin in the database
     */
    protected function get_options_name() {
        return 'coe_galogin_ext';
    }

    /**
     * Hook to add plugin settings.
     */
    public function ext_admin_init() {
        register_setting( $this->get_options_pagename(), $this->get_options_name());
        add_settings_section('coe_gal_ext_plugin_main', 'CoE Google Apps Login Extension settings', array($this, 'ext_admin_settings_main'), $this->get_options_pagename());
        add_settings_field('coe_gal_ext_redirect_url_default', 'Default Redirect Url', array($this, 'ext_admin_setting_redirect_url_default'),  $this->get_options_pagename(), 'coe_gal_ext_plugin_main');
    }

    /**
     * Hook to display HTML content before the plugin settings.
     */
    public function ext_admin_settings_main() {
        //echo '<p>Main description of this section here.</p>';
    }

    /**
     * Hook to display textbox to allow users to enter url where users should go
     * if no redirect_url is specified on log in.
     */
    public function ext_admin_setting_redirect_url_default() {
        $options = get_option($this->get_options_name());
        if ( !array_key_exists("coe_gal_ext_redirect_url_default", $options)) {
            $options["coe_gal_ext_redirect_url_default"] = get_site_url();
        }
        echo "<input id='coe_gal_ext_redirect_url_default' name='".$this->get_options_name()."[coe_gal_ext_redirect_url_default]' size='40' type='text' value='{$options['coe_gal_ext_redirect_url_default']}' />";
    }

    /**
     * Hook to add settings menu.
     */
    public function ext_admin_menu() {
		if (is_multisite()) {
            add_submenu_page( 'settings.php', 
                __( 'CoE Google Apps Login Extension settings' , 'coe-google-apps-login-extension'), 
                __( 'CoE Google Apps Login Extension' , 'coe-google-apps-login-extension'),
				'manage_network_options', $this->get_options_menuname(),
				array($this, 'plugin_options'));
		}
		else {
            add_options_page( __( 'CoE Google Apps Login Extension settings' , 'coe-google-apps-login-extension'), 
                 __( 'CoE Google Apps Login Extension' , 'coe-google-apps-login-extension'),
				 'manage_options', $this->get_options_menuname(),
				 array($this, 'plugin_options'));
		}
    }
    
    /**
     * Renders options html.
     */
    public function plugin_options() {
		if (!current_user_can(is_multisite() ? 'manage_network_options' : 'manage_options')) {
			wp_die();
		}
		
		$submit_page = is_multisite() ? 'edit.php?action='.$this->get_options_menuname() : 'options.php';
		
		//if (is_multisite()) {
		//	$this->ga_options_do_network_errors();
		//}
		?>
		  
          <form action="<?php echo $submit_page; ?>" method="post" id="coe_gal_ext_form" enctype="multipart/form-data" >
            <div class="wrap">
              <?php settings_fields($this->get_options_pagename()); ?>
              <?php do_settings_sections($this->get_options_pagename()); ?>
		      <p class="submit">
                <?php submit_button(); ?>
		      </p>
            </div>
		</form>
        <?php
    }   
// </editor-fold>     

}

// Global accessor function to singleton
function coeGoogleAppsLogin() {
	return coe_google_apps_login_extension::get_instance();
}

// ensure called at least once
coeGoogleAppsLogin();

?>