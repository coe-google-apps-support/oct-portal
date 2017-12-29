
jQuery(document).ready(function() {
	
	function galSetActionToTab(id) {
		var frm = jQuery('#gal_form');
		frm.attr('action', frm.attr('action').replace(/(#.+)?$/, '#'+id) );
	}

	jQuery('#gal-tabs').find('a').click(function() {
			jQuery('#gal-tabs').find('a').removeClass('nav-tab-active');
			jQuery('.galtab').removeClass('active');
			var id = jQuery(this).attr('id').replace('-tab','');
			jQuery('#' + id + '-section').addClass('active');
			jQuery(this).addClass('nav-tab-active');
			
			// Set submit URL to this tab
			galSetActionToTab(id);
	});
	
	// Did page load with a tab active?
	var active_tab = window.location.hash.replace('#','');
	if ( active_tab != '') {
		var activeSection = jQuery('#' + active_tab + '-section');
		var activeTab = jQuery('#' + active_tab + '-tab');
	
		if (activeSection && activeTab) {
			jQuery('#gal-tabs').find('a').removeClass('nav-tab-active');
			jQuery('.galtab').removeClass('active');
	
			activeSection.addClass('active');
			activeTab.addClass('nav-tab-active');
			galSetActionToTab(active_tab);
		}
	}
	
	// JSON keyfile Browse for File <-> Textarea
	jQuery('a.gal_jsonkeyfile').on('click', function(e){
		jQuery('input#input_ga_keyfileupload').replaceWith(
				jQuery("<input type='file' name='ga_keyfileupload' id='input_ga_keyfileupload' class='gal_jsonkeyfile'/>")
		);
		jQuery('.gal_jsonkeyfile').hide();
		jQuery('.gal_jsonkeytext').show();
		e.preventDefault();
	});
	jQuery('a.gal_jsonkeytext').on('click', function(e){
		jQuery('.gal_jsonkeytext').hide();
		jQuery('.gal_jsonkeyfile').show();
		jQuery('textarea#input_ga_keyjson').val('');
		e.preventDefault();
	});
	
	// Dependent fields in premium
	// Default role only makes sense if Auto-create users is checked
	clickfn = function() {
		jQuery('#ga_defaultrole').prop('disabled',  !jQuery('#input_ga_autocreate').is(':checked'));
	};
	jQuery('#input_ga_autocreate').on('click', clickfn);
	clickfn();
	
	// Only allow Completely hide WP login if Disable WP login for my domain is checked
	clickfn2 = function() {
		jQuery('#input_ga_hidewplogin').prop('disabled',  !jQuery('#input_ga_disablewplogin').is(':checked'));
	};
	jQuery('#input_ga_disablewplogin').on('click', clickfn2);
	clickfn2();
	
	// Only bother with any domain-specific options if a domain has been entered
	if (jQuery('#input_ga_domainname').length > 0) {
		domainchangefn = function() {
			var domainname = jQuery('#input_ga_domainname').val().trim();
			jQuery('#domain-section input.gal_needsdomain').prop('disabled', domainname == '');
		};
		jQuery('#input_ga_domainname').on('change', domainchangefn);
		domainchangefn();
	}
	
	// Show service account button
	jQuery('#gal-show-admin-serviceacct').on('click', function(e) {
		jQuery('#gal-hide-admin-serviceacct').show();
		jQuery('#gal-show-admin-serviceacct').hide();
		e.preventDefault();
	});
	
	// Copy and paste click
	function selectText(element) {
	    var range, selection;
	    
	    if (document.body.createTextRange) { //ms
	        range = document.body.createTextRange();
	        range.moveToElementText(element);
	        range.select();
	    } else if (window.getSelection) { //all others
	        selection = window.getSelection();        
	        range = document.createRange();
	        range.selectNodeContents(element);
	        selection.removeAllRanges();
	        selection.addRange(range);
	    }
	}
	jQuery('.gal-admin-scopes-list').on('click', function(e) {
		selectText(e.target);
	});

	// Drip signup form in basic version
	jQuery('.gal-drip-signup-button').on('click', function(e) {
        jQuery('#gal-drip-signup-form').submit(function() {
            jQuery('#gal-drip-signup-form div').hide().after(jQuery('<div><p>Thank you!</p></div>'));
            // Mark WP user as signed up
            jQuery.post(ajaxurl, {action: 'gal_drip_submitted'}, function(response) {

            });
        });
	});
}); 
