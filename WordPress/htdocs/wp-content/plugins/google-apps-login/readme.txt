=== Plugin Name ===
Contributors: danlester
Tags: login, google, authentication, oauth, google login, google apps, g suite, sso, single-sign-on, auth, intranet
Requires at least: 4.0
Tested up to: 4.9
Stable tag: 3.2
License: GPLv3
License URI: http://www.gnu.org/licenses/gpl-3.0.html

Simple secure login and user management for Wordpress through your G Suite / Google Apps domain
(uses secure OAuth2, and MFA if enabled)

== Description ==

Google Apps Login allows existing Wordpress user accounts to login to your website using Google to securely authenticate their account. This means that if they are already logged into Gmail for example, they can simply click their way through the Wordpress login screen - no username or password is explicitly required!

Google Apps Login uses the latest **secure OAuth2 authentication recommended by Google**, including 2-factor Auth if enabled for your G Suite (formerly Google Apps) accounts.
This is far simpler to configure than the older SAML protocol.

Trusted by thousands of organizations from schools to large public companies, Google Apps Login is the most popular enterprise grade plugin enabling login and user management based on your Google Apps domain.

Plugin setup requires you to have admin access to any G Suite (Google Apps) domain, or a regular Gmail account, to register and obtain two simple codes from Google.

= Support and Premium features =

Full support and premium features are also available for purchase:

Eliminate the need for G Suite (Google Apps) domain admins to separately manage WordPress user accounts, and get peace
of mind that only authorized employees have access to your organizations's websites and intranet.

**See [our website at wp-glogin.com](http://wp-glogin.com/glogin/?utm_source=Login%20Readme%20Top&utm_medium=freemium&utm_campaign=Freemium) for more details.**

The Premium version allows everyone in your G Suite (Google Apps) domain to login to WordPress - an account will be automatically created in WordPress if one doesn't already exist.

Our Enterprise version goes further, allowing you to specify granular access and role controls based on Google Group or Organizational Unit membership.
You can also see logs of accounts created and roles changed by the plugin.


= Extensible Platform =

Google Apps Login allows you to centralize your site's Google functionality and build your own extensions, or use third-party extensions, which require no configuration themselves and share the same user authentication and permissions that users already allowed for Google Apps Login itself.

Using our platform, your website appears to Google accounts as one unified 'web application', making it more secure and easier to manage.

[Google Drive Embedder](http://wp-glogin.com/wpgoogledriveembedder) is an extension plugin allowing
users to browse for Google Drive documents to embed directly in their posts or pages.

[Google Apps Directory](http://wp-glogin.com/wpgoogleappsdirectory) is an extension plugin allowing
logged-in users to search your Google Apps employee directory from a widget on your intranet or client site.

[Google Profile Avatars](http://wp-glogin.com/avatars/?utm_source=Login%20Readme%20Avatars&utm_medium=freemium&utm_campaign=Freemium) 
is available on our website. It displays users' Google profile photos in place of their avatars throughout your site.

Google Apps Login works on single or multisite WordPress websites or private intranets.

= Requirements =

One-click login will work for the following domains and user accounts:

*  G Suite Basic (Google Apps for Work)
*  G Suite Business (Google Apps Unlimited for Work)
*  G Suite for Education (Google Apps for Education)
*  G Suite for Non-profits (Google Apps for Non-profits)
*  G Suite for Government (Google Apps for Government)
*  Personal gmail.com and googlemail.com emails

Google Apps Login uses the latest secure OAuth2 authentication recommended by Google. Other 3rd party authentication plugins may allow you to use your Google username and password to login, but they do not do this securely unless they also use OAuth2. This is discussed further in the [FAQ](http://wordpress.org/plugins/google-apps-login/faq/).

= Translations =

This plugin currently operates in the following languages:

* English - default
* French (fr_FR) - translated by Lucien Ntumba of [GPC.solutions](http://gpcsolutions.fr/)
* Spanish (es_ES) - translated by David Perez of [Closemarketing](https://www.closemarketing.es/)
* Danish (da_DK) - translated by Alexander Leo-Hansen of [alexanderleohansen.dk](http://alexanderleohansen.dk/)
* Serbo-Croatian (sr_RS) - translated by Borisa Djuraskovic of [Web Hosting Hub](http://www.webhostinghub.com/)
* Arabic (ar_SA) - translated by [Jeremy Varnham](http://profiles.wordpress.org/jvarn13)
* Ukrainian (uk_UA) - translated by Serj Kondrashov
* Dutch (nl_NL) - translated by Noell Taravati of [Peppix](http://www.peppix.nl/)
* Swedish (sv_SE) - translated by Johan Linde of [S/Y ELLEN](http://syellen.se/)
* Italian (it_IT) - translated by Giorgio Draghetti of [tipinoncomuni](http://tipinoncomuni.it/)
* Persian (fa_IR) - translated by [Saeed1000](https://profiles.wordpress.org/saeed1000/)
* Belarussian (be_BY) - translated by Natasha Dyatko of [UStarCash](https://www.ustarcash.com/)
* Indonesian (id_ID) - translated by Jordan Silaen of [ChameleonJohn.com](http://ChameleonJohn.com/)

We welcome volunteers to translate into their own language. If you would like to contribute a translation, please click Translate under Contributors & Developers below.

= Website and Upgrades =

Please see our website [http://wp-glogin.com/](http://wp-glogin.com/?utm_source=Login%20Readme%20Website&utm_medium=freemium&utm_campaign=Freemium) for more information about this free plugin and extra features available in our Premium and Enterprise upgrades, plus support details, other plugins, and useful guides for admins of WordPress sites and Google Apps.

The [Premium and Enterprise versions](http://wp-glogin.com/glogin/?utm_source=Login%20Readme%20PremEnt&utm_medium=freemium&utm_campaign=Freemium) eliminate the need to manage user accounts in your WordPress site - everything is synced from Google Apps instead.

If you are building your organization's intranet on WordPress, try out our [All-In-One Intranet plugin](http://wp-glogin.com/intranet/?utm_source=Login%20Readme%20AIOI&utm_medium=freemium&utm_campaign=Freemium).

== Screenshots ==

1. User login screen can work as normal or via Google's authentication system
2. Login to Google account - only if not already logged in to Google within the browser
3. First time only grant permissions for Google to connect to the WordPress site
4. Admin obtains two simple codes from Google to set up - easy instructions to follow 

== Frequently Asked Questions ==

= How can I obtain support for this product? =

Full support is available if you purchase the appropriate license from the author via: [http://wp-glogin.com/glogin/](http://wp-glogin.com/glogin/?utm_source=Login%20Readme%20Premium&utm_medium=freemium&utm_campaign=Freemium)

Please feel free to email [contact@wp-glogin.com](mailto:contact@wp-glogin.com) with any questions, as we may be able to help, but you may be required to purchase a support license if the problem is specific to your installation or requirements.

We may occasionally be able to respond to support queries posted on the 'Support' forum here on the wordpress.org plugin page, but we recommend sending us an email instead if possible.

= Is login restricted to the G Suite domain I use to set up the plugin? =

No, once you set up the plugin, any WordPress accounts whose email address corresponds to *any* Google account, whether on a different G Suite domain or even a personal gmail.com account, will be able to use 'Login with Google' to easily connect to your WordPress site.

However, our [premium plugin](http://wp-glogin.com/glogin/?utm_source=Login%20Readme%20FAQ&utm_medium=freemium&utm_campaign=Freemium) has features that greatly simplify your WordPress user management if your WordPress users are mostly on the same G Suite domain(s).

= Does the plugin work with HTTP or HTTPS login pages? =

The plugin will work whether your site is configured for HTTP or HTTPS.

However, you may have configured your site to run so that the login pages can be accessed by *either* HTTP *or* HTTPS. In that case, you may run into problems. 
We recommend that you set [FORCE_SSL_ADMIN](http://codex.wordpress.org/Administration_Over_SSL) 
to true. This will ensure that all users are consistently using HTTPS
for login.

You may then need to ensure the Redirect URL and Web Origin in the Google Cloud Console are set as HTTPS (this will make sense if you follow the installation instructions again).

If for some reason you cannot set FORCE_SSL_ADMIN, then instead you can add two URLs to the Google Cloud Console for each entry, e.g. Redirect URL = http://wpexample.com/wp-login.php, and then add another one for https://wpexample.com/wp-login.php. Same idea for Web Origin.

= Does the plugin work on Multisite? =

It is written, tested, and secure for multisite WordPress, both for subdirectories and subdomains, and *must* be activated network-wide for security reasons.

There are many different possible configurations of multisite WordPress, however, so you must test carefully if you have any other plugins or special setup.

In a multisite setup, you will see an extra option in Settings -> Google Apps Login, named 'Use sub-site specific callback from Google'. Read details in the configuration instructions (linked from the Settings page). This setting will need to be ON if you are using any domain mapping plugin, and extra Redirect URIs will need to be registered in Google Cloud Console.

= Is it secure? =

Yes, and depending on your setup, it can be much more secure than just using WordPress usernames and passwords.

However, the author does not accept liability or offer any guarantee, and it is your responsibility to ensure that your site is secure in the way you require.

In particular, other plugins may conflict with each other, and different WordPress versions and configurations may render your site insecure.

= Does it conflict with any other plugins? =

Sometimes conflicts can arise. We have built workarounds for some problems, and would always appreciate your feedback to resolve any issues you might encounter yourself.

One known issue is with iThemes Security: the settings 'filter suspicious query strings' and 'filter long URL strings' can both cause intermittent conflicts and should be turned off if you are happy with the implications.

My Private Site - Try setting the My Private Site option "Omit ?redirect_to= from URL (this option is recommended for Custom Login pages)".

WP Email Login - incompatible with Google Apps Login

= How does it compare to other 3rd party auth plugins? =

Google Apps Login uses the latest secure OAuth2 authentication recommended by Google. Other 3rd party authentication plugins may allow you to use your Google username and password to login, but they do not always do this securely:

*  Other plugins: Users' passwords may be handled by your blog's server, potentially unencrypted. If these are compromised, hackers would be able to gain access to your Google email accounts! This includes all 
[G Suite apps](https://gsuite.google.com/products/) (Gmail, Drive, Calendar etc), and any other services which use your Google account to login.

*  This plugin: Users' passwords are only ever submitted to Google itself, then Google is asked to authenticate the user to your WordPress site. This means Multi-factor Authentication can still be used (if set up on your Google account). 
Your website only requires permission to authenticate the user and obtain basic profile data - it can never have access to your emails and other data.

= What are the system requirements? =

*  PHP 5.2.x or higher with JSON extensions
*  Wordpress 3.5 or above

And you will need a Google account to set up the plugin.


== Installation ==

To set up the plugin, you will need access to a G Suite (Google Apps) domain as an administrator, or just a regular Gmail account.

Easiest way:

1. Go to your WordPress admin control panel's plugin page
1. Search for 'Google Apps Login'
1. Click Install
1. Click Activate on the plugin
1. Go to 'Google Apps Login' under Settings in your Wordpress admin area
1. Follow the instructions on that page to obtain two codes from Google, and also submit two URLs back to Google

If you cannot install from the WordPress plugins directory for any reason, and need to install from ZIP file:

1. Upload `googleappslogin` directory and contents to the `/wp-content/plugins/` directory, or upload the ZIP file directly in the Plugins section of your Wordpress admin
1. Follow the instructions from step 4 above

Personalized instructions to configure the plugin by registering your site with Google Apps are linked from 
the WordPress admin panel once you have activated the plugin. For a (non-personalized) preview of these instructions please [click here](http://wp-glogin.com/installing-google-apps-login/basic-setup/).

== Changelog ==

= 3.2 =

Added workaround for incompatibility with WPMU Defender plugin's new 2FA feature.
Updated compatibility with the upcoming WordPress 4.9 release.

= 3.0 =

Internal changes to Google Client library. Essential for the latest versions of some extension plugins such as Google Drive Embedder.

= 2.10.5 =

Multisite improvements: better handling of COOKIE_DOMAIN configuration and also allows login redirects straight to subsites even when login is handled by the root site's wp-login.php page.
Login page cookies now last for the length of the current browser session instead of for a fixed time, so this should reduce unexpected 'Session mismatch' errors.

Ensures plugin options are not loaded until 'plugins_loaded' stage. This makes it easier to use the gal_options hook more reliably.

Added language files for be_BY.
Added filters 'gal_options' and 'gal_sa_options' so you can configure settings using PHP code.

Changed the way Google client library is included to avoid conflicts with other Google-related plugins that set the include path
in a way that doesn't allow for other plugins.

= 2.9.7 =

Added gal_set_login_cookie filter so you can prevent Google Apps Login from setting its wordpress_google_apps_login cookie under
certain circumstances. It only technically needs to be set on the wp-login.php page in most installations, and you may find
that if it sets the cookie on any page load (which it does when not already set) then this prevents caching on heavy traffic pages.

= 2.9.6 =

New 'Remember Me' in advanced options. Check to ensure users are not automatically logged out at the end of their browser session.
This applies to all users using 'Login with Google'. It has the same functionality as checking the 'Remember Me' checkbox on
the login form when using regular WordPress username/password to login.

Improved handling of errors when multiple versions of the plugin are inadvertently activated.

= 2.8.17 =

Added filter gal_login_button_text so developers can customize the 'Login with Google' button in all versions (in Premium/Enterprise,
it is possible to set the button text in settings, and that will always take priority if set).

Fixed a text injection vulnerability whereby it was possible for a third party to trick a user into viewing a version of the login page
containing an error message entirely of their own choosing - all sites should upgrade to this plugin version.

= 2.8.16 =

Updated to work correctly alongside some plugins that change the login URL from /wp-login.php to something else.

= 2.8.15 =

Removed a WordPress function that is deprecated in WP 4.4 - force_ssl_login
This could have resulted in some visible warning messages.

= 2.8.14 =

Updated some URLs pointing to information about Premium/Enterprise upgrades.
Readme updated.

= 2.8.13 =

Ready for WordPress 4.4.
New translation strings for languages.

= 2.8.12 =

Opportunity to sign up for emails on Google Apps and WordPress, from settings page.

= 2.8.11 =

Service Account Client ID is imported from JSON key file so user can copy and paste it into admin.google.com

= 2.8.10 =

Changed name of login cookie from 'google_apps_login' to 'wp_google_apps_login'.

= 2.8.3 =

Resolved conflict with some other plugins over Google-related function names.

= 2.8.1 =

'Session mismatch' warning should be much less of a problem now.

= 2.8 =

Session mismatch (could be a problem setting cookies) should now occur less frequently. Service Account can have no admin email (for gmail.com accounts).

= 2.7 =

Accepts filter gal_client_config_ini containing filesystem path to an INI file to supply to Google's client library on instantiation, so you can override settings.
Added substitution version core/Google/IO/DebugVersionOfCurl.php to replace core/Google/IO/Curl.php temporarily to log communications to Google's server for debugging purposes.

= 2.5.2 =

Service Account settings can be uploaded by copy-and-paste of JSON file contents as well as just uploading the file directly.

= 2.5 =

Platform extended to provide Service Account settings.

= 2.4.4 =

Readme updates and tidied settings page.

= 2.4.3 =

New hooks for profile photos. Updated Google client library.

= 2.3.1 =

Fixed conflicts with some other plugins such as Google Analyticator which use similar Google client libraries.

= 2.3 =

Better organized config pages. 

Uses latest Google client library. 

Option to link to wp-glogin.com from login page.

= 2.2 =

Fix for an error seen on multisite admin. Added Arabic translation.

= 2.1 =

New design to support multiple languages. Includes Serbo-Croatian. Fixed some conflicts 
with other plugins when used in 'auto redirect to Google' mode.

= 2.0 =

Our platform provides centralized setup and management of Google-related features in your 
WordPress site and plugins.

Other developers can easily extend our Google authentication into their own plugins. 

= 1.4 =

Added clearer instructions, plus new options: automatically redirect users
to Login via Google; plus force users to fully approve access to their
Google account every time they login (allowing them to switch accounts if only
logged into the wrong one, as well as making the process clearer).

= 1.3 =
Much neater support for redirecting users to most appropriate page post-login,
especially on multisite installations; Better notices guiding admins through 
configuration

= 1.2 =
Upgrade to match WordPress 3.8; 
More extensible code

= 1.1 =
Increased security - uses an extra authenticity check; 
Better support for mal-configured Google credentials; 
No longer uses PHP-based sessions - will work on even more WordPress configurations

= 1.0 =
All existing versions are functionally identical - no need to upgrade.

