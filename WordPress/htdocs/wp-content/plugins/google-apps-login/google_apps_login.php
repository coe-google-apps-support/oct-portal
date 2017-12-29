<?php

/**
 * Plugin Name: Google Apps Login
 * Plugin URI: http://wp-glogin.com/
 * Description: Simple secure login for Wordpress through users' Google Apps accounts (uses secure OAuth2, and MFA if enabled)
 * Version: 3.2
 * Author: Dan Lester
 * Author URI: http://wp-glogin.com/
 * License: GPL3
 * Network: true
 * Text Domain: google-apps-login
 * Domain Path: /lang
 */

if (class_exists('core_google_apps_login')) {
	global $gal_core_already_exists;
	$gal_core_already_exists = true;
}
else {
	require_once( plugin_dir_path( __FILE__ ) . '/core/core_google_apps_login.php' );
}

class basic_google_apps_login extends core_google_apps_login {
	
	protected $PLUGIN_VERSION = '3.2';
	
	// Singleton
	private static $instance = null;
	
	public static function get_instance() {
		if (null == self::$instance) {
			self::$instance = new self;
		}
		return self::$instance;
	}

	protected function createUserOrError($userinfo, $options) {
		apply_filters( 'register_coe_user', null, $userinfo, $options );		
	}
	
	public function ga_activation_hook($network_wide) {
		parent::ga_activation_hook($network_wide);
		
		// If installed previously, keep 'poweredby' to off (default) since they were used to that
		$old_options = get_site_option($this->get_options_name());
	
		if (!$old_options) {
			$new_options = $this->get_option_galogin();
			$new_option['ga_poweredby'] = true;
			$this->save_option_galogin($new_option);
		}
	}

    protected function add_actions() {
        parent::add_actions();
        add_action('wp_ajax_gal_drip_submitted', array($this, 'gal_drip_submitted'));
    }
		
	protected function ga_section_text_end() {
	?>
		<p><b><?php _e( 'For full support, and premium features that greatly simplify WordPress user management for admins, please visit:' , 'google-apps-login'); ?>
		<a href="http://wp-glogin.com/glogin/?utm_source=Admin%20Promo&utm_medium=freemium&utm_campaign=Freemium" target="_blank">http://wp-glogin.com/</a></b>
		</p>
	<?php
	}
	
	protected function ga_options_do_sidebar() {
		$drivelink = "http://wp-glogin.com/drive/?utm_source=Admin%20Sidebar&utm_medium=freemium&utm_campaign=Drive";
		$upgradelink = "http://wp-glogin.com/glogin/?utm_source=Admin%20Sidebar&utm_medium=freemium&utm_campaign=Freemium";
		$avatarslink = "http://wp-glogin.com/avatars/?utm_source=Admin%20Sidebar&utm_medium=freemium&utm_campaign=Avatars";
		$aioilink = "http://wp-glogin.com/intranet/?utm_source=Admin%20Sidebar&utm_medium=freemium&utm_campaign=AIOI";
		
		$adverts = Array();
		
		$adverts[] = '<div>'
		.'<a href="'.$upgradelink.'" target="_blank">'
		.'<img src="'.$this->my_plugin_url().'img/basic_loginupgrade.png" />'
		.'</a>'
		.'<span>Buy our <a href="'.$upgradelink.'" target="_blank">premium Login plugin</a> to revolutionize user management</span>'
		.'</div>';
		
		$adverts[] = '<div>'
		.'<a href="'.$drivelink.'" target="_blank">'
		.'<img src="'.$this->my_plugin_url().'img/basic_driveplugin.png" />'
		.'</a>'
		.'<span>Try our <a href="'.$drivelink.'" target="_blank">Google Drive Embedder</a> plugin</span>'
		.'</div>';

		$adverts[] = '<div>'
		.'<a href="'.$avatarslink.'" target="_blank">'
		.'<img src="'.$this->my_plugin_url().'img/basic_avatars.png" />'
		.'</a>'
		.'<span>Bring your site to life with <a href="'.$avatarslink.'" target="_blank">Google Profile Avatars</a></span>'
		.'</div>';

		$adverts[] = '<div>'
		.'<a href="'.$aioilink.'" target="_blank">'
		.'<img src="'.$this->my_plugin_url().'img/basic_aioi.png" />'
		.'</a>'
		.'<span>Instantly turn WordPress into a corporate intranet with <a href="'.$aioilink.'" target="_blank">All-In-One Intranet</a></span>'
		.'</div>';
		
		$startnum = (int)date('j');
		
		echo '<div id="gal-tableright" class="gal-tablecell">';

		$this->output_drip_form();
		
		for ($i=0 ; $i<2 ; $i++) {
			echo $adverts[($startnum+$i) % 4];
		}
		
		echo '</div>';
		
	}

	protected function output_drip_form() {
        $userdata = wp_get_current_user();
        if (!$userdata) {
            return;
        }
		$signedup = get_user_meta($userdata->ID, 'gal_user_signedup_to_drip', true);

		if (!$signedup) {

            $useremail = $userdata->user_email;

			?>
			<div>
				<form action="https://www.getdrip.com/forms/9468024/submissions" method="post" target="_blank" data-drip-embedded-form="9468024" id="gal-drip-signup-form">
					<h3 data-drip-attribute="headline">Get the most out of Google Apps and WordPress</h3>
					<p data-drip-attribute="description">
                        Register your email address to receive information on building a WordPress site
                        that truly integrates G Suite and WordPress.
                    </p>
					<div>
						<label for="fields[email]">Email Address</label>
                        <br />
						<input type="email" name="fields[email]" value="<?php echo esc_js($useremail); ?>" />
                        <br />
						<input type="submit" name="submit" value="Sign Up" data-drip-attribute="sign-up-button" class="gal-drip-signup-button" />
					</div>
                    <p class="gal-drip-unsubscribe">
                        You can unsubscribe at any time, and we will never share your email address.
                    </p>
				</form>
			</div>
			<?php
		}
	}

    public function gal_drip_submitted() {
        $userdata = wp_get_current_user();
        if (!$userdata) {
            return;
        }
        update_user_meta($userdata->ID, 'gal_user_signedup_to_drip', true);
    }
	
	protected function ga_domainsection_text() {
		echo '<div id="domain-section" class="galtab">';
		
		echo '<p>'.__('The Domain Control section is only applicable to the premium and enterprise versions of this plugin.', 'google-apps-login').'</p>';
		echo '<p>';
		_e( 'In this basic version of the plugin, any <i>existing</i> WordPress account corresponding to a Google email address can authenticate via Google.', 'google-apps-login');
		echo '</p>';
		?>
		
		<h3>Premium Upgrade</h3>
		
		<p>In our professional plugins, you can specify your G Suite (Google Apps) domain name to obtain more powerful features.</p>

		<ul class="ul-disc">
			<li>Save time and increase security</li>
			<li>Completely forget about WordPress user management &ndash; it syncs users from G Suite (Google Apps) automatically</li>
			<li>Ensures that employees who leave or change roles no longer have unauthorized access to sensitive sites</li>
			<li>Specify Google Groups or Organizational Units whose members should be mapped to different roles in WordPress (Enterprise only)</li>
		</ul>
		
		<p>Find out more about purchase options on our website:
		<a href="https://wp-glogin.com/glogin/?utm_source=Domain%20Control&utm_medium=freemium&utm_campaign=Freemium" target="_blank">http://wp-glogin.com/</a>
		</p>
		
		<?php
		echo '</div>';
	}
	
	protected function set_other_admin_notices() {
		global $pagenow;
		if (in_array($pagenow, array('users.php', 'user-new.php')) ) {
			$no_thanks = get_user_meta(get_current_user_id(), $this->get_options_name().'_no_thanks', true);
			if (!$no_thanks) {
				if (isset($_REQUEST['google_apps_login_action']) && $_REQUEST['google_apps_login_action']=='no_thanks') {
					$this->ga_said_no_thanks(null);
				}
				
				add_action('admin_notices', Array($this, 'ga_user_screen_upgrade_message'));
				if (is_multisite()) {
					add_action('network_admin_notices', Array($this, 'ga_user_screen_upgrade_message'));
				}
			}
		}
	}
	
	public function ga_said_no_thanks( $data ) {
		update_user_meta(get_current_user_id(), $this->get_options_name().'_no_thanks', true);
		wp_redirect( remove_query_arg( 'google_apps_login_action' ) );
		exit;
	}
	
	public function ga_user_screen_upgrade_message() {
		$purchase_url = 'http://wp-glogin.com/glogin/?utm_source=User%20Pages&utm_medium=freemium&utm_campaign=Freemium';
		$nothanks_url = add_query_arg( 'google_apps_login_action', 'no_thanks' );
		echo '<div class="updated"><p>';
		echo sprintf( __('Completely forget about WordPress user management - upgrade to <a href="%s">Google Apps Login Premium or Enterprise</a> to automatically sync users from your Google Apps domain', 'google-apps-login'),
				$purchase_url );
		echo ' &nbsp; <a href="'.$purchase_url.'" class="button-secondary">' . __( 'Find out more', 'google-apps-login' ) . '</a>';
		echo '&nbsp;<a href="' . esc_url( $nothanks_url ) . '" class="button-secondary">' . __( 'No Thanks', 'google-apps-login' ) . '</a>';
		echo '</p></div>';
	}
	
	public function my_plugin_basename() {
		$basename = plugin_basename(__FILE__);
		if ('/'.$basename == __FILE__) { // Maybe due to symlink
			$basename = basename(dirname(__FILE__)).'/'.basename(__FILE__);
		}
		return $basename;
	}
	
	protected function my_plugin_url() {
		$basename = plugin_basename(__FILE__);
		if ('/'.$basename == __FILE__) { // Maybe due to symlink
			return plugins_url().'/'.basename(dirname(__FILE__)).'/';
		}
		// Normal case (non symlink)
		return plugin_dir_url( __FILE__ );
	}

}

// Global accessor function to singleton
function galbasicGoogleAppsLogin() {
	return basic_google_apps_login::get_instance();
}

// Initialise at least once
galbasicGoogleAppsLogin();

if (!function_exists('GoogleAppsLogin')) {
	function GoogleAppsLogin() {
		return galbasicGoogleAppsLogin();
	}
}

?>
