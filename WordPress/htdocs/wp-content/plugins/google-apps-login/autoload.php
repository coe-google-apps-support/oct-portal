<?php
/*
 * Copyright 2014 Google Inc.
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
 */

function gal_google_api_php_client_autoload($className) {
  $classPath = explode('_', $className);
  if ($classPath[0] != 'GoogleGAL') { // Was Google
    return;
  }
  if (count($classPath) > 3) {
    // Maximum class file path depth in this project is 3.
    $classPath = array_slice($classPath, 0, 3);
  }
  $classPath = str_replace('GoogleGAL', 'Google', $classPath); // Adjust back to Google's path
  $filePath = dirname(__FILE__) . '/core/' . implode('/', $classPath) . '.php'; // was src -> now core
  if (file_exists($filePath)) {
    require_once($filePath);
  }
}

spl_autoload_register('gal_google_api_php_client_autoload');
