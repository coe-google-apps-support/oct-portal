<?php

class gal_keyfile_uploader {
	
	protected $fileindex = '';
	protected $jsontext = '';
	// JSON could have been submitted by a file or by text
	public function __construct($fileindex, $jsontext) {
		$this->fileindex = $fileindex;
		$this->jsontext = $jsontext;
		$this->attempt_upload();
	}
	
	protected $contents = null;
	protected function attempt_upload() {
		// If there was an attempt to upload a file
		if (isset($_FILES[$this->fileindex]) 
					&& (!isset($_FILES[$this->fileindex]['error']) || $_FILES[$this->fileindex]['error'] != 4)) {
					// error 4 = no file chosen anyway
			
			if (isset($_FILES[$this->fileindex]['error']) && $_FILES[$this->fileindex]['error'] != 0) {
				error_log('JSON Key file upload error number '.$_FILES[$this->fileindex]['error']);
				// Some import errors have error explanations
				$this->error = 'file_upload_error'.(in_array($_FILES[$this->fileindex]['error'], array(2,6,7)) ? $_FILES[$this->fileindex]['error'] : '');
				return;
			}

			if (isset($_FILES[$this->fileindex]['size']) && $_FILES[$this->fileindex]['size'] <= 0) {
				$this->error = 'no_content';
				return;
			}
			
			$filepath = $_FILES[$this->fileindex]['tmp_name'];
			$this->contents = @file_get_contents($filepath);
		}
		else if (strlen(trim($this->jsontext)) > 0) {
			$this->contents = strpos($this->jsontext, '\\\\') !== false ? stripslashes($this->jsontext) : $this->jsontext;
		}
		if ($this->contents !== null) {
			$this->read_json();
		}
	}
	
	protected function read_json() {
		$fullkey = json_decode($this->contents, TRUE);
		if ($fullkey === null || !is_array($fullkey)) {
			$this->error = 'decode_error';
			return;
		}
		if (!isset($fullkey['client_id']) || !isset($fullkey['client_email']) || !isset($fullkey['private_key']) || !isset($fullkey['type'])
			|| $fullkey['client_id'] == '' || $fullkey['client_email'] == '' || $fullkey['private_key'] == '') {
			$this->error = 'missing_values';
			return;
		}
		if (isset($fullkey['type']) && $fullkey['type'] != 'service_account') {
			$this->error = 'not_serviceacct';
			return;
		}
		
		if (!$this->test_key($fullkey['private_key'])) {
			$this->error = 'bad_pem';
			return;
		}
		
		$this->key = $fullkey['private_key'];
		$this->id = $fullkey['client_id'];
		$this->email = $fullkey['client_email'];
		$this->pkeyprint = isset($fullkey['private_key_id']) ? $fullkey['private_key_id'] : '<unspecified>';
	}
	
	protected function test_key($pemkey) {
		$hash = defined("OPENSSL_ALGO_SHA256") ? OPENSSL_ALGO_SHA256 : "sha256";
		$data = 'test data';
		$signature = '';
		if (!@openssl_sign($data, $signature, $pemkey, $hash)) {
			return false;
		}
		return $signature != '' ? true : false;
	}
	
	protected $email = '';
	public function getEmail() {
		return $this->email;
	}

	protected $id = '';
	public function getId() {
		return $this->id;
	}

	protected $key = '';
	public function getKey() {
		return $this->key;
	}
	
	protected $pkeyprint = '';
	public function getPrint() {
		return $this->pkeyprint;
	}
	
	protected $error = '';
	public function getError() {
		return $this->error;
	}
}