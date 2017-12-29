<?php

/**
 * Plugin component common to all versions of Google Apps Login
 */

class core_google_apps_login {
	
	protected function __construct() {
		$this->add_actions();
		register_activation_hook($this->my_plugin_basename(), array( $this, 'ga_activation_hook' ) );
	}

	protected static $gal_cookie_name = 'wordpress_google_apps_login';
	
	// May be overridden in basic or premium
	public function ga_activation_hook($network_wide) {
		global $gal_core_already_exists;
		if ($gal_core_already_exists) {
			deactivate_plugins( $this->my_plugin_basename() );
			echo( 'Please Deactivate the free version of Google Apps Login before you activate the new Premium/Enterprise version.' );
			exit;
		}
	}
	
	public function ga_plugins_loaded() {
		load_plugin_textdomain( 'google-apps-login', false, dirname($this->my_plugin_basename()).'/lang/' );
	}
	
	protected $newcookievalue = null;
	protected function get_cookie_value() {
		if (!$this->newcookievalue) {
			if (isset($_COOKIE[self::$gal_cookie_name])) {
				$this->newcookievalue = $_COOKIE[self::$gal_cookie_name];
			}
			else {
				$this->newcookievalue = md5(rand());
			}
		}
		return $this->newcookievalue;
	}
	
	private $doneIncludePath = false;
	private function setIncludePath() {
		if (!$this->doneIncludePath) {
			set_include_path(plugin_dir_path(__FILE__) . PATH_SEPARATOR . get_include_path());
			$this->doneIncludePath = true;
		}
	}
	
	protected function createGoogleClient($options, $includeoauth=false) {
		// Another plugin might have already included these files
		// Unfortunately we just have to hope they have a similar enough version
		
		$this->setIncludePath();
		
		// Google PHP Client obtained from https://github.com/google/google-api-php-client
		// Using modified Google Client to avoid name clashes - rename process:
		// On OSX requires export LC_CTYPE=C and export LANG=C in your ~/.profile
		// find . -type f -exec sed -i '' -e 's/Google_/GoogleGAL_/g' {} +
		// We also updated Google/Auth/AssertionCredentials.php to be able to accept the PEM class
		// We wrote PEM class here: Google/Signer/PEM.php
		// Also wrote our own autoload.php in /core
		
		$client = $this->get_Google_Client();
				
		$client->setClientId($options['ga_clientid']);
		$client->setClientSecret($options['ga_clientsecret']);
		$client->setRedirectUri($this->get_login_url());

		$hd = $this->get_hd();
		if ($hd) {
			$client->setHostedDomain($hd);
		}
		
		$scopes = array_unique(apply_filters('gal_gather_scopes', $this->get_default_scopes()));
		$client->setScopes($scopes);
		$client->setApprovalPrompt($options['ga_force_permissions'] ? 'force' : 'auto');
		
		$oauthservice = null;
		if ($includeoauth) {
			/*if (!class_exists('GoogleGAL_Service_Oauth2')) {
				require_once( 'Google/Service/Oauth2.php' );
			}*/
			$oauthservice = new GoogleGAL_Service_Oauth2($client);
		}
				
		return Array($client, $oauthservice);
	}

	protected function get_hd() {
		return '';
	}
	
	protected function get_default_scopes() {
		return Array('openid', 'email', 'https://www.googleapis.com/auth/userinfo.profile');
	}
	
	public function ga_login_styles() {
		$options = $this->get_option_galogin();
		wp_enqueue_script('jquery');
		 ?>
		<style type="text/css">
			form#loginform p.galogin {
				background: none repeat scroll 0 0 #2EA2CC;
				border-color: #0074A2;
				box-shadow: 0 1px 0 rgba(120, 200, 230, 0.5) inset, 0 1px 0 rgba(0, 0, 0, 0.15);
				color: #FFFFFF;
				text-decoration: none;
				text-align: center;
				vertical-align: middle;
				border-radius: 3px;
				padding: 4px;
				height: 27px;
				font-size: 14px;
				margin-bottom: <?php echo $options['ga_poweredby'] ? '6' : '16' ?>px;
			}
			
			form#loginform p.galogin a {
				color: #FFFFFF;
				line-height: 27px;
				font-weight: bold;
			}

			form#loginform p.galogin a:hover {
				color: #CCCCCC;
			}
			
			h3.galogin-or {
				text-align: center;
				margin-top: 16px;
				margin-bottom: 16px;
			}
			
			p.galogin-powered {
				font-size: 0.7em;
				font-style: italic;
				text-align: right;
			}
			
			p.galogin-logout {
			  	background-color: #FFFFFF;
				border: 4px solid #CCCCCC;
				box-shadow: 0 1px 1px 0 rgba(0, 0, 0, 0.1);
				padding: 12px;
				margin: 12px 0;
			}
			
			<?php if ($this->should_hidewplogin($options)) { ?>
			
			div#login form#loginform p label[for=user_login], 
			div#login form#loginform p label[for=user_pass],
			div#login form#loginform p label[for=rememberme],
			div#login form#loginform p.submit,
			div#login p#nav {
				display: none;
			} 
			 
			<?php } ?>
			
		 </style>
	<?php }
	
	// public in case widgets want to use it
	public function ga_start_auth_get_url() {
		$options = $this->get_option_galogin();
		$clients = $this->createGoogleClient($options);
		/** @var GoogleGAL_Client $client */
		$client = $clients[0];
		
		// Generate a CSRF token
		$client->setState(urlencode(
				$this->session_indep_create_nonce('google_apps_login-'.$this->get_cookie_value())
				.'|'.$this->get_redirect_url()
		));
		
		$authUrl = $client->createAuthUrl();
		if ($options['ga_clientid'] == '' || $options['ga_clientsecret'] == '') {
			$authUrl = "?error=ga_needs_configuring";
		}
		return $authUrl;
	}
	
	public function ga_login_form() {
		$options = $this->get_option_galogin();
		
		$authUrl = $this->ga_start_auth_get_url();
		
		$do_autologin = false;
		
		if (isset($_GET['gaautologin'])) { // This GET param can always override the option set in admin panel
			$do_autologin = $_GET['gaautologin'] == 'true';
		}
		elseif ($options['ga_auto_login']) {
			// Respect the option unless GET params mean we should remain on login page (e.g. ?loggedout=true)
			if (count($_GET) == (isset($_GET['redirect_to']) ? 1 : 0) 
									+ (isset($_GET['reauth']) ? 1 : 0) 
									+ (isset($_GET['action']) && $_GET['action']=='login' ? 1 : 0)) {
				$do_autologin = true;
			}
			if (isset($_POST['log']) && isset($_POST['pwd'])) { // This was a WP username/password login attempt
				$do_autologin = false;
			}
		}
		
		if ($do_autologin && $options['ga_clientid'] != '' && $options['ga_clientsecret'] != '') {
			if (!headers_sent()) {
				wp_redirect($authUrl);
				exit;
			}
			else { ?>
				<p><b><?php printf( __( 'Redirecting to <a href="%s">Login via Google</a>...' , 'google-apps-login'), $authUrl ); ?></b></p>
				<script type="text/javascript">
				window.location = "<?php echo $authUrl; ?>";
				</script>
			<?php 
			}
		}
		
?>
		<p class="galogin"> 
			<a href="<?php echo $authUrl; ?>"><?php echo esc_html($this->get_login_button_text()); ?></a>
		</p>
		
		<?php if ($options['ga_poweredby']) { ?>
		<p class='galogin-powered'><?php esc_html_e( 'Powered by ' , 'google-apps-login'); ?><a href='http://wp-glogin.com/?utm_source=Login%20Form&utm_medium=freemium&utm_campaign=LoginForm' target="_blank">wp-glogin.com</a></p>
		<?php } ?>
		
		<script>
		jQuery(document).ready(function(){
			<?php ob_start(); /* Buffer javascript contents so we can run it through a filter */ ?>
			
			var loginform = jQuery('#loginform,#front-login-form');
			var googlelink = jQuery('p.galogin');
			var poweredby = jQuery('p.galogin-powered');

			<?php if ($this->should_hidewplogin($options)) { ?>
				loginform.empty();
			<?php 
			} else {
			?>
				loginform.prepend("<h3 class='galogin-or'><?php esc_html_e( 'or' , 'google-apps-login'); ?></h3>");
			<?php } ?>
			
			if (poweredby) {
				loginform.prepend(poweredby);
			}
			loginform.prepend(googlelink);

			<?php 
				$fntxt = ob_get_clean(); 
				echo apply_filters('gal_login_form_readyjs', $fntxt);
			?>
		});
		</script>
<?php 	
	}
	
	protected function get_login_button_text() {
		$login_button_text = __('Login with Google', 'google-apps-login');
		return apply_filters('gal_login_button_text', $login_button_text);
	}
	
	protected function should_hidewplogin($options) {
		return false;
	}
	
	protected function get_redirect_url() {
		$options = $this->get_option_galogin();
		
		if (array_key_exists('redirect_to', $_REQUEST) && $_REQUEST['redirect_to']) {
			return $_REQUEST['redirect_to'];
		} elseif (is_multisite() && !$options['ga_ms_usesubsitecallback']) {
			return admin_url(); // This is what WordPress would choose as default
								// but we have to specify explicitly since all callbacks go via root site
		}
		return '';
	}
	
	public function ga_authenticate($user, $username=null, $password=null) {
		if (isset($_REQUEST['error'])) {
			switch ($_REQUEST['error']) {
				case 'access_denied':
					$error_message = __( 'You did not grant access' , 'google-apps-login');
				break;
				case 'ga_needs_configuring':
					$error_message = __( 'The admin needs to configure Google Apps Login plugin - please follow '
										.'<a href="http://wp-glogin.com/installing-google-apps-login/#main-settings"'
										.' target="_blank">instructions here</a>' , 'google-apps-login');
				break;
				case 'ga_user_must_glogin':
					$error_message = sprintf(__( 'The user must use <i>%s</i> to access the site' , 'google-apps-login'),
					                        htmlentities($this->get_login_button_text()));
				break;
				default:
					$error_message = __( 'Unrecognized error message' , 'google-apps-login');
				break;
			}
			$user = new WP_Error('ga_login_error', $error_message);
			return $this->displayAndReturnError($user);
		}
		
		$options = $this->get_option_galogin();
		
		if (isset($_GET['code'])) {
			if (!isset($_REQUEST['state'])) {
				$user = new WP_Error('ga_login_error', __( "Session mismatch - try again, but there could be a problem setting state" , 'google-apps-login') );
				return $this->displayAndReturnError($user);
			}
			
			$statevars = explode('|', urldecode($_REQUEST['state']));
			if (count($statevars) != 2) {
				$user = new WP_Error('ga_login_error', __( "Session mismatch - try again, but there could be a problem passing state" , 'google-apps-login') );
				return $this->displayAndReturnError($user);
			}
			$retnonce = $statevars[0];
			$retredirectto = $statevars[1];
			
			if (!$this->session_indep_verify_nonce($retnonce, 'google_apps_login-'.$this->get_cookie_value())) {
				$user = new WP_Error('ga_login_error', __( "Session mismatch - try again, but there could be a problem setting cookies" , 'google-apps-login') );
				return $this->displayAndReturnError($user);
			}

			try {
				$clients = $this->createGoogleClient($options, true);
				/** @var GoogleGAL_Client $client */
				$client = $clients[0];
				$oauthservice = $clients[1];
				
				$client->authenticate($_GET['code']);
							
				/*  userinfo example:
				 "id": "115886881859296909934",
				 "email": "dan@danlester.com",
				 "verified_email": true,
				 "name": "Dan Lester",
				 "given_name": "Dan",
				 "family_name": "Lester",
				 "link": "https://plus.google.com/115886881859296909934",
				 "picture": "https://lh3.googleusercontent.com/-r4WThnaSX8o/AAAAAAAAAAI/AAAAAAAAABE/pEJQwH5wyqM/photo.jpg",
				 "gender": "male",
				 "locale": "en-GB",
				 "hd": "danlester.com"
				 */
				$userinfo = $oauthservice->userinfo->get();
				
				if ($userinfo && is_object($userinfo) && property_exists($userinfo, 'email') 
						&& property_exists($userinfo, 'verifiedEmail')) {
					
					$google_email = $userinfo->email;
					$google_verified_email = $userinfo->verifiedEmail;
					
					if (!$google_verified_email) {
						$user = new WP_Error('ga_login_error', __( 'Email needs to be verified on your Google Account' , 'google-apps-login') );
					}
					else {
						$user = get_user_by('email', $google_email);
						
						$userdidnotexist = false;
						if (!$user) {
							$userdidnotexist = true;
							$user = $this->createUserOrError($userinfo, $options);
						}
						
						if ($user && !is_wp_error($user)) {
							// In some versions, check group membership
							$this->check_groups($client, $userinfo, $user, $userdidnotexist);

							// Set redirect for wp-login to receive via our own login_redirect callback
							$this->setFinalRedirect($retredirectto);
							
							// Call hook in case another plugin wants to use the user's data
							do_action('gal_user_loggedin', $user, $userinfo, $userdidnotexist, $client, $oauthservice);
						}
					}
				}
				else {
					$user = new WP_Error('ga_login_error', __( "User authenticated OK, but error fetching user details from Google" , 'google-apps-login') );
				}
			} catch (GoogleGAL_Exception $e) {
				$user = new WP_Error('ga_login_error', $e->getMessage());
			}
		}
		else {
			$user = $this->checkRegularWPLogin($user, $username, $password, $options);
		}
		
		if (is_wp_error($user)) {
			$this->displayAndReturnError($user);
		}
		
		return $user;
	}
	
	protected function createUserOrError($userinfo, $options) {
		return( new WP_Error('ga_login_error', sprintf( __( 'User %s not registered in Wordpress' , 'google-apps-login'),
												 $userinfo->email) ) );
	}

	protected function checkRegularWPLogin($user, $username, $password, $options) {
		return $user;
	}
	
	// Has content in Enterprise
	protected function check_groups($client, $userinfo, $user, $userdidnotexist) {
	}
		
	protected function displayAndReturnError($user) {
		if (is_wp_error($user) && get_bloginfo('version') < 3.7) {
			// Only newer wordpress versions display errors from $user for us
			global $error;
			/** @var WP_Error $user */
			$error = htmlentities2($user->get_error_message());
		}
		return $user;
	}
	
	protected $_final_redirect = '';
	
	protected function setFinalRedirect($redirect_to) {
		$this->_final_redirect = $redirect_to;
	}

	protected function getFinalRedirect() {
		return $this->_final_redirect;
	}
	
	public function ga_login_redirect($redirect_to, $request_from='', $user=null) {
		if ($user && !is_wp_error($user)) {
			$final_redirect = $this->getFinalRedirect();
			if ($final_redirect !== '') {
				$option = $this->get_option_galogin();
				// Whitelist the subdomain if all auth is going through the top level domain's wp-login.php
				if (is_multisite() && !$option['ga_ms_usesubsitecallback']) {
					$this->add_allowed_redirect_host($final_redirect);
					add_filter('allowed_redirect_hosts', array($this,'gal_allowed_redirect_hosts'), 10);
				}
				return $final_redirect;
			}
		}
		return $redirect_to;
	}
	
	public function ga_init() {
		if (isset($_GET['code']) && isset($_GET['state']) && $_SERVER['REQUEST_METHOD']=='GET') {
			$options = $this->get_option_galogin();
			if ($options['ga_rememberme']) {
				$_POST['rememberme'] = true;
			}
		}
		if (!isset($_COOKIE[self::$gal_cookie_name]) && apply_filters('gal_set_login_cookie', true)) {
			$secure = ( 'https' === parse_url( $this->get_login_url(), PHP_URL_SCHEME ) );
			setcookie(self::$gal_cookie_name, $this->get_cookie_value(), 0, '/', defined('COOKIE_DOMAIN') ? COOKIE_DOMAIN : '', $secure );
		}
	}
	
	protected function get_login_url() {
		$options = $this->get_option_galogin();
		$login_url = wp_login_url();

		if (is_multisite() && !$options['ga_ms_usesubsitecallback']) {
			$login_url = network_site_url('wp-login.php');
		} 

		if (force_ssl_admin() && strtolower(substr($login_url,0,7)) == 'http://') {
			$login_url = 'https://'.substr($login_url,7);
		}

		return apply_filters( 'gal_login_url', $login_url );
	}

	protected $allowed_redirect_hosts = array();
	// In multisite, add subdomains to allowed_redirect_hosts so redirect_to can work for them
	public function gal_allowed_redirect_hosts($hosts) {
		return array_merge($hosts, $this->allowed_redirect_hosts);
	}

	protected function add_allowed_redirect_host($location) {
		if (!is_multisite()) {
			return;
		}

		if (!defined('SUBDOMAIN_INSTALL') || !SUBDOMAIN_INSTALL) {
			return;
		}

		$location = trim( strtolower($location) );
		// browsers will assume 'http' is your protocol, and will obey a redirect to a URL starting with '//'
		if ( substr($location, 0, 2) == '//' )
			$location = 'http:' . $location;

		// In php 5 parse_url may fail if the URL query part contains http://, bug #38143
		$test = ( $cut = strpos($location, '?') ) ? substr( $location, 0, $cut ) : $location;

		// @-operator is used to prevent possible warnings in PHP < 5.3.3.
		$lp = @parse_url($test);

		// Give up if malformed URL
		if ( false === $lp )
			return;

		$sites = get_sites(
			array('domain' => $lp['host'])
		);

		if (count($sites) > 0) {
			$this->allowed_redirect_hosts[] = $lp['host'];
		}
	}
	
	// Build our own nonce functions as wp_create_nonce is user dependent,
	// and our nonce is created when logged-out, then verified when logged-in
	
	protected function session_indep_create_nonce($action = -1) {
		$i = wp_nonce_tick();
		return substr( wp_hash( $i . '|' . $action, 'nonce' ), -12, 10 );
	}
	
	protected function session_indep_verify_nonce( $nonce, $action = -1 ) {
		$nonce = (string) $nonce;
		if ( empty( $nonce ) ) {
			return false;
		}
	
		$i = wp_nonce_tick();
	
		// Nonce generated 0-12 hours ago
		$expected = substr( wp_hash( $i . '|' . $action, 'nonce'), -12, 10 );
		if ( $this->hash_equals( $expected, $nonce ) ) {
			return 1;
		}
	
		// Nonce generated 12-24 hours ago
		$expected = substr( wp_hash( ( $i - 1 ) . '|' . $action, 'nonce' ), -12, 10 );
		if ( $this->hash_equals( $expected, $nonce ) ) {
			return 2;
		}
	
		// Invalid nonce
		return false;
	}
	
	private function hash_equals($expected, $nonce) {
		// Global/PHP fn hash_equals didn't exist before WP3.9.2
		if (function_exists('hash_equals')) {
			return hash_equals($expected, $nonce);
		}
		return $expected == $nonce;
	}
	
	// ADMIN AND OPTIONS
	// *****************

	protected function get_options_menuname() {
		return 'galogin_list_options';
	}
	
	protected function get_options_pagename() {
		return 'galogin_options';
	}
	
	protected function get_settings_url() {
		return is_multisite()
			? network_admin_url( 'settings.php?page='.$this->get_options_menuname() )
			: admin_url( 'options-general.php?page='.$this->get_options_menuname() );
	}
	
	public function ga_admin_auth_message() {
		echo '<div class="error"><p>';
		echo sprintf( __('You will need to complete Google Apps Login <a href="%s">Settings</a> in order for the plugin to work', 'google-apps-login'), 
					esc_url($this->get_settings_url()) ); 
		echo '</p></div>';
	}
	
	public function ga_admin_init() {
		register_setting( $this->get_options_pagename(), $this->get_options_name(), Array($this, 'ga_options_validate') );
				
		// Admin notice that configuration is required
		$options = $this->get_option_galogin();
		
		if (current_user_can( is_multisite() ? 'manage_network_options' : 'manage_options' ) 
				&& ($options['ga_clientid'] == '' || $options['ga_clientsecret'] == '')) {

			if (!array_key_exists('page', $_REQUEST) || $_REQUEST['page'] != $this->get_options_menuname()) {
				add_action('admin_notices', Array($this, 'ga_admin_auth_message'));
				if (is_multisite()) {
					add_action('network_admin_notices', Array($this, 'ga_admin_auth_message'));
				}
			}
		}
		else {
			$this->set_other_admin_notices();
		}
		
		add_action('user_profile_picture_description', Array($this, 'gal_user_profile_picture_description'));
	}
	
	public function gal_user_profile_picture_description($description) {
		if ($description != '') {

			// Display avatar in profile
			$purchase_url = 'http://wp-glogin.com/avatars/?utm_source=Profile%20Page&utm_medium=freemium&utm_campaign=Avatars';
			$source_text = '<b>Install <a href="'.$purchase_url.'">Google Profile Avatars</a> to use your Google account\'s profile photo here automatically.</b>';

			$wp_user = wp_get_current_user();
			$description = apply_filters('gal_avatar_source_desc', $description.' <br /> '.$source_text, $wp_user);
		}

		return $description;
	}
	
	// Has content in Basic
	protected function set_other_admin_notices() {
	}
	
	public function ga_admin_menu() {
		if (is_multisite()) {
			add_submenu_page( 'settings.php', __( 'Google Apps Login settings' , 'google-apps-login'), __( 'Google Apps Login' , 'google-apps-login'),
				'manage_network_options', $this->get_options_menuname(),
				array($this, 'ga_options_do_page'));
		}
		else {
			add_options_page( __( 'Google Apps Login settings' , 'google-apps-login'), __( 'Google Apps Login' , 'google-apps-login'),
				 'manage_options', $this->get_options_menuname(),
				 array($this, 'ga_options_do_page'));
		}
	}
	
	public function ga_options_do_page() {
		if (!current_user_can(is_multisite() ? 'manage_network_options' : 'manage_options')) {
			wp_die();
		}
		
		wp_enqueue_script( 'gal_admin_js', $this->my_plugin_url().'js/gal-admin.js', array('jquery') );
		wp_enqueue_style( 'gal_admin_css', $this->my_plugin_url().'css/gal-admin.css' );

		$submit_page = is_multisite() ? 'edit.php?action='.$this->get_options_menuname() : 'options.php';
		
		if (is_multisite()) {
			$this->ga_options_do_network_errors();
		}
		?>
		  
		<div>
		
		<h2><?php _e('Google Apps Login setup', 'google-apps-login'); ?></h2>
		
		<div id="gal-tablewrapper">
		
		<div id="gal-tableleft" class="gal-tablecell">
		
		<p><?php _e( 'To set up your website to enable Google logins, you will need to follow instructions specific to your website.', 'google-apps-login'); ?></p>
		
		<p><a href="<?php echo $this->calculate_instructions_url(); ?>#config" id="gal-personalinstrlink" class="button-secondary" target="gainstr"><?php 
		_e( 'Click here to open your personalized instructions in a new window' , 'google-apps-login'); ?></a></p>
		
	
		<?php 
		$this->ga_section_text_end();
		?>
		
		<h2 id="gal-tabs" class="nav-tab-wrapper">
			<a href="#main" id="main-tab" class="nav-tab nav-tab-active">Main Setup</a>
			<a href="#domain" id="domain-tab" class="nav-tab">Domain Control</a>
			<a href="#advanced" id="advanced-tab" class="nav-tab">Advanced Options</a>
			<?php $this->draw_more_tabs(); ?>
		</h2>
				
		
		<form action="<?php echo $submit_page; ?>" method="post" id="gal_form" enctype="multipart/form-data" >
		
		<?php 
		settings_fields($this->get_options_pagename());
		$this->ga_mainsection_text();
		$this->ga_domainsection_text();
		$this->ga_advancedsection_text();
		$this->ga_moresection_text();
		?>
		
		<p class="submit">
			<input type="submit" value="<?php esc_attr_e( 'Save Changes' , 'google-apps-login'); ?>" class="button button-primary" id="submit" name="submit">
		</p>
		</form>
		</div>

		<?php $this->ga_options_do_sidebar(); ?>

		</div>
		
		</div>  <?php
	}
	
	// Extended in premium
	protected function draw_more_tabs() {
	}
	
	// Extended in premium
	protected function ga_moresection_text() {
	}
	
	// Has content in Basic
	protected function ga_options_do_sidebar() {
	}
	
	protected function ga_options_do_network_errors() {
		if (isset($_REQUEST['updated']) && $_REQUEST['updated']) {
			?>
				<div id="setting-error-settings_updated" class="updated settings-error">
				<p>
				<strong><?php _e( 'Settings saved.', 'google-apps-login'); ?></strong>
				</p>
				</div>
			<?php
		}

		if (isset($_REQUEST['error_setting']) && is_array($_REQUEST['error_setting'])
			&& isset($_REQUEST['error_code']) && is_array($_REQUEST['error_code'])) {
			$error_code = $_REQUEST['error_code'];
			$error_setting = $_REQUEST['error_setting'];
			if (count($error_code) > 0 && count($error_code) == count($error_setting)) {
				for ($i=0; $i<count($error_code) ; ++$i) {
					?>
				<div id="setting-error-settings_<?php echo $i; ?>" class="error settings-error">
				<p>
				<strong><?php echo htmlentities2($this->get_error_string($error_setting[$i].'|'.$error_code[$i])); ?></strong>
				</p>
				</div>
					<?php
				}
			}
		}
	}
	
	protected function ga_mainsection_text() {
		echo '<div id="main-section" class="galtab active">';
		echo '<p>';
		echo sprintf( __( "The <a href='%s'>instructions</a> above will guide you to Google's Cloud Console where you will enter two URLs, and also obtain two codes (Client ID and Client Secret) which you will need to enter in the boxes below.",
			 'google-apps-login'), $this->calculate_instructions_url()."#config" );
		echo '</p>';
		
		$options = $this->get_option_galogin(); // Must be in this order to invoke upgrade code
		$saoptions = $this->get_sa_option();
		
		echo '<label for="input_ga_clientid" class="textinput big">'.__('Client ID', 'google-apps-login').'</label>';
		echo "<input id='input_ga_clientid' class='textinput' name='".$this->get_options_name()."[ga_clientid]' size='68' type='text' value='".esc_attr($options['ga_clientid'])."' />";
		echo '<br class="clear"/><p class="desc big">';
		printf( __('Normally something like %s', 'google-apps-login'), '1234567890123-w1dwn5pfgjeo96c73821dfbof6n4kdhw.apps.googleusercontent.com' );
		echo '</p>';
		
		echo '<label for="input_ga_clientsecret" class="textinput big">'.__('Client Secret', 'google-apps-login').'</label>';
		echo "<input id='input_ga_clientsecret' class='textinput' name='".$this->get_options_name()."[ga_clientsecret]' size='40' type='text' value='".esc_attr($options['ga_clientsecret'])."' />";
		echo '<br class="clear" /><p class="desc big">';
		printf( __('Normally something like %s', 'google-apps-login'), 'sHSfR4_jf_2jsy-kjPjgf2dT' );
		echo '</p>';
		
		$serviceacct_plugins = apply_filters('gal_gather_serviceacct_reqs', array());
		
		echo '<h3>Service Account settings</h3>';
		
		if (count($serviceacct_plugins) == 0) {
			?>			
			<p>Some Google Apps extensions may require you to set up a Service Account. If you Activate those extension plugins then
			come back to this page, you will see further instructions, including the 'permission scopes' those extensions require. 
			However, if you know you need to set up a Service Account in advance, you can click below to reveal the settings.</p>
			
			<p><a href="#" id="gal-show-admin-serviceacct">Show Service Account settings</a></p>
			
			<span id="gal-hide-admin-serviceacct" style="display: none;">
			
		<?php } ?>
		
		<p>In order for all users to have permissions to access domain-level information from Google, you will need to create
		a Service Account. Please see our 
		<a href="https://wp-glogin.com/installing-google-apps-login/service-account-setup/?utm_source=ServiceAccount&utm_medium=freemium&utm_campaign=Login" target="_blank">extended instructions here</a>.</p>
		
		<?php 
		if (count($serviceacct_plugins) > 0) {
			$this->ga_show_service_account_reqs($serviceacct_plugins);
		}
		
		echo '<br class="clear">';
		if ($saoptions['ga_serviceemail'] != '') {
			if ($saoptions['ga_serviceid'] != '') {
				// Display client id
				echo '<label for="input_ga_serviceid" class="textinput">'.__('Service Account Client ID / Name', 'google-apps-login').'</label>';
				echo "<div class='gal-lowerinput'>";
				//echo "<span id='input_ga_serviceid'>".htmlentities($saoptions['ga_serviceid'])."</span>";
				echo "<div id='input_ga_serviceid' class='gal-admin-scopes-list'>".htmlentities($saoptions['ga_serviceid'])."</div>";
				echo '</div>';
				echo '<br class="clear">';
			}
			// Display service email
			echo '<label for="input_ga_serviceemail" class="textinput">'.__('Service Account email address', 'google-apps-login').'</label>';
			echo "<div class='gal-lowerinput'>";
			echo "<span id='input_ga_serviceemail'>".htmlentities($saoptions['ga_serviceemail'])."</span>";
			echo '</div>';
			echo '<br class="clear">';
			if ($saoptions['ga_pkey_print'] != '') {
				// Display finger print of key
				echo '<label for="input_ga_pkey_print" class="textinput">'.__('Private key fingerprint', 'google-apps-login').'</label>';
				echo "<div class='gal-lowerinput'>";
				echo "<span id='input_ga_pkey_print'>".htmlentities($saoptions['ga_pkey_print'])."</span>";
				echo '</div>';
				echo '<br class="clear">';
			}
		}
		
		echo '<label for="input_ga_keyfileupload" class="textinput gal_jsonkeyfile">'.__('Upload a new Service Account JSON file', 'google-apps-login').'</label>';
		echo '<label for="input_ga_keyjson" class="textinput gal_jsonkeytext" style="display: none;">'.__('Paste contents of JSON file', 'google-apps-login').'</label>';
		
		echo "<div class='gal-lowerinput'>";
		echo "<input type='hidden' name='MAX_FILE_SIZE' value='10240' />";
		echo "<input type='file' name='ga_keyfileupload' id='input_ga_keyfileupload' class='gal_jsonkeyfile'/>";
		echo "<a href='#' class='gal_jsonkeyfile'>Problem uploading file?</a>";
		echo "<textarea name='".$this->get_options_name()."[ga_keyjson]' id='input_ga_keyjson' class='gal_jsonkeytext' style='display: none;'></textarea>";
		echo "<a href='#' class='gal_jsonkeytext' style='display: none;'>Prefer the file upload?</a>";
		echo '</div>';
		echo '<br class="clear">';
		
		echo '<label for="input_ga_domainadmin" class="textinput">'.__('A Google Apps Domain admin\'s email', 'google-apps-login').'</label>';
		echo "<input id='input_ga_domainadmin' name='".$this->get_options_name()."[ga_domainadmin]' size='40' type='text' value='".esc_attr($options['ga_domainadmin'])."' class='textinput' />";
		echo '<br class="clear">';
		
		if (count($serviceacct_plugins) == 0) {
			?>
			</span>
			<?php
		}
		
		echo '</div>';
	}
	
	protected function ga_show_service_account_reqs($serviceacct_plugins) {
		$all_scopes = array();
		?>
		<p>A Service Account will be required for the following extensions, and they need the permission scopes listed:
			<table class="gal-admin-service-scopes">
				<thead>
					<tr>
						<td>Extension Name</td>
						<td>Scopes Requested</td>
						<td>Reason</td>
					</tr>
				</thead>
				<tbody>
		<?php
		foreach ($serviceacct_plugins as $plg) {
			if (is_array($plg) && count($plg) == 2) {
				$i = 0;
				foreach ($plg[1] as $k => $v) {
					echo '<tr>';
					if ($i==0) {
						echo '<td rowspan="'.count($plg[1]).'">'.htmlentities($plg[0]).'</td>';
					}
					echo '<td>'.htmlentities($k).'</td>';
					echo '<td>'.htmlentities($v).'</td>';
					echo '</tr>';
					$all_scopes[] = $k;
					++$i;
				}
			}
		}
		?>
				</tbody>
			</table>
		</p>
		
		<p>Here is a comma-separated list of API Scopes to copy and paste into your Google Apps admin security page (see instructions).
		<br />
		<div class="gal-admin-scopes-list"><?php echo htmlentities(implode(', ',array_unique($all_scopes))); ?></div>
		</p>
		<?php
		
	}
	
	// Has content in Basic
	protected function ga_section_text_end() {
	}
	
	// Has content in Premium
	protected function ga_domainsection_text() {
	}
		
	protected function ga_advancedsection_text() {
		echo '<div id="advanced-section" class="galtab">';
		echo '<p>';
		printf( __('Once you have the plugin working, you can try these settings to customize the login flow for your users.', 'google-apps-login')
				.' '.__('See <a href="%s" target="gainstr">instructions here</a>.', 'google-apps-login'),
				$this->calculate_instructions_url('a').'#advanced' );
		echo '</p>';

		$options = $this->get_option_galogin();
		
		echo "<input id='input_ga_force_permissions' name='".$this->get_options_name()."[ga_force_permissions]' type='checkbox' ".($options['ga_force_permissions'] ? 'checked' : '')." class='checkbox' />";
		echo '<label for="input_ga_force_permissions" class="checkbox plain">';
		_e( 'Force user to confirm Google permissions every time' , 'google-apps-login' );
		echo '</label>';
		
		echo '<br class="clear" />';
		
		echo "<input id='input_ga_auto_login' name='".$this->get_options_name()."[ga_auto_login]' type='checkbox' ".($options['ga_auto_login'] ? 'checked' : '')." class='checkbox' />";
		
		echo '<label for="input_ga_auto_login" class="checkbox plain">';
		_e( 'Automatically redirect to Google from login page' , 'google-apps-login' );
		echo '</label>';

		echo '<br class="clear" />';

		echo "<input id='input_ga_rememberme' name='".$this->get_options_name()."[ga_rememberme]' type='checkbox' ".($options['ga_rememberme'] ? 'checked' : '')." class='checkbox' />";

		echo '<label for="input_ga_rememberme" class="checkbox plain">';
		_e( 'Remember Me - do not log users out at end of browser session' , 'google-apps-login' );
		echo '</label>';

		echo '<br class="clear" />';
		
		echo "<input id='input_ga_poweredby' name='".$this->get_options_name()."[ga_poweredby]' type='checkbox' ".($options['ga_poweredby'] ? 'checked' : '')." class='checkbox' />";
		
		echo '<label for="input_ga_poweredby" class="checkbox plain">';
		_e( 'Display \'Powered By wp-glogin.com\' on Login form' , 'google-apps-login' );
		echo '</label>';
		
		$this->ga_advancedsection_extra();
		
		echo '<br class="clear" />';
		
		if (is_multisite()) {
			echo '<h3>'.__( 'Multisite Options' , 'google-apps-login').'</h3><p>';
			printf( __('This setting is for multisite admins only. See <a href="%s" target="gainstr">instructions here</a>.', 'google-apps-login')
			, $this->calculate_instructions_url('m').'#multisite' );
			echo '</p>';
			echo "<input id='input_ga_ms_usesubsitecallback' name='".$this->get_options_name()."[ga_ms_usesubsitecallback]' type='checkbox' ".($options['ga_ms_usesubsitecallback'] ? 'checked' : '')." class='checkbox'/>";
				
			echo '<label for="input_ga_ms_usesubsitecallback" class="checkbox plain">'.__( 'Use sub-site specific callback from Google' , 'google-apps-login').'</label>';
			echo '<br class="clear" />';
			
			echo '<p class="desc">';
			_e( 'Leave unchecked if in doubt' , 'google-apps-login');
			echo '</p>';
		}
		
		echo '</div>';
	}
	
	// Overridden in Commercial
	protected function ga_advancedsection_extra() {
	}

	public function ga_options_validate($input) {
		$newinput = Array();
		$newinput['ga_clientid'] = isset($input['ga_clientid']) ? trim($input['ga_clientid']) : '';
		$newinput['ga_clientsecret'] = isset($input['ga_clientsecret']) ? trim($input['ga_clientsecret']) : '';
		if(!preg_match('/^.{10}.*$/i', $newinput['ga_clientid'])) {
			add_settings_error(
			'ga_clientid',
			'tooshort_texterror',
			self::get_error_string('ga_clientid|tooshort_texterror'),
			'error'
			);
		}
		if(!preg_match('/^.{10}.*$/i', $newinput['ga_clientsecret'])) {
			add_settings_error(
			'ga_clientsecret',
			'tooshort_texterror',
			self::get_error_string('ga_clientsecret|tooshort_texterror'),
			'error'
			);
		}
		$newinput['ga_ms_usesubsitecallback'] = isset($input['ga_ms_usesubsitecallback']) ? (boolean)$input['ga_ms_usesubsitecallback'] : false;
		$newinput['ga_force_permissions'] = isset($input['ga_force_permissions']) ? (boolean)$input['ga_force_permissions'] : false;
		$newinput['ga_auto_login'] = isset($input['ga_auto_login']) ? (boolean)$input['ga_auto_login'] : false;
		$newinput['ga_poweredby'] = isset($input['ga_poweredby']) ? (boolean)$input['ga_poweredby'] : false;
		$newinput['ga_rememberme'] = isset($input['ga_rememberme']) ? (boolean)$input['ga_rememberme'] : false;
		
		// Service account settings
		$newinput['ga_domainadmin'] = isset($input['ga_domainadmin']) ? trim($input['ga_domainadmin']) : '';
		if (!preg_match('/^([A-Za-z0-9._%+-]+@([0-9a-z-]+\.)*[0-9a-z-]+\.[a-z]{2,63})?$/', $newinput['ga_domainadmin'])) {
			add_settings_error(
			'ga_domainadmin',
			'invalid_email',
			self::get_error_string('ga_domainadmin|invalid_email'),
			'error'
					);
		}

		// Submitting a JSON key for Service Account
		if (isset($_FILES['ga_keyfileupload']) || (isset($input['ga_keyjson']) && strlen(trim($input['ga_keyjson'])) > 0)) {
			if (!class_exists('gal_keyfile_uploader')) {
				$this->setIncludePath();
				require_once( 'keyfile_uploader.php' );
			}
			
			$saoptions = $this->get_sa_option();
			
			$kfu = new gal_keyfile_uploader('ga_keyfileupload', isset($input['ga_keyjson']) ? $input['ga_keyjson'] : '');
			$newemail = $kfu->getEmail();
			$newid = $kfu->getId();
			$newkey = $kfu->getKey();
			$newprint = $kfu->getPrint();
			if ($newemail != '' && $newkey != '' && $newid != '') {
				$saoptions['ga_serviceemail'] = $newemail;
				$saoptions['ga_serviceid'] = $newid;
				$saoptions['ga_sakey'] = $newkey;
				$saoptions['ga_pkey_print'] = $newprint;
				$this->save_sa_option($saoptions);
			}
			else if (($kfuerror = $kfu->getError()) != '') {
				add_settings_error(
				'ga_jsonkeyfile',
				$kfuerror,
				self::get_error_string('ga_jsonkeyfile|'.$kfuerror),
				'error'
				);
			}
		}
		
		$newinput['ga_version'] = $this->PLUGIN_VERSION;
		return $newinput;
	}
	
	protected function get_error_string($fielderror) {
		$local_error_strings = Array(
				'ga_clientid|tooshort_texterror' => __('The Client ID should be longer than that', 'google-apps-login') ,
				'ga_clientsecret|tooshort_texterror' => __('The Client Secret should be longer than that', 'google-apps-login'),
				'ga_serviceemail|invalid_email' => __('Service Account email must be a valid email addresses', 'google-apps-login'),
				'ga_domainadmin|invalid_email' => __('Google Apps domain admin must be a valid email address of one of your Google Apps admins', 'google-apps-login'),
				'ga_jsonkeyfile|file_upload_error' => __('Error with file upload on the server', 'google-apps-login'),
				'ga_jsonkeyfile|file_upload_error2' => __('Error with file upload on the server - file was too large', 'google-apps-login'),
				'ga_jsonkeyfile|file_upload_error6' => __('Error with file upload on the server - no temp directory exists', 'google-apps-login'),
				'ga_jsonkeyfile|file_upload_error7' => __('Error with file upload on the server - failed to write to disk', 'google-apps-login'),
				'ga_jsonkeyfile|no_content' => __('JSON key file was empty', 'google-apps-login'),
				'ga_jsonkeyfile|decode_error' => __('JSON key file could not be decoded correctly', 'google-apps-login'),
				'ga_jsonkeyfile|missing_values' => __('JSON key file does not contain all of client_email, client_id, private_key, and type', 'google-apps-login'),
				'ga_jsonkeyfile|not_serviceacct' => __('JSON key file does not represent a Service Account', 'google-apps-login'),
				'ga_jsonkeyfile|bad_pem' => __('Key cannot be coerced into a PEM key - invalid format in private_key of JSON key file', 'google-apps-login')
		);
		if (isset($local_error_strings[$fielderror])) {
			return $local_error_strings[$fielderror];
		}
		return __( 'Unspecified error' , 'google-apps-login');
	}
	
	protected function get_options_name() {
		return 'galogin';
	}

	protected function get_default_options() {
		return Array('ga_version' => $this->PLUGIN_VERSION, 
						'ga_clientid' => '', 
						'ga_clientsecret' => '', 
						'ga_ms_usesubsitecallback' => false,
						'ga_force_permissions' => false,
						'ga_auto_login' => false,
						'ga_poweredby' => false,
						'ga_rememberme' => false,
						'ga_sakey' => '',
						'ga_domainadmin' => '');
	}
	
	protected $ga_options = null;
	public function get_option_galogin() {
		if ($this->ga_options != null) {
			return $this->ga_options;
		}
			
		$option = get_site_option($this->get_options_name(), Array());
		
		$default_options = $this->get_default_options();
		foreach ($default_options as $k => $v) {
			if (!isset($option[$k])) {
				$option[$k] = $v;
			}
		}
		
		$this->ga_options = apply_filters( 'gal_options', $option );
		return $this->ga_options;
	}
	
	protected function save_option_galogin($option) {
		update_site_option($this->get_options_name(), $option);
		$this->ga_options = $option;
	}
	
	// Options for service account only
	protected function get_sa_options_name() {
		return 'ga_serviceacct';
	}
	
	protected $ga_sa_options = null;
	protected function get_sa_option() {
		if ($this->ga_sa_options != null) {
			return $this->ga_sa_options;
		}
		
		$ga_sa_options = get_site_option($this->get_sa_options_name(), Array());
		
		// Do we need to convert to separate service account settings, from older version?
		if (count($ga_sa_options) == 0) {
			$option = $this->get_option_galogin();
			if (isset($option['ga_keyfilepath']) || isset($option['ga_serviceemail'])) {
				$this->setIncludePath();
				if (!function_exists('gal_service_account_upgrade')) {
					require_once( 'service_account_upgrade.php' );
					gal_service_account_upgrade($option, $this->get_options_name(), $ga_sa_options, $this->get_sa_options_name());
					// options were updated by reference
					$this->save_option_galogin($option);
					$this->save_sa_option($ga_sa_options);
				}
			}
		}

		// Set defaults
		foreach (array('ga_sakey', 'ga_serviceemail', 'ga_serviceid', 'ga_pkey_print') as $k) {
			if (!isset($ga_sa_options[$k])) {
				$ga_sa_options[$k] = '';
			}
		}
		
		$this->ga_sa_options = apply_filters('gal_sa_options', $ga_sa_options );
		return $this->ga_sa_options;
	}
	
	protected function save_sa_option($saoptions) {
		update_site_option($this->get_sa_options_name(), $saoptions);
		$this->ga_sa_options = $saoptions;
	}
	
	public function ga_save_network_options() {
		check_admin_referer( $this->get_options_pagename().'-options' );
		
		if (isset($_POST[$this->get_options_name()]) && is_array($_POST[$this->get_options_name()])) {
			$inoptions = $_POST[$this->get_options_name()];
			$outoptions = $this->ga_options_validate($inoptions);
					
			$error_code = Array();
			$error_setting = Array();
			foreach (get_settings_errors() as $e) {
				if (is_array($e) && isset($e['code']) && isset($e['setting'])) {
					$error_code[] = $e['code'];
					$error_setting[] = $e['setting'];
				}
			}

			$this->save_option_galogin($outoptions);
			
			// redirect to settings page in network
			wp_redirect(
				add_query_arg(
					array( 'page' => $this->get_options_menuname(),
							'updated' => true,
							'error_setting' => $error_setting,
							'error_code' => $error_code ),
						network_admin_url( 'admin.php' )
				)
			);
			exit;
		}
	}
	
	protected function calculate_instructions_url($refresh='n') {
		return add_query_arg(
					array( 'garedirect' => urlencode( $this->get_login_url() ),
							'gaorigin' => urlencode( (is_ssl() || force_ssl_admin()
											? 'https://' : 'http://').$_SERVER['HTTP_HOST'] ),
							'ganotms' => is_multisite() ? 'false' : 'true',
							'gar' => urlencode( $refresh ),
							'utm_source' => 'Admin%20Instructions',
							'utm_medium' => 'freemium',
							'utm_campaign' => 'Freemium' ),
						$this->get_wpglogincom_baseurl()
				);
	}
	
	protected function get_wpglogincom_baseurl() {
		return 'http://wp-glogin.com/installing-google-apps-login/basic-setup/';
	}
	
	// Google Apps Login platform
	
	public function gal_get_clientid() {
		$options = $this->get_option_galogin();
		return $options['ga_clientid'];
	}
	
	public function get_Auth_AssertionCredentials($scopes, $sub_email='') {
		$options = $this->get_option_galogin();
		$saoptions = $this->get_sa_option();
		$this->setIncludePath();
		if (!class_exists('GoogleGAL_Auth_AssertionCredentials')) {
			require_once( 'Google/Auth/AssertionCredentials.php' );
		}
		
		if ($saoptions['ga_serviceemail'] == '' || $saoptions['ga_sakey'] == '') {
			throw new GAL_Service_Exception('Please configure Service Account in Google Apps Login setup');
		}
		
		$cred = new GoogleGAL_Auth_AssertionCredentials(
				// Replace this with the email address from the client.
				$saoptions['ga_serviceemail'],
				// Replace this with the scopes you are requesting.
				$scopes,
				$saoptions['ga_sakey'],
				''
		);
		$cred->setSignerClass('GoogleGAL_Signer_PEM');
			
		$cred->sub = $sub_email != '' ? $sub_email : $options['ga_domainadmin'];
		
		return $cred;
	}
	
	public function get_Google_Client() {
		$this->setIncludePath();
		if (!class_exists('GoogleGAL_Client')) {
			require_once( 'Google/Client.php' );
		}
		
		$client = new GoogleGAL_Client(apply_filters('gal_client_config_ini', null));
		$client->setApplicationName("Wordpress Site");
		return $client;
	}

	public function get_sa_admin_email() {
		$options = $this->get_option_galogin();
		return isset($options['ga_domainadmin']) ? $options['ga_domainadmin'] : '';
	}


	// PLUGINS PAGE
	
	public function ga_plugin_action_links( $links, $file ) {
		if ($file == $this->my_plugin_basename()) {
			$settings_link = '<a href="'.$this->get_settings_url().'">'.__( 'Settings' , 'google-apps-login').'</a>';
			array_unshift( $links, $settings_link );
		}
	
		return $links;
	}
	
	// HOOKS AND FILTERS
	// *****************
	
	protected function add_actions() {
		add_action('plugins_loaded', array($this, 'ga_plugins_loaded'));
		
		add_action('login_enqueue_scripts', array($this, 'ga_login_styles'));
		add_action('login_form', array($this, 'ga_login_form'));
		add_filter('authenticate', array($this, 'ga_authenticate'), 5, 3);
		
		add_filter('login_redirect', array($this, 'ga_login_redirect'), 5, 3 );
		add_action('init', array($this, 'ga_init'), 1);
		
		add_action('admin_init', array($this, 'ga_admin_init'), 5, 0);
				
		add_action(is_multisite() ? 'network_admin_menu' : 'admin_menu', array($this, 'ga_admin_menu'));
		
		add_filter('gal_get_clientid', Array($this, 'gal_get_clientid') );

		if (is_multisite()) {
			add_filter('network_admin_plugin_action_links', array($this, 'ga_plugin_action_links'), 10, 2 );
			add_action('network_admin_edit_'.$this->get_options_menuname(), array($this, 'ga_save_network_options'));
		}
		else {
			add_filter( 'plugin_action_links', array($this, 'ga_plugin_action_links'), 10, 2 );
		}
	}

	// Abstract

	protected function my_plugin_basename() {
		throw new Exception("core_google_apps_login is an abstract class");
	}

	protected function my_plugin_url() {
		throw new Exception("core_google_apps_login is an abstract class");
	}

}

class GAL_Service_Exception extends Exception {
}

?>