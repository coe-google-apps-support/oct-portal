<?php
/*
 * Copyright 2011 Google Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
* 
* Updated by Dan Lester 2014
* 
*/

require_once realpath(dirname(__FILE__) . '/../../../autoload.php');

/**
 * Signs data with PEM key (e.g. taken from new JSON files).
 *
 * @author Dan Lester <dan@danlester.com>
 */
class GoogleGAL_Signer_PEM extends GoogleGAL_Signer_Abstract
{
	// OpenSSL private key resource
	private $privateKey;

	// Creates a new signer from a PEM text string
	public function __construct($pem, $password)
	{
		if (!function_exists('openssl_x509_read')) {
			throw new GoogleGAL_Exception(
					'The Google PHP API library needs the openssl PHP extension'
			);
		}

		$this->privateKey = $pem;
		if (!$this->privateKey) {
			throw new GoogleGAL_Auth_Exception("Unable to load private key");
		}
	}

	public function __destruct()
	{
	}

	public function sign($data)
	{
		if (version_compare(PHP_VERSION, '5.3.0') < 0) {
			throw new GoogleGAL_Auth_Exception(
					"PHP 5.3.0 or higher is required to use service accounts."
			);
		}
		$hash = defined("OPENSSL_ALGO_SHA256") ? OPENSSL_ALGO_SHA256 : "sha256";
		if (!openssl_sign($data, $signature, $this->privateKey, $hash)) {
			throw new GoogleGAL_Auth_Exception("Unable to sign data");
		}
		return $signature;
	}
}
