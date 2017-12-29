<?php

function gal_service_account_upgrade(&$option, $gal_option_name, &$existing_sa_options, $gal_sa_option_name) {
	/* Convert ga_serviceemail ga_keyfilepath
	* into new separate sa options:
	* ga_sakey, ga_serviceemail, ga_pkey_print
	*/
	
	if (count($existing_sa_options)) {
		return;
	}
	
	$existing_sa_options = array('ga_serviceemail' => isset($option['ga_serviceemail']) ? $option['ga_serviceemail'] : '',
						'ga_sakey' => '',
						'ga_pkey_print' => '<unspecified>');
	
	try {
		if (version_compare(PHP_VERSION, '5.3.0') >= 0 && function_exists('openssl_x509_read')) {
			if (isset($option['ga_keyfilepath']) && $option['ga_keyfilepath'] != '' && file_exists($option['ga_keyfilepath'])) {
				$p12key = @file_get_contents($option['ga_keyfilepath']);
				
				$certs = array();
				if (openssl_pkcs12_read($p12key, $certs, 'notasecret')) {
					if (array_key_exists("pkey", $certs) && $certs["pkey"]) {
						$privateKey = openssl_pkey_get_private($certs['pkey']);
						$pemString = '';
						if (openssl_pkey_export($privateKey, $pemString)) {
							$existing_sa_options['ga_sakey'] = $pemString;
						}
						openssl_pkey_free($privateKey);
						
						@unlink($options['ga_keyfilepath']);
					}	
				}
			}
		}
	}
	catch (Exception $e) {
		// Never mind
	}
	
	// Remove redundant parts of regular options
	unset($option['ga_serviceemail']);
	unset($option['ga_keyfilepath']);
}
