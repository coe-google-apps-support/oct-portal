<?php
/**
 * @package coe_gravity_forms_to_initiatives
 * @version 0.1
 */
/*
Plugin Name: CoE Gravity Forms to Initiatives
Plugin URI: TBD
Description: Hooks into Gravity Forms to submit forms to Initiatives backend on certain specified forms
Author: Dan Chenier
*/

/**
 * Singleton class for handling the coe-gravity-forms-to-initiatives plugin
 */
class coe_gravity_forms_to_initiatives {

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
        //see https://docs.gravityforms.com/gform_after_submission/
		add_action( 'gform_after_submission', array($this, 'after_gform_submission'), 10, 2 );
    }
    
    public function after_gform_submission( $entry, $form ) {
        // TODO: send form to initiatives API...
        error_log( 'BEGIN-Submitting Gravity Form' );

        $post_url = get_site_url() . '/api/initiatives';
        $body = array(
            'title' => rgar( $entry, '1' ),
            'description' => rgar( $entry, '2' ),
            );

        #error_log( 'post_url is ' . $post_url);
        #error_log( 'title is ' . rgar( $entry, '1' ));
        #error_log( 'body is ' . rgar( $entry, '2' ));

        $bodyJson = wp_json_encode($body);

        error_log( 'Sending ' . $bodyJson . ' to initiatives API...');

        $request = new WP_Http();
        #TODO: set the Content-Type header to application/json
        #      copy the incoming cookies to this request
        $response = $request->post( $post_url, $bodyJson );

        error_log( 'END-Submitting Gravity Form!' );
    }

}


// Global accessor function to singleton
function coeGravityFormsToInitiatives() {
	return coe_gravity_forms_to_initiatives::get_instance();
}

// ensure called at least once
coeGravityFormsToInitiatives();

?>