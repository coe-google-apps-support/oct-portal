<?php
/*
Plugin Name: City of Edmonton Ideas
Description: City of Edmonton Ideas
Author: Dan Chenier
Author URI: https://github.com/dchenier
*/

// based on https://wpshout.com/writing-wordpress-reactjs-app-plugin/

// should be refactored into its own class

// CoeIdeasWebpackBuildFiles.php is dynamically built by webpack
require_once( 'CoeIdeasWebpackBuiltFiles.php' );

// Shortcode to output needed markup
add_shortcode( 'coe_ideas_new', 'coe_ideas_show_new' );
function coe_ideas_show_new() {
	return '<div id="app-ideas"></div>';
}

add_action( 'wp_enqueue_scripts', 'coe_ideas_enqueue_scripts' );
function coe_ideas_enqueue_scripts() {
	// // Only load for the specific posts?
	// if( ! is_single( 10742 ) ) {
	// 	return;
    // }

    // order needs to be like this 'manifest, vendor, app, others'
    $scripts = CoeIdeasWebpackBuiltFiles::$jsFiles;
    usort($scripts, 'coe_ideas_enqueue_scripts_get_compare_script_order');
    foreach ($scripts as $jsFile) {
        wp_enqueue_script( $jsFile, plugin_dir_url( __FILE__ ) . $jsFile, null, null, true );
    }

    foreach ( CoeIdeasWebpackBuiltFiles::$cssFiles as $cssFile) {
        wp_enqueue_style( $cssFile, plugin_dir_url( __FILE__ ) . $cssFile );
    }
}

function coe_ideas_enqueue_scripts_get_compare_script_order($a, $b) {
    // order needs to be like this 'manifest, vendor, app, others'
    return coe_ideas_enqueue_scripts_get_compare_script_order_get_arg_weight($a) 
         > coe_ideas_enqueue_scripts_get_compare_script_order_get_arg_weight($b);
}

function coe_ideas_enqueue_scripts_get_compare_script_order_get_arg_weight($a) {
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