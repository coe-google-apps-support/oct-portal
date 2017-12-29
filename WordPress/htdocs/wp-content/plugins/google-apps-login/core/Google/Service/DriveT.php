<?php

/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * Service definition for Drive (v3).
 *
 * <p>
 * Manages files in Drive including uploading, downloading, searching, detecting
 * changes, and updating sharing permissions.</p>
 *
 * <p>
 * For more information about this service, see the API
 * <a href="https://developers.google.com/drive/" target="_blank">Documentation</a>
 * </p>
 *
 * @author Google, Inc.
 */
class GoogleGAL_Service_DriveT extends GoogleGAL_Service
{
  /** View and manage the files in your Google Drive. */
  const DRIVE =
      "https://www.googleapis.com/auth/drive";
  /** View and manage its own configuration data in your Google Drive. */
  const DRIVE_APPDATA =
      "https://www.googleapis.com/auth/drive.appdata";
  /** View and manage Google Drive files and folders that you have opened or created with this app. */
  const DRIVE_FILE =
      "https://www.googleapis.com/auth/drive.file";
  /** View and manage metadata of files in your Google Drive. */
  const DRIVE_METADATA =
      "https://www.googleapis.com/auth/drive.metadata";
  /** View metadata for files in your Google Drive. */
  const DRIVE_METADATA_READONLY =
      "https://www.googleapis.com/auth/drive.metadata.readonly";
  /** View the photos, videos and albums in your Google Photos. */
  const DRIVE_PHOTOS_READONLY =
      "https://www.googleapis.com/auth/drive.photos.readonly";
  /** View the files in your Google Drive. */
  const DRIVE_READONLY =
      "https://www.googleapis.com/auth/drive.readonly";
  /** Modify your Google Apps Script scripts' behavior. */
  const DRIVE_SCRIPTS =
      "https://www.googleapis.com/auth/drive.scripts";

  public $about;
  public $changes;
  public $channels;
  public $comments;
  public $files;
  public $permissions;
  public $replies;
  public $revisions;
  public $teamdrives;
  
  /**
   * Constructs the internal representation of the Drive service.
   *
   * @param GoogleGAL_Client $client
   */
  public function __construct(GoogleGAL_Client $client)
  {
    parent::__construct($client);
    $this->rootUrl = 'https://www.googleapis.com/';
    $this->servicePath = 'drive/v3/';
    $this->version = 'v3';
    $this->serviceName = 'drive';

    $this->about = new GoogleGAL_Service_DriveT_Resource_About(
        $this,
        $this->serviceName,
        'about',
        array(
          'methods' => array(
            'get' => array(
              'path' => 'about',
              'httpMethod' => 'GET',
              'parameters' => array(),
            ),
          )
        )
    );
    $this->changes = new GoogleGAL_Service_DriveT_Resource_Changes(
        $this,
        $this->serviceName,
        'changes',
        array(
          'methods' => array(
            'getStartPageToken' => array(
              'path' => 'changes/startPageToken',
              'httpMethod' => 'GET',
              'parameters' => array(
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'teamDriveId' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
              ),
            ),'list' => array(
              'path' => 'changes',
              'httpMethod' => 'GET',
              'parameters' => array(
                'pageToken' => array(
                  'location' => 'query',
                  'type' => 'string',
                  'required' => true,
                ),
                'includeCorpusRemovals' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'includeRemoved' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'includeTeamDriveItems' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'pageSize' => array(
                  'location' => 'query',
                  'type' => 'integer',
                ),
                'restrictToMyDrive' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'spaces' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'teamDriveId' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
              ),
            ),'watch' => array(
              'path' => 'changes/watch',
              'httpMethod' => 'POST',
              'parameters' => array(
                'pageToken' => array(
                  'location' => 'query',
                  'type' => 'string',
                  'required' => true,
                ),
                'includeCorpusRemovals' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'includeRemoved' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'includeTeamDriveItems' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'pageSize' => array(
                  'location' => 'query',
                  'type' => 'integer',
                ),
                'restrictToMyDrive' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'spaces' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'teamDriveId' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
              ),
            ),
          )
        )
    );
    $this->channels = new GoogleGAL_Service_DriveT_Resource_Channels(
        $this,
        $this->serviceName,
        'channels',
        array(
          'methods' => array(
            'stop' => array(
              'path' => 'channels/stop',
              'httpMethod' => 'POST',
              'parameters' => array(),
            ),
          )
        )
    );
    $this->comments = new GoogleGAL_Service_DriveT_Resource_Comments(
        $this,
        $this->serviceName,
        'comments',
        array(
          'methods' => array(
            'create' => array(
              'path' => 'files/{fileId}/comments',
              'httpMethod' => 'POST',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),'delete' => array(
              'path' => 'files/{fileId}/comments/{commentId}',
              'httpMethod' => 'DELETE',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'commentId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),'get' => array(
              'path' => 'files/{fileId}/comments/{commentId}',
              'httpMethod' => 'GET',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'commentId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'includeDeleted' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'list' => array(
              'path' => 'files/{fileId}/comments',
              'httpMethod' => 'GET',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'includeDeleted' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'pageSize' => array(
                  'location' => 'query',
                  'type' => 'integer',
                ),
                'pageToken' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'startModifiedTime' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
              ),
            ),'update' => array(
              'path' => 'files/{fileId}/comments/{commentId}',
              'httpMethod' => 'PATCH',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'commentId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),
          )
        )
    );
    $this->files = new GoogleGAL_Service_DriveT_Resource_Files(
        $this,
        $this->serviceName,
        'files',
        array(
          'methods' => array(
            'copy' => array(
              'path' => 'files/{fileId}/copy',
              'httpMethod' => 'POST',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'ignoreDefaultVisibility' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'keepRevisionForever' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'ocrLanguage' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'create' => array(
              'path' => 'files',
              'httpMethod' => 'POST',
              'parameters' => array(
                'ignoreDefaultVisibility' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'keepRevisionForever' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'ocrLanguage' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'useContentAsIndexableText' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'delete' => array(
              'path' => 'files/{fileId}',
              'httpMethod' => 'DELETE',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'emptyTrash' => array(
              'path' => 'files/trash',
              'httpMethod' => 'DELETE',
              'parameters' => array(),
            ),'export' => array(
              'path' => 'files/{fileId}/export',
              'httpMethod' => 'GET',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'mimeType' => array(
                  'location' => 'query',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),'generateIds' => array(
              'path' => 'files/generateIds',
              'httpMethod' => 'GET',
              'parameters' => array(
                'count' => array(
                  'location' => 'query',
                  'type' => 'integer',
                ),
                'space' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
              ),
            ),'get' => array(
              'path' => 'files/{fileId}',
              'httpMethod' => 'GET',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'acknowledgeAbuse' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'list' => array(
              'path' => 'files',
              'httpMethod' => 'GET',
              'parameters' => array(
                'corpora' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'corpus' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'includeTeamDriveItems' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'orderBy' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'pageSize' => array(
                  'location' => 'query',
                  'type' => 'integer',
                ),
                'pageToken' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'q' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'spaces' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'teamDriveId' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
              ),
            ),'update' => array(
              'path' => 'files/{fileId}',
              'httpMethod' => 'PATCH',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'addParents' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'keepRevisionForever' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'ocrLanguage' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'removeParents' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'useContentAsIndexableText' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'watch' => array(
              'path' => 'files/{fileId}/watch',
              'httpMethod' => 'POST',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'acknowledgeAbuse' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),
          )
        )
    );
    $this->permissions = new GoogleGAL_Service_DriveT_Resource_Permissions(
        $this,
        $this->serviceName,
        'permissions',
        array(
          'methods' => array(
            'create' => array(
              'path' => 'files/{fileId}/permissions',
              'httpMethod' => 'POST',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'emailMessage' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'sendNotificationEmail' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'transferOwnership' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'delete' => array(
              'path' => 'files/{fileId}/permissions/{permissionId}',
              'httpMethod' => 'DELETE',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'permissionId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'get' => array(
              'path' => 'files/{fileId}/permissions/{permissionId}',
              'httpMethod' => 'GET',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'permissionId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'list' => array(
              'path' => 'files/{fileId}/permissions',
              'httpMethod' => 'GET',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'pageSize' => array(
                  'location' => 'query',
                  'type' => 'integer',
                ),
                'pageToken' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'update' => array(
              'path' => 'files/{fileId}/permissions/{permissionId}',
              'httpMethod' => 'PATCH',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'permissionId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'removeExpiration' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'supportsTeamDrives' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'transferOwnership' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),
          )
        )
    );
    $this->replies = new GoogleGAL_Service_DriveT_Resource_Replies(
        $this,
        $this->serviceName,
        'replies',
        array(
          'methods' => array(
            'create' => array(
              'path' => 'files/{fileId}/comments/{commentId}/replies',
              'httpMethod' => 'POST',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'commentId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),'delete' => array(
              'path' => 'files/{fileId}/comments/{commentId}/replies/{replyId}',
              'httpMethod' => 'DELETE',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'commentId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'replyId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),'get' => array(
              'path' => 'files/{fileId}/comments/{commentId}/replies/{replyId}',
              'httpMethod' => 'GET',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'commentId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'replyId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'includeDeleted' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'list' => array(
              'path' => 'files/{fileId}/comments/{commentId}/replies',
              'httpMethod' => 'GET',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'commentId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'includeDeleted' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
                'pageSize' => array(
                  'location' => 'query',
                  'type' => 'integer',
                ),
                'pageToken' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
              ),
            ),'update' => array(
              'path' => 'files/{fileId}/comments/{commentId}/replies/{replyId}',
              'httpMethod' => 'PATCH',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'commentId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'replyId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),
          )
        )
    );
    $this->revisions = new GoogleGAL_Service_DriveT_Resource_Revisions(
        $this,
        $this->serviceName,
        'revisions',
        array(
          'methods' => array(
            'delete' => array(
              'path' => 'files/{fileId}/revisions/{revisionId}',
              'httpMethod' => 'DELETE',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'revisionId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),'get' => array(
              'path' => 'files/{fileId}/revisions/{revisionId}',
              'httpMethod' => 'GET',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'revisionId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'acknowledgeAbuse' => array(
                  'location' => 'query',
                  'type' => 'boolean',
                ),
              ),
            ),'list' => array(
              'path' => 'files/{fileId}/revisions',
              'httpMethod' => 'GET',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'pageSize' => array(
                  'location' => 'query',
                  'type' => 'integer',
                ),
                'pageToken' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
              ),
            ),'update' => array(
              'path' => 'files/{fileId}/revisions/{revisionId}',
              'httpMethod' => 'PATCH',
              'parameters' => array(
                'fileId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
                'revisionId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),
          )
        )
    );
    $this->teamdrives = new GoogleGAL_Service_DriveT_Resource_Teamdrives(
        $this,
        $this->serviceName,
        'teamdrives',
        array(
          'methods' => array(
            'create' => array(
              'path' => 'teamdrives',
              'httpMethod' => 'POST',
              'parameters' => array(
                'requestId' => array(
                  'location' => 'query',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),'delete' => array(
              'path' => 'teamdrives/{teamDriveId}',
              'httpMethod' => 'DELETE',
              'parameters' => array(
                'teamDriveId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),'get' => array(
              'path' => 'teamdrives/{teamDriveId}',
              'httpMethod' => 'GET',
              'parameters' => array(
                'teamDriveId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),'list' => array(
              'path' => 'teamdrives',
              'httpMethod' => 'GET',
              'parameters' => array(
                'pageSize' => array(
                  'location' => 'query',
                  'type' => 'integer',
                ),
                'pageToken' => array(
                  'location' => 'query',
                  'type' => 'string',
                ),
              ),
            ),'update' => array(
              'path' => 'teamdrives/{teamDriveId}',
              'httpMethod' => 'PATCH',
              'parameters' => array(
                'teamDriveId' => array(
                  'location' => 'path',
                  'type' => 'string',
                  'required' => true,
                ),
              ),
            ),
          )
        )
    );
  }
}

  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_About extends GoogleGAL_Collection
{
	protected $collection_key = 'folderColorPalette';
	public $appInstalled;
	public $exportFormats;
	public $folderColorPalette;
	public $importFormats;
	public $kind;
	public $maxImportSizes;
	public $maxUploadSize;
	protected $storageQuotaType = 'GoogleGAL_Service_DriveT_AboutStorageQuota';
	protected $storageQuotaDataType = '';
	protected $userType = 'GoogleGAL_Service_DriveT_User';
	protected $userDataType = '';

	public function setAppInstalled($appInstalled)
	{
		$this->appInstalled = $appInstalled;
	}
	public function getAppInstalled()
	{
		return $this->appInstalled;
	}
	public function setExportFormats($exportFormats)
	{
		$this->exportFormats = $exportFormats;
	}
	public function getExportFormats()
	{
		return $this->exportFormats;
	}
	public function setFolderColorPalette($folderColorPalette)
	{
		$this->folderColorPalette = $folderColorPalette;
	}
	public function getFolderColorPalette()
	{
		return $this->folderColorPalette;
	}
	public function setImportFormats($importFormats)
	{
		$this->importFormats = $importFormats;
	}
	public function getImportFormats()
	{
		return $this->importFormats;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setMaxImportSizes($maxImportSizes)
	{
		$this->maxImportSizes = $maxImportSizes;
	}
	public function getMaxImportSizes()
	{
		return $this->maxImportSizes;
	}
	public function setMaxUploadSize($maxUploadSize)
	{
		$this->maxUploadSize = $maxUploadSize;
	}
	public function getMaxUploadSize()
	{
		return $this->maxUploadSize;
	}
	public function setStorageQuota(GoogleGAL_Service_DriveT_AboutStorageQuota $storageQuota)
	{
		$this->storageQuota = $storageQuota;
	}
	public function getStorageQuota()
	{
		return $this->storageQuota;
	}
	public function setUser(GoogleGAL_Service_DriveT_User $user)
	{
		$this->user = $user;
	}
	public function getUser()
	{
		return $this->user;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_AboutStorageQuota extends GoogleGAL_Model
{
	public $limit;
	public $usage;
	public $usageInDrive;
	public $usageInDriveTrash;

	public function setLimit($limit)
	{
		$this->limit = $limit;
	}
	public function getLimit()
	{
		return $this->limit;
	}
	public function setUsage($usage)
	{
		$this->usage = $usage;
	}
	public function getUsage()
	{
		return $this->usage;
	}
	public function setUsageInDrive($usageInDrive)
	{
		$this->usageInDrive = $usageInDrive;
	}
	public function getUsageInDrive()
	{
		return $this->usageInDrive;
	}
	public function setUsageInDriveTrash($usageInDriveTrash)
	{
		$this->usageInDriveTrash = $usageInDriveTrash;
	}
	public function getUsageInDriveTrash()
	{
		return $this->usageInDriveTrash;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_Change extends GoogleGAL_Model
{
	protected $fileType = 'GoogleGAL_Service_DriveT_DriveFile';
	protected $fileDataType = '';
	public $fileId;
	public $kind;
	public $removed;
	protected $teamDriveType = 'GoogleGAL_Service_DriveT_TeamDrive';
	protected $teamDriveDataType = '';
	public $teamDriveId;
	public $time;
	public $type;

	public function setFile(GoogleGAL_Service_DriveT_DriveFile $file)
	{
		$this->file = $file;
	}
	public function getFile()
	{
		return $this->file;
	}
	public function setFileId($fileId)
	{
		$this->fileId = $fileId;
	}
	public function getFileId()
	{
		return $this->fileId;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setRemoved($removed)
	{
		$this->removed = $removed;
	}
	public function getRemoved()
	{
		return $this->removed;
	}
	public function setTeamDrive(GoogleGAL_Service_DriveT_TeamDrive $teamDrive)
	{
		$this->teamDrive = $teamDrive;
	}
	public function getTeamDrive()
	{
		return $this->teamDrive;
	}
	public function setTeamDriveId($teamDriveId)
	{
		$this->teamDriveId = $teamDriveId;
	}
	public function getTeamDriveId()
	{
		return $this->teamDriveId;
	}
	public function setTime($time)
	{
		$this->time = $time;
	}
	public function getTime()
	{
		return $this->time;
	}
	public function setType($type)
	{
		$this->type = $type;
	}
	public function getType()
	{
		return $this->type;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_ChangeList extends GoogleGAL_Collection
{
	protected $collection_key = 'changes';
	protected $changesType = 'GoogleGAL_Service_DriveT_Change';
	protected $changesDataType = 'array';
	public $kind;
	public $newStartPageToken;
	public $nextPageToken;

	public function setChanges($changes)
	{
		$this->changes = $changes;
	}
	public function getChanges()
	{
		return $this->changes;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setNewStartPageToken($newStartPageToken)
	{
		$this->newStartPageToken = $newStartPageToken;
	}
	public function getNewStartPageToken()
	{
		return $this->newStartPageToken;
	}
	public function setNextPageToken($nextPageToken)
	{
		$this->nextPageToken = $nextPageToken;
	}
	public function getNextPageToken()
	{
		return $this->nextPageToken;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_Channel extends GoogleGAL_Model
{
	public $address;
	public $expiration;
	public $id;
	public $kind;
	public $params;
	public $payload;
	public $resourceId;
	public $resourceUri;
	public $token;
	public $type;

	public function setAddress($address)
	{
		$this->address = $address;
	}
	public function getAddress()
	{
		return $this->address;
	}
	public function setExpiration($expiration)
	{
		$this->expiration = $expiration;
	}
	public function getExpiration()
	{
		return $this->expiration;
	}
	public function setId($id)
	{
		$this->id = $id;
	}
	public function getId()
	{
		return $this->id;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setParams($params)
	{
		$this->params = $params;
	}
	public function getParams()
	{
		return $this->params;
	}
	public function setPayload($payload)
	{
		$this->payload = $payload;
	}
	public function getPayload()
	{
		return $this->payload;
	}
	public function setResourceId($resourceId)
	{
		$this->resourceId = $resourceId;
	}
	public function getResourceId()
	{
		return $this->resourceId;
	}
	public function setResourceUri($resourceUri)
	{
		$this->resourceUri = $resourceUri;
	}
	public function getResourceUri()
	{
		return $this->resourceUri;
	}
	public function setToken($token)
	{
		$this->token = $token;
	}
	public function getToken()
	{
		return $this->token;
	}
	public function setType($type)
	{
		$this->type = $type;
	}
	public function getType()
	{
		return $this->type;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_Comment extends GoogleGAL_Collection
{
	protected $collection_key = 'replies';
	public $anchor;
	protected $authorType = 'GoogleGAL_Service_DriveT_User';
	protected $authorDataType = '';
	public $content;
	public $createdTime;
	public $deleted;
	public $htmlContent;
	public $id;
	public $kind;
	public $modifiedTime;
	protected $quotedFileContentType = 'GoogleGAL_Service_DriveT_CommentQuotedFileContent';
	protected $quotedFileContentDataType = '';
	protected $repliesType = 'GoogleGAL_Service_DriveT_Reply';
	protected $repliesDataType = 'array';
	public $resolved;

	public function setAnchor($anchor)
	{
		$this->anchor = $anchor;
	}
	public function getAnchor()
	{
		return $this->anchor;
	}
	public function setAuthor(GoogleGAL_Service_DriveT_User $author)
	{
		$this->author = $author;
	}
	public function getAuthor()
	{
		return $this->author;
	}
	public function setContent($content)
	{
		$this->content = $content;
	}
	public function getContent()
	{
		return $this->content;
	}
	public function setCreatedTime($createdTime)
	{
		$this->createdTime = $createdTime;
	}
	public function getCreatedTime()
	{
		return $this->createdTime;
	}
	public function setDeleted($deleted)
	{
		$this->deleted = $deleted;
	}
	public function getDeleted()
	{
		return $this->deleted;
	}
	public function setHtmlContent($htmlContent)
	{
		$this->htmlContent = $htmlContent;
	}
	public function getHtmlContent()
	{
		return $this->htmlContent;
	}
	public function setId($id)
	{
		$this->id = $id;
	}
	public function getId()
	{
		return $this->id;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setModifiedTime($modifiedTime)
	{
		$this->modifiedTime = $modifiedTime;
	}
	public function getModifiedTime()
	{
		return $this->modifiedTime;
	}
	public function setQuotedFileContent(GoogleGAL_Service_DriveT_CommentQuotedFileContent $quotedFileContent)
	{
		$this->quotedFileContent = $quotedFileContent;
	}
	public function getQuotedFileContent()
	{
		return $this->quotedFileContent;
	}
	public function setReplies($replies)
	{
		$this->replies = $replies;
	}
	public function getReplies()
	{
		return $this->replies;
	}
	public function setResolved($resolved)
	{
		$this->resolved = $resolved;
	}
	public function getResolved()
	{
		return $this->resolved;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_CommentList extends GoogleGAL_Collection
{
	protected $collection_key = 'comments';
	protected $commentsType = 'GoogleGAL_Service_DriveT_Comment';
	protected $commentsDataType = 'array';
	public $kind;
	public $nextPageToken;

	public function setComments($comments)
	{
		$this->comments = $comments;
	}
	public function getComments()
	{
		return $this->comments;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setNextPageToken($nextPageToken)
	{
		$this->nextPageToken = $nextPageToken;
	}
	public function getNextPageToken()
	{
		return $this->nextPageToken;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_CommentQuotedFileContent extends GoogleGAL_Model
{
	public $mimeType;
	public $value;

	public function setMimeType($mimeType)
	{
		$this->mimeType = $mimeType;
	}
	public function getMimeType()
	{
		return $this->mimeType;
	}
	public function setValue($value)
	{
		$this->value = $value;
	}
	public function getValue()
	{
		return $this->value;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_DriveFile extends GoogleGAL_Collection
{
	protected $collection_key = 'spaces';
	public $appProperties;
	protected $capabilitiesType = 'GoogleGAL_Service_DriveT_DriveFileCapabilities';
	protected $capabilitiesDataType = '';
	protected $contentHintsType = 'GoogleGAL_Service_DriveT_DriveFileContentHints';
	protected $contentHintsDataType = '';
	public $createdTime;
	public $description;
	public $explicitlyTrashed;
	public $fileExtension;
	public $folderColorRgb;
	public $fullFileExtension;
	public $hasAugmentedPermissions;
	public $hasThumbnail;
	public $headRevisionId;
	public $iconLink;
	public $id;
	protected $imageMediaMetadataType = 'GoogleGAL_Service_DriveT_DriveFileImageMediaMetadata';
	protected $imageMediaMetadataDataType = '';
	public $isAppAuthorized;
	public $kind;
	protected $lastModifyingUserType = 'GoogleGAL_Service_DriveT_User';
	protected $lastModifyingUserDataType = '';
	public $md5Checksum;
	public $mimeType;
	public $modifiedByMe;
	public $modifiedByMeTime;
	public $modifiedTime;
	public $name;
	public $originalFilename;
	public $ownedByMe;
	protected $ownersType = 'GoogleGAL_Service_DriveT_User';
	protected $ownersDataType = 'array';
	public $parents;
	protected $permissionsType = 'GoogleGAL_Service_DriveT_Permission';
	protected $permissionsDataType = 'array';
	public $properties;
	public $quotaBytesUsed;
	public $shared;
	public $sharedWithMeTime;
	protected $sharingUserType = 'GoogleGAL_Service_DriveT_User';
	protected $sharingUserDataType = '';
	public $size;
	public $spaces;
	public $starred;
	public $teamDriveId;
	public $thumbnailLink;
	public $thumbnailVersion;
	public $trashed;
	public $trashedTime;
	protected $trashingUserType = 'GoogleGAL_Service_DriveT_User';
	protected $trashingUserDataType = '';
	public $version;
	protected $videoMediaMetadataType = 'GoogleGAL_Service_DriveT_DriveFileVideoMediaMetadata';
	protected $videoMediaMetadataDataType = '';
	public $viewedByMe;
	public $viewedByMeTime;
	public $viewersCanCopyContent;
	public $webContentLink;
	public $webViewLink;
	public $writersCanShare;

	public function setAppProperties($appProperties)
	{
		$this->appProperties = $appProperties;
	}
	public function getAppProperties()
	{
		return $this->appProperties;
	}
	public function setCapabilities(GoogleGAL_Service_DriveT_DriveFileCapabilities $capabilities)
	{
		$this->capabilities = $capabilities;
	}
	public function getCapabilities()
	{
		return $this->capabilities;
	}
	public function setContentHints(GoogleGAL_Service_DriveT_DriveFileContentHints $contentHints)
	{
		$this->contentHints = $contentHints;
	}
	public function getContentHints()
	{
		return $this->contentHints;
	}
	public function setCreatedTime($createdTime)
	{
		$this->createdTime = $createdTime;
	}
	public function getCreatedTime()
	{
		return $this->createdTime;
	}
	public function setDescription($description)
	{
		$this->description = $description;
	}
	public function getDescription()
	{
		return $this->description;
	}
	public function setExplicitlyTrashed($explicitlyTrashed)
	{
		$this->explicitlyTrashed = $explicitlyTrashed;
	}
	public function getExplicitlyTrashed()
	{
		return $this->explicitlyTrashed;
	}
	public function setFileExtension($fileExtension)
	{
		$this->fileExtension = $fileExtension;
	}
	public function getFileExtension()
	{
		return $this->fileExtension;
	}
	public function setFolderColorRgb($folderColorRgb)
	{
		$this->folderColorRgb = $folderColorRgb;
	}
	public function getFolderColorRgb()
	{
		return $this->folderColorRgb;
	}
	public function setFullFileExtension($fullFileExtension)
	{
		$this->fullFileExtension = $fullFileExtension;
	}
	public function getFullFileExtension()
	{
		return $this->fullFileExtension;
	}
	public function setHasAugmentedPermissions($hasAugmentedPermissions)
	{
		$this->hasAugmentedPermissions = $hasAugmentedPermissions;
	}
	public function getHasAugmentedPermissions()
	{
		return $this->hasAugmentedPermissions;
	}
	public function setHasThumbnail($hasThumbnail)
	{
		$this->hasThumbnail = $hasThumbnail;
	}
	public function getHasThumbnail()
	{
		return $this->hasThumbnail;
	}
	public function setHeadRevisionId($headRevisionId)
	{
		$this->headRevisionId = $headRevisionId;
	}
	public function getHeadRevisionId()
	{
		return $this->headRevisionId;
	}
	public function setIconLink($iconLink)
	{
		$this->iconLink = $iconLink;
	}
	public function getIconLink()
	{
		return $this->iconLink;
	}
	public function setId($id)
	{
		$this->id = $id;
	}
	public function getId()
	{
		return $this->id;
	}
	public function setImageMediaMetadata(GoogleGAL_Service_DriveT_DriveFileImageMediaMetadata $imageMediaMetadata)
	{
		$this->imageMediaMetadata = $imageMediaMetadata;
	}
	public function getImageMediaMetadata()
	{
		return $this->imageMediaMetadata;
	}
	public function setIsAppAuthorized($isAppAuthorized)
	{
		$this->isAppAuthorized = $isAppAuthorized;
	}
	public function getIsAppAuthorized()
	{
		return $this->isAppAuthorized;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setLastModifyingUser(GoogleGAL_Service_DriveT_User $lastModifyingUser)
	{
		$this->lastModifyingUser = $lastModifyingUser;
	}
	public function getLastModifyingUser()
	{
		return $this->lastModifyingUser;
	}
	public function setMd5Checksum($md5Checksum)
	{
		$this->md5Checksum = $md5Checksum;
	}
	public function getMd5Checksum()
	{
		return $this->md5Checksum;
	}
	public function setMimeType($mimeType)
	{
		$this->mimeType = $mimeType;
	}
	public function getMimeType()
	{
		return $this->mimeType;
	}
	public function setModifiedByMe($modifiedByMe)
	{
		$this->modifiedByMe = $modifiedByMe;
	}
	public function getModifiedByMe()
	{
		return $this->modifiedByMe;
	}
	public function setModifiedByMeTime($modifiedByMeTime)
	{
		$this->modifiedByMeTime = $modifiedByMeTime;
	}
	public function getModifiedByMeTime()
	{
		return $this->modifiedByMeTime;
	}
	public function setModifiedTime($modifiedTime)
	{
		$this->modifiedTime = $modifiedTime;
	}
	public function getModifiedTime()
	{
		return $this->modifiedTime;
	}
	public function setName($name)
	{
		$this->name = $name;
	}
	public function getName()
	{
		return $this->name;
	}
	public function setOriginalFilename($originalFilename)
	{
		$this->originalFilename = $originalFilename;
	}
	public function getOriginalFilename()
	{
		return $this->originalFilename;
	}
	public function setOwnedByMe($ownedByMe)
	{
		$this->ownedByMe = $ownedByMe;
	}
	public function getOwnedByMe()
	{
		return $this->ownedByMe;
	}
	public function setOwners($owners)
	{
		$this->owners = $owners;
	}
	public function getOwners()
	{
		return $this->owners;
	}
	public function setParents($parents)
	{
		$this->parents = $parents;
	}
	public function getParents()
	{
		return $this->parents;
	}
	public function setPermissions($permissions)
	{
		$this->permissions = $permissions;
	}
	public function getPermissions()
	{
		return $this->permissions;
	}
	public function setProperties($properties)
	{
		$this->properties = $properties;
	}
	public function getProperties()
	{
		return $this->properties;
	}
	public function setQuotaBytesUsed($quotaBytesUsed)
	{
		$this->quotaBytesUsed = $quotaBytesUsed;
	}
	public function getQuotaBytesUsed()
	{
		return $this->quotaBytesUsed;
	}
	public function setShared($shared)
	{
		$this->shared = $shared;
	}
	public function getShared()
	{
		return $this->shared;
	}
	public function setSharedWithMeTime($sharedWithMeTime)
	{
		$this->sharedWithMeTime = $sharedWithMeTime;
	}
	public function getSharedWithMeTime()
	{
		return $this->sharedWithMeTime;
	}
	public function setSharingUser(GoogleGAL_Service_DriveT_User $sharingUser)
	{
		$this->sharingUser = $sharingUser;
	}
	public function getSharingUser()
	{
		return $this->sharingUser;
	}
	public function setSize($size)
	{
		$this->size = $size;
	}
	public function getSize()
	{
		return $this->size;
	}
	public function setSpaces($spaces)
	{
		$this->spaces = $spaces;
	}
	public function getSpaces()
	{
		return $this->spaces;
	}
	public function setStarred($starred)
	{
		$this->starred = $starred;
	}
	public function getStarred()
	{
		return $this->starred;
	}
	public function setTeamDriveId($teamDriveId)
	{
		$this->teamDriveId = $teamDriveId;
	}
	public function getTeamDriveId()
	{
		return $this->teamDriveId;
	}
	public function setThumbnailLink($thumbnailLink)
	{
		$this->thumbnailLink = $thumbnailLink;
	}
	public function getThumbnailLink()
	{
		return $this->thumbnailLink;
	}
	public function setThumbnailVersion($thumbnailVersion)
	{
		$this->thumbnailVersion = $thumbnailVersion;
	}
	public function getThumbnailVersion()
	{
		return $this->thumbnailVersion;
	}
	public function setTrashed($trashed)
	{
		$this->trashed = $trashed;
	}
	public function getTrashed()
	{
		return $this->trashed;
	}
	public function setTrashedTime($trashedTime)
	{
		$this->trashedTime = $trashedTime;
	}
	public function getTrashedTime()
	{
		return $this->trashedTime;
	}
	public function setTrashingUser(GoogleGAL_Service_DriveT_User $trashingUser)
	{
		$this->trashingUser = $trashingUser;
	}
	public function getTrashingUser()
	{
		return $this->trashingUser;
	}
	public function setVersion($version)
	{
		$this->version = $version;
	}
	public function getVersion()
	{
		return $this->version;
	}
	public function setVideoMediaMetadata(GoogleGAL_Service_DriveT_DriveFileVideoMediaMetadata $videoMediaMetadata)
	{
		$this->videoMediaMetadata = $videoMediaMetadata;
	}
	public function getVideoMediaMetadata()
	{
		return $this->videoMediaMetadata;
	}
	public function setViewedByMe($viewedByMe)
	{
		$this->viewedByMe = $viewedByMe;
	}
	public function getViewedByMe()
	{
		return $this->viewedByMe;
	}
	public function setViewedByMeTime($viewedByMeTime)
	{
		$this->viewedByMeTime = $viewedByMeTime;
	}
	public function getViewedByMeTime()
	{
		return $this->viewedByMeTime;
	}
	public function setViewersCanCopyContent($viewersCanCopyContent)
	{
		$this->viewersCanCopyContent = $viewersCanCopyContent;
	}
	public function getViewersCanCopyContent()
	{
		return $this->viewersCanCopyContent;
	}
	public function setWebContentLink($webContentLink)
	{
		$this->webContentLink = $webContentLink;
	}
	public function getWebContentLink()
	{
		return $this->webContentLink;
	}
	public function setWebViewLink($webViewLink)
	{
		$this->webViewLink = $webViewLink;
	}
	public function getWebViewLink()
	{
		return $this->webViewLink;
	}
	public function setWritersCanShare($writersCanShare)
	{
		$this->writersCanShare = $writersCanShare;
	}
	public function getWritersCanShare()
	{
		return $this->writersCanShare;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_DriveFileCapabilities extends GoogleGAL_Model
{
	public $canAddChildren;
	public $canComment;
	public $canCopy;
	public $canDelete;
	public $canDownload;
	public $canEdit;
	public $canListChildren;
	public $canMoveItemIntoTeamDrive;
	public $canMoveTeamDriveItem;
	public $canReadRevisions;
	public $canReadTeamDrive;
	public $canRemoveChildren;
	public $canRename;
	public $canShare;
	public $canTrash;
	public $canUntrash;

	public function setCanAddChildren($canAddChildren)
	{
		$this->canAddChildren = $canAddChildren;
	}
	public function getCanAddChildren()
	{
		return $this->canAddChildren;
	}
	public function setCanComment($canComment)
	{
		$this->canComment = $canComment;
	}
	public function getCanComment()
	{
		return $this->canComment;
	}
	public function setCanCopy($canCopy)
	{
		$this->canCopy = $canCopy;
	}
	public function getCanCopy()
	{
		return $this->canCopy;
	}
	public function setCanDelete($canDelete)
	{
		$this->canDelete = $canDelete;
	}
	public function getCanDelete()
	{
		return $this->canDelete;
	}
	public function setCanDownload($canDownload)
	{
		$this->canDownload = $canDownload;
	}
	public function getCanDownload()
	{
		return $this->canDownload;
	}
	public function setCanEdit($canEdit)
	{
		$this->canEdit = $canEdit;
	}
	public function getCanEdit()
	{
		return $this->canEdit;
	}
	public function setCanListChildren($canListChildren)
	{
		$this->canListChildren = $canListChildren;
	}
	public function getCanListChildren()
	{
		return $this->canListChildren;
	}
	public function setCanMoveItemIntoTeamDrive($canMoveItemIntoTeamDrive)
	{
		$this->canMoveItemIntoTeamDrive = $canMoveItemIntoTeamDrive;
	}
	public function getCanMoveItemIntoTeamDrive()
	{
		return $this->canMoveItemIntoTeamDrive;
	}
	public function setCanMoveTeamDriveItem($canMoveTeamDriveItem)
	{
		$this->canMoveTeamDriveItem = $canMoveTeamDriveItem;
	}
	public function getCanMoveTeamDriveItem()
	{
		return $this->canMoveTeamDriveItem;
	}
	public function setCanReadRevisions($canReadRevisions)
	{
		$this->canReadRevisions = $canReadRevisions;
	}
	public function getCanReadRevisions()
	{
		return $this->canReadRevisions;
	}
	public function setCanReadTeamDrive($canReadTeamDrive)
	{
		$this->canReadTeamDrive = $canReadTeamDrive;
	}
	public function getCanReadTeamDrive()
	{
		return $this->canReadTeamDrive;
	}
	public function setCanRemoveChildren($canRemoveChildren)
	{
		$this->canRemoveChildren = $canRemoveChildren;
	}
	public function getCanRemoveChildren()
	{
		return $this->canRemoveChildren;
	}
	public function setCanRename($canRename)
	{
		$this->canRename = $canRename;
	}
	public function getCanRename()
	{
		return $this->canRename;
	}
	public function setCanShare($canShare)
	{
		$this->canShare = $canShare;
	}
	public function getCanShare()
	{
		return $this->canShare;
	}
	public function setCanTrash($canTrash)
	{
		$this->canTrash = $canTrash;
	}
	public function getCanTrash()
	{
		return $this->canTrash;
	}
	public function setCanUntrash($canUntrash)
	{
		$this->canUntrash = $canUntrash;
	}
	public function getCanUntrash()
	{
		return $this->canUntrash;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_DriveFileContentHints extends GoogleGAL_Model
{
	public $indexableText;
	protected $thumbnailType = 'GoogleGAL_Service_DriveT_DriveFileContentHintsThumbnail';
	protected $thumbnailDataType = '';

	public function setIndexableText($indexableText)
	{
		$this->indexableText = $indexableText;
	}
	public function getIndexableText()
	{
		return $this->indexableText;
	}
	public function setThumbnail(GoogleGAL_Service_DriveT_DriveFileContentHintsThumbnail $thumbnail)
	{
		$this->thumbnail = $thumbnail;
	}
	public function getThumbnail()
	{
		return $this->thumbnail;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_DriveFileContentHintsThumbnail extends GoogleGAL_Model
{
	public $image;
	public $mimeType;

	public function setImage($image)
	{
		$this->image = $image;
	}
	public function getImage()
	{
		return $this->image;
	}
	public function setMimeType($mimeType)
	{
		$this->mimeType = $mimeType;
	}
	public function getMimeType()
	{
		return $this->mimeType;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_DriveFileImageMediaMetadata extends GoogleGAL_Model
{
	public $aperture;
	public $cameraMake;
	public $cameraModel;
	public $colorSpace;
	public $exposureBias;
	public $exposureMode;
	public $exposureTime;
	public $flashUsed;
	public $focalLength;
	public $height;
	public $isoSpeed;
	public $lens;
	protected $locationType = 'GoogleGAL_Service_DriveT_DriveFileImageMediaMetadataLocation';
	protected $locationDataType = '';
	public $maxApertureValue;
	public $meteringMode;
	public $rotation;
	public $sensor;
	public $subjectDistance;
	public $time;
	public $whiteBalance;
	public $width;

	public function setAperture($aperture)
	{
		$this->aperture = $aperture;
	}
	public function getAperture()
	{
		return $this->aperture;
	}
	public function setCameraMake($cameraMake)
	{
		$this->cameraMake = $cameraMake;
	}
	public function getCameraMake()
	{
		return $this->cameraMake;
	}
	public function setCameraModel($cameraModel)
	{
		$this->cameraModel = $cameraModel;
	}
	public function getCameraModel()
	{
		return $this->cameraModel;
	}
	public function setColorSpace($colorSpace)
	{
		$this->colorSpace = $colorSpace;
	}
	public function getColorSpace()
	{
		return $this->colorSpace;
	}
	public function setExposureBias($exposureBias)
	{
		$this->exposureBias = $exposureBias;
	}
	public function getExposureBias()
	{
		return $this->exposureBias;
	}
	public function setExposureMode($exposureMode)
	{
		$this->exposureMode = $exposureMode;
	}
	public function getExposureMode()
	{
		return $this->exposureMode;
	}
	public function setExposureTime($exposureTime)
	{
		$this->exposureTime = $exposureTime;
	}
	public function getExposureTime()
	{
		return $this->exposureTime;
	}
	public function setFlashUsed($flashUsed)
	{
		$this->flashUsed = $flashUsed;
	}
	public function getFlashUsed()
	{
		return $this->flashUsed;
	}
	public function setFocalLength($focalLength)
	{
		$this->focalLength = $focalLength;
	}
	public function getFocalLength()
	{
		return $this->focalLength;
	}
	public function setHeight($height)
	{
		$this->height = $height;
	}
	public function getHeight()
	{
		return $this->height;
	}
	public function setIsoSpeed($isoSpeed)
	{
		$this->isoSpeed = $isoSpeed;
	}
	public function getIsoSpeed()
	{
		return $this->isoSpeed;
	}
	public function setLens($lens)
	{
		$this->lens = $lens;
	}
	public function getLens()
	{
		return $this->lens;
	}
	public function setLocation(GoogleGAL_Service_DriveT_DriveFileImageMediaMetadataLocation $location)
	{
		$this->location = $location;
	}
	public function getLocation()
	{
		return $this->location;
	}
	public function setMaxApertureValue($maxApertureValue)
	{
		$this->maxApertureValue = $maxApertureValue;
	}
	public function getMaxApertureValue()
	{
		return $this->maxApertureValue;
	}
	public function setMeteringMode($meteringMode)
	{
		$this->meteringMode = $meteringMode;
	}
	public function getMeteringMode()
	{
		return $this->meteringMode;
	}
	public function setRotation($rotation)
	{
		$this->rotation = $rotation;
	}
	public function getRotation()
	{
		return $this->rotation;
	}
	public function setSensor($sensor)
	{
		$this->sensor = $sensor;
	}
	public function getSensor()
	{
		return $this->sensor;
	}
	public function setSubjectDistance($subjectDistance)
	{
		$this->subjectDistance = $subjectDistance;
	}
	public function getSubjectDistance()
	{
		return $this->subjectDistance;
	}
	public function setTime($time)
	{
		$this->time = $time;
	}
	public function getTime()
	{
		return $this->time;
	}
	public function setWhiteBalance($whiteBalance)
	{
		$this->whiteBalance = $whiteBalance;
	}
	public function getWhiteBalance()
	{
		return $this->whiteBalance;
	}
	public function setWidth($width)
	{
		$this->width = $width;
	}
	public function getWidth()
	{
		return $this->width;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_DriveFileImageMediaMetadataLocation extends GoogleGAL_Model
{
	public $altitude;
	public $latitude;
	public $longitude;

	public function setAltitude($altitude)
	{
		$this->altitude = $altitude;
	}
	public function getAltitude()
	{
		return $this->altitude;
	}
	public function setLatitude($latitude)
	{
		$this->latitude = $latitude;
	}
	public function getLatitude()
	{
		return $this->latitude;
	}
	public function setLongitude($longitude)
	{
		$this->longitude = $longitude;
	}
	public function getLongitude()
	{
		return $this->longitude;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_DriveFileVideoMediaMetadata extends GoogleGAL_Model
{
	public $durationMillis;
	public $height;
	public $width;

	public function setDurationMillis($durationMillis)
	{
		$this->durationMillis = $durationMillis;
	}
	public function getDurationMillis()
	{
		return $this->durationMillis;
	}
	public function setHeight($height)
	{
		$this->height = $height;
	}
	public function getHeight()
	{
		return $this->height;
	}
	public function setWidth($width)
	{
		$this->width = $width;
	}
	public function getWidth()
	{
		return $this->width;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_FileList extends GoogleGAL_Collection
{
	protected $collection_key = 'files';
	protected $filesType = 'GoogleGAL_Service_DriveT_DriveFile';
	protected $filesDataType = 'array';
	public $incompleteSearch;
	public $kind;
	public $nextPageToken;

	public function setFiles($files)
	{
		$this->files = $files;
	}
	public function getFiles()
	{
		return $this->files;
	}
	public function setIncompleteSearch($incompleteSearch)
	{
		$this->incompleteSearch = $incompleteSearch;
	}
	public function getIncompleteSearch()
	{
		return $this->incompleteSearch;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setNextPageToken($nextPageToken)
	{
		$this->nextPageToken = $nextPageToken;
	}
	public function getNextPageToken()
	{
		return $this->nextPageToken;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_GeneratedIds extends GoogleGAL_Collection
{
	protected $collection_key = 'ids';
	public $ids;
	public $kind;
	public $space;

	public function setIds($ids)
	{
		$this->ids = $ids;
	}
	public function getIds()
	{
		return $this->ids;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setSpace($space)
	{
		$this->space = $space;
	}
	public function getSpace()
	{
		return $this->space;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_Permission extends GoogleGAL_Collection
{
	protected $collection_key = 'teamDrivePermissionDetails';
	public $allowFileDiscovery;
	public $displayName;
	public $domain;
	public $emailAddress;
	public $expirationTime;
	public $id;
	public $kind;
	public $photoLink;
	public $role;
	protected $teamDrivePermissionDetailsType = 'GoogleGAL_Service_DriveT_PermissionTeamDrivePermissionDetails';
	protected $teamDrivePermissionDetailsDataType = 'array';
	public $type;

	public function setAllowFileDiscovery($allowFileDiscovery)
	{
		$this->allowFileDiscovery = $allowFileDiscovery;
	}
	public function getAllowFileDiscovery()
	{
		return $this->allowFileDiscovery;
	}
	public function setDisplayName($displayName)
	{
		$this->displayName = $displayName;
	}
	public function getDisplayName()
	{
		return $this->displayName;
	}
	public function setDomain($domain)
	{
		$this->domain = $domain;
	}
	public function getDomain()
	{
		return $this->domain;
	}
	public function setEmailAddress($emailAddress)
	{
		$this->emailAddress = $emailAddress;
	}
	public function getEmailAddress()
	{
		return $this->emailAddress;
	}
	public function setExpirationTime($expirationTime)
	{
		$this->expirationTime = $expirationTime;
	}
	public function getExpirationTime()
	{
		return $this->expirationTime;
	}
	public function setId($id)
	{
		$this->id = $id;
	}
	public function getId()
	{
		return $this->id;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setPhotoLink($photoLink)
	{
		$this->photoLink = $photoLink;
	}
	public function getPhotoLink()
	{
		return $this->photoLink;
	}
	public function setRole($role)
	{
		$this->role = $role;
	}
	public function getRole()
	{
		return $this->role;
	}
	public function setTeamDrivePermissionDetails($teamDrivePermissionDetails)
	{
		$this->teamDrivePermissionDetails = $teamDrivePermissionDetails;
	}
	public function getTeamDrivePermissionDetails()
	{
		return $this->teamDrivePermissionDetails;
	}
	public function setType($type)
	{
		$this->type = $type;
	}
	public function getType()
	{
		return $this->type;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_PermissionList extends GoogleGAL_Collection
{
	protected $collection_key = 'permissions';
	public $kind;
	public $nextPageToken;
	protected $permissionsType = 'GoogleGAL_Service_DriveT_Permission';
	protected $permissionsDataType = 'array';

	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setNextPageToken($nextPageToken)
	{
		$this->nextPageToken = $nextPageToken;
	}
	public function getNextPageToken()
	{
		return $this->nextPageToken;
	}
	public function setPermissions($permissions)
	{
		$this->permissions = $permissions;
	}
	public function getPermissions()
	{
		return $this->permissions;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_PermissionTeamDrivePermissionDetails extends GoogleGAL_Model
{
	public $inherited;
	public $inheritedFrom;
	public $role;
	public $teamDrivePermissionType;

	public function setInherited($inherited)
	{
		$this->inherited = $inherited;
	}
	public function getInherited()
	{
		return $this->inherited;
	}
	public function setInheritedFrom($inheritedFrom)
	{
		$this->inheritedFrom = $inheritedFrom;
	}
	public function getInheritedFrom()
	{
		return $this->inheritedFrom;
	}
	public function setRole($role)
	{
		$this->role = $role;
	}
	public function getRole()
	{
		return $this->role;
	}
	public function setTeamDrivePermissionType($teamDrivePermissionType)
	{
		$this->teamDrivePermissionType = $teamDrivePermissionType;
	}
	public function getTeamDrivePermissionType()
	{
		return $this->teamDrivePermissionType;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_Reply extends GoogleGAL_Model
{
	public $action;
	protected $authorType = 'GoogleGAL_Service_DriveT_User';
	protected $authorDataType = '';
	public $content;
	public $createdTime;
	public $deleted;
	public $htmlContent;
	public $id;
	public $kind;
	public $modifiedTime;

	public function setAction($action)
	{
		$this->action = $action;
	}
	public function getAction()
	{
		return $this->action;
	}
	public function setAuthor(GoogleGAL_Service_DriveT_User $author)
	{
		$this->author = $author;
	}
	public function getAuthor()
	{
		return $this->author;
	}
	public function setContent($content)
	{
		$this->content = $content;
	}
	public function getContent()
	{
		return $this->content;
	}
	public function setCreatedTime($createdTime)
	{
		$this->createdTime = $createdTime;
	}
	public function getCreatedTime()
	{
		return $this->createdTime;
	}
	public function setDeleted($deleted)
	{
		$this->deleted = $deleted;
	}
	public function getDeleted()
	{
		return $this->deleted;
	}
	public function setHtmlContent($htmlContent)
	{
		$this->htmlContent = $htmlContent;
	}
	public function getHtmlContent()
	{
		return $this->htmlContent;
	}
	public function setId($id)
	{
		$this->id = $id;
	}
	public function getId()
	{
		return $this->id;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setModifiedTime($modifiedTime)
	{
		$this->modifiedTime = $modifiedTime;
	}
	public function getModifiedTime()
	{
		return $this->modifiedTime;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_ReplyList extends GoogleGAL_Collection
{
	protected $collection_key = 'replies';
	public $kind;
	public $nextPageToken;
	protected $repliesType = 'GoogleGAL_Service_DriveT_Reply';
	protected $repliesDataType = 'array';

	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setNextPageToken($nextPageToken)
	{
		$this->nextPageToken = $nextPageToken;
	}
	public function getNextPageToken()
	{
		return $this->nextPageToken;
	}
	public function setReplies($replies)
	{
		$this->replies = $replies;
	}
	public function getReplies()
	{
		return $this->replies;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_Revision extends GoogleGAL_Model
{
	public $id;
	public $keepForever;
	public $kind;
	protected $lastModifyingUserType = 'GoogleGAL_Service_DriveT_User';
	protected $lastModifyingUserDataType = '';
	public $md5Checksum;
	public $mimeType;
	public $modifiedTime;
	public $originalFilename;
	public $publishAuto;
	public $published;
	public $publishedOutsideDomain;
	public $size;

	public function setId($id)
	{
		$this->id = $id;
	}
	public function getId()
	{
		return $this->id;
	}
	public function setKeepForever($keepForever)
	{
		$this->keepForever = $keepForever;
	}
	public function getKeepForever()
	{
		return $this->keepForever;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setLastModifyingUser(GoogleGAL_Service_DriveT_User $lastModifyingUser)
	{
		$this->lastModifyingUser = $lastModifyingUser;
	}
	public function getLastModifyingUser()
	{
		return $this->lastModifyingUser;
	}
	public function setMd5Checksum($md5Checksum)
	{
		$this->md5Checksum = $md5Checksum;
	}
	public function getMd5Checksum()
	{
		return $this->md5Checksum;
	}
	public function setMimeType($mimeType)
	{
		$this->mimeType = $mimeType;
	}
	public function getMimeType()
	{
		return $this->mimeType;
	}
	public function setModifiedTime($modifiedTime)
	{
		$this->modifiedTime = $modifiedTime;
	}
	public function getModifiedTime()
	{
		return $this->modifiedTime;
	}
	public function setOriginalFilename($originalFilename)
	{
		$this->originalFilename = $originalFilename;
	}
	public function getOriginalFilename()
	{
		return $this->originalFilename;
	}
	public function setPublishAuto($publishAuto)
	{
		$this->publishAuto = $publishAuto;
	}
	public function getPublishAuto()
	{
		return $this->publishAuto;
	}
	public function setPublished($published)
	{
		$this->published = $published;
	}
	public function getPublished()
	{
		return $this->published;
	}
	public function setPublishedOutsideDomain($publishedOutsideDomain)
	{
		$this->publishedOutsideDomain = $publishedOutsideDomain;
	}
	public function getPublishedOutsideDomain()
	{
		return $this->publishedOutsideDomain;
	}
	public function setSize($size)
	{
		$this->size = $size;
	}
	public function getSize()
	{
		return $this->size;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_RevisionList extends GoogleGAL_Collection
{
	protected $collection_key = 'revisions';
	public $kind;
	public $nextPageToken;
	protected $revisionsType = 'GoogleGAL_Service_DriveT_Revision';
	protected $revisionsDataType = 'array';

	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setNextPageToken($nextPageToken)
	{
		$this->nextPageToken = $nextPageToken;
	}
	public function getNextPageToken()
	{
		return $this->nextPageToken;
	}
	public function setRevisions($revisions)
	{
		$this->revisions = $revisions;
	}
	public function getRevisions()
	{
		return $this->revisions;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_StartPageToken extends GoogleGAL_Model
{
	public $kind;
	public $startPageToken;

	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setStartPageToken($startPageToken)
	{
		$this->startPageToken = $startPageToken;
	}
	public function getStartPageToken()
	{
		return $this->startPageToken;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_TeamDrive extends GoogleGAL_Model
{
	protected $capabilitiesType = 'GoogleGAL_Service_DriveT_TeamDriveCapabilities';
	protected $capabilitiesDataType = '';
	public $id;
	public $kind;
	public $name;

	public function setCapabilities(GoogleGAL_Service_DriveT_TeamDriveCapabilities $capabilities)
	{
		$this->capabilities = $capabilities;
	}
	public function getCapabilities()
	{
		return $this->capabilities;
	}
	public function setId($id)
	{
		$this->id = $id;
	}
	public function getId()
	{
		return $this->id;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setName($name)
	{
		$this->name = $name;
	}
	public function getName()
	{
		return $this->name;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_TeamDriveCapabilities extends GoogleGAL_Model
{
	public $canAddChildren;
	public $canComment;
	public $canCopy;
	public $canDeleteTeamDrive;
	public $canDownload;
	public $canEdit;
	public $canListChildren;
	public $canManageMembers;
	public $canReadRevisions;
	public $canRemoveChildren;
	public $canRename;
	public $canRenameTeamDrive;
	public $canShare;

	public function setCanAddChildren($canAddChildren)
	{
		$this->canAddChildren = $canAddChildren;
	}
	public function getCanAddChildren()
	{
		return $this->canAddChildren;
	}
	public function setCanComment($canComment)
	{
		$this->canComment = $canComment;
	}
	public function getCanComment()
	{
		return $this->canComment;
	}
	public function setCanCopy($canCopy)
	{
		$this->canCopy = $canCopy;
	}
	public function getCanCopy()
	{
		return $this->canCopy;
	}
	public function setCanDeleteTeamDrive($canDeleteTeamDrive)
	{
		$this->canDeleteTeamDrive = $canDeleteTeamDrive;
	}
	public function getCanDeleteTeamDrive()
	{
		return $this->canDeleteTeamDrive;
	}
	public function setCanDownload($canDownload)
	{
		$this->canDownload = $canDownload;
	}
	public function getCanDownload()
	{
		return $this->canDownload;
	}
	public function setCanEdit($canEdit)
	{
		$this->canEdit = $canEdit;
	}
	public function getCanEdit()
	{
		return $this->canEdit;
	}
	public function setCanListChildren($canListChildren)
	{
		$this->canListChildren = $canListChildren;
	}
	public function getCanListChildren()
	{
		return $this->canListChildren;
	}
	public function setCanManageMembers($canManageMembers)
	{
		$this->canManageMembers = $canManageMembers;
	}
	public function getCanManageMembers()
	{
		return $this->canManageMembers;
	}
	public function setCanReadRevisions($canReadRevisions)
	{
		$this->canReadRevisions = $canReadRevisions;
	}
	public function getCanReadRevisions()
	{
		return $this->canReadRevisions;
	}
	public function setCanRemoveChildren($canRemoveChildren)
	{
		$this->canRemoveChildren = $canRemoveChildren;
	}
	public function getCanRemoveChildren()
	{
		return $this->canRemoveChildren;
	}
	public function setCanRename($canRename)
	{
		$this->canRename = $canRename;
	}
	public function getCanRename()
	{
		return $this->canRename;
	}
	public function setCanRenameTeamDrive($canRenameTeamDrive)
	{
		$this->canRenameTeamDrive = $canRenameTeamDrive;
	}
	public function getCanRenameTeamDrive()
	{
		return $this->canRenameTeamDrive;
	}
	public function setCanShare($canShare)
	{
		$this->canShare = $canShare;
	}
	public function getCanShare()
	{
		return $this->canShare;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_TeamDriveList extends GoogleGAL_Collection
{
	protected $collection_key = 'teamDrives';
	public $kind;
	public $nextPageToken;
	protected $teamDrivesType = 'GoogleGAL_Service_DriveT_TeamDrive';
	protected $teamDrivesDataType = 'array';

	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setNextPageToken($nextPageToken)
	{
		$this->nextPageToken = $nextPageToken;
	}
	public function getNextPageToken()
	{
		return $this->nextPageToken;
	}
	public function setTeamDrives($teamDrives)
	{
		$this->teamDrives = $teamDrives;
	}
	public function getTeamDrives()
	{
		return $this->teamDrives;
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

class GoogleGAL_Service_DriveT_User extends GoogleGAL_Model
{
	public $displayName;
	public $emailAddress;
	public $kind;
	public $me;
	public $permissionId;
	public $photoLink;

	public function setDisplayName($displayName)
	{
		$this->displayName = $displayName;
	}
	public function getDisplayName()
	{
		return $this->displayName;
	}
	public function setEmailAddress($emailAddress)
	{
		$this->emailAddress = $emailAddress;
	}
	public function getEmailAddress()
	{
		return $this->emailAddress;
	}
	public function setKind($kind)
	{
		$this->kind = $kind;
	}
	public function getKind()
	{
		return $this->kind;
	}
	public function setMe($me)
	{
		$this->me = $me;
	}
	public function getMe()
	{
		return $this->me;
	}
	public function setPermissionId($permissionId)
	{
		$this->permissionId = $permissionId;
	}
	public function getPermissionId()
	{
		return $this->permissionId;
	}
	public function setPhotoLink($photoLink)
	{
		$this->photoLink = $photoLink;
	}
	public function getPhotoLink()
	{
		return $this->photoLink;
	}
}

  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * The "about" collection of methods.
 * Typical usage is:
 *  <code>
 *   $driveService = new GoogleGAL_Service_DriveT(...);
 *   $about = $driveService->about;
 *  </code>
 */
class GoogleGAL_Service_DriveT_Resource_About extends GoogleGAL_Service_Resource
{
	/**
	 * Gets information about the user, the user's Drive, and system capabilities.
	 * (about.get)
	 *
	 * @param array $optParams Optional parameters.
	 * @return GoogleGAL_Service_DriveT_About
	 */
	public function get($optParams = array())
	{
		$params = array();
		$params = array_merge($params, $optParams);
		return $this->call('get', array($params), "GoogleGAL_Service_DriveT_About");
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * The "changes" collection of methods.
 * Typical usage is:
 *  <code>
 *   $driveService = new GoogleGAL_Service_DriveT(...);
 *   $changes = $driveService->changes;
 *  </code>
 */
class GoogleGAL_Service_DriveT_Resource_Changes extends GoogleGAL_Service_Resource
{
	/**
	 * Gets the starting pageToken for listing future changes.
	 * (changes.getStartPageToken)
	 *
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @opt_param string teamDriveId The ID of the Team Drive for which the starting
	 * pageToken for listing future changes from that Team Drive will be returned.
	 * @return GoogleGAL_Service_DriveT_StartPageToken
	 */
	public function getStartPageToken($optParams = array())
	{
		$params = array();
		$params = array_merge($params, $optParams);
		return $this->call('getStartPageToken', array($params), "GoogleGAL_Service_DriveT_StartPageToken");
	}
	/**
	 * Lists the changes for a user or Team Drive. (changes.listChanges)
	 *
	 * @param string $pageToken The token for continuing a previous list request on
	 * the next page. This should be set to the value of 'nextPageToken' from the
	 * previous response or to the response from the getStartPageToken method.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool includeCorpusRemovals Whether changes should include the file
	 * resource if the file is still accessible by the user at the time of the
	 * request, even when a file was removed from the list of changes and there will
	 * be no further change entries for this file.
	 * @opt_param bool includeRemoved Whether to include changes indicating that
	 * items have been removed from the list of changes, for example by deletion or
	 * loss of access.
	 * @opt_param bool includeTeamDriveItems Whether Team Drive files or changes
	 * should be included in results.
	 * @opt_param int pageSize The maximum number of changes to return per page.
	 * @opt_param bool restrictToMyDrive Whether to restrict the results to changes
	 * inside the My Drive hierarchy. This omits changes to files such as those in
	 * the Application Data folder or shared files which have not been added to My
	 * Drive.
	 * @opt_param string spaces A comma-separated list of spaces to query within the
	 * user corpus. Supported values are 'drive', 'appDataFolder' and 'photos'.
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @opt_param string teamDriveId The Team Drive from which changes will be
	 * returned. If specified the change IDs will be reflective of the Team Drive;
	 * use the combined Team Drive ID and change ID as an identifier.
	 * @return GoogleGAL_Service_DriveT_ChangeList
	 */
	public function listChanges($pageToken, $optParams = array())
	{
		$params = array('pageToken' => $pageToken);
		$params = array_merge($params, $optParams);
		return $this->call('list', array($params), "GoogleGAL_Service_DriveT_ChangeList");
	}
	/**
	 * Subscribes to changes for a user. (changes.watch)
	 *
	 * @param string $pageToken The token for continuing a previous list request on
	 * the next page. This should be set to the value of 'nextPageToken' from the
	 * previous response or to the response from the getStartPageToken method.
	 * @param GoogleGAL_Service_DriveT_Channel $postBody
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool includeCorpusRemovals Whether changes should include the file
	 * resource if the file is still accessible by the user at the time of the
	 * request, even when a file was removed from the list of changes and there will
	 * be no further change entries for this file.
	 * @opt_param bool includeRemoved Whether to include changes indicating that
	 * items have been removed from the list of changes, for example by deletion or
	 * loss of access.
	 * @opt_param bool includeTeamDriveItems Whether Team Drive files or changes
	 * should be included in results.
	 * @opt_param int pageSize The maximum number of changes to return per page.
	 * @opt_param bool restrictToMyDrive Whether to restrict the results to changes
	 * inside the My Drive hierarchy. This omits changes to files such as those in
	 * the Application Data folder or shared files which have not been added to My
	 * Drive.
	 * @opt_param string spaces A comma-separated list of spaces to query within the
	 * user corpus. Supported values are 'drive', 'appDataFolder' and 'photos'.
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @opt_param string teamDriveId The Team Drive from which changes will be
	 * returned. If specified the change IDs will be reflective of the Team Drive;
	 * use the combined Team Drive ID and change ID as an identifier.
	 * @return GoogleGAL_Service_DriveT_Channel
	 */
	public function watch($pageToken, GoogleGAL_Service_DriveT_Channel $postBody, $optParams = array())
	{
		$params = array('pageToken' => $pageToken, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('watch', array($params), "GoogleGAL_Service_DriveT_Channel");
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * The "channels" collection of methods.
 * Typical usage is:
 *  <code>
 *   $driveService = new GoogleGAL_Service_DriveT(...);
 *   $channels = $driveService->channels;
 *  </code>
 */
class GoogleGAL_Service_DriveT_Resource_Channels extends GoogleGAL_Service_Resource
{
	/**
	 * Stop watching resources through this channel (channels.stop)
	 *
	 * @param GoogleGAL_Service_DriveT_Channel $postBody
	 * @param array $optParams Optional parameters.
	 */
	public function stop(GoogleGAL_Service_DriveT_Channel $postBody, $optParams = array())
	{
		$params = array('postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('stop', array($params));
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * The "comments" collection of methods.
 * Typical usage is:
 *  <code>
 *   $driveService = new GoogleGAL_Service_DriveT(...);
 *   $comments = $driveService->comments;
 *  </code>
 */
class GoogleGAL_Service_DriveT_Resource_Comments extends GoogleGAL_Service_Resource
{
	/**
	 * Creates a new comment on a file. (comments.create)
	 *
	 * @param string $fileId The ID of the file.
	 * @param GoogleGAL_Service_DriveT_Comment $postBody
	 * @param array $optParams Optional parameters.
	 * @return GoogleGAL_Service_DriveT_Comment
	 */
	public function create($fileId, GoogleGAL_Service_DriveT_Comment $postBody, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('create', array($params), "GoogleGAL_Service_DriveT_Comment");
	}
	/**
	 * Deletes a comment. (comments.delete)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $commentId The ID of the comment.
	 * @param array $optParams Optional parameters.
	 */
	public function delete($fileId, $commentId, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'commentId' => $commentId);
		$params = array_merge($params, $optParams);
		return $this->call('delete', array($params));
	}
	/**
	 * Gets a comment by ID. (comments.get)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $commentId The ID of the comment.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool includeDeleted Whether to return deleted comments. Deleted
	 * comments will not include their original content.
	 * @return GoogleGAL_Service_DriveT_Comment
	 */
	public function get($fileId, $commentId, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'commentId' => $commentId);
		$params = array_merge($params, $optParams);
		return $this->call('get', array($params), "GoogleGAL_Service_DriveT_Comment");
	}
	/**
	 * Lists a file's comments. (comments.listComments)
	 *
	 * @param string $fileId The ID of the file.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool includeDeleted Whether to include deleted comments. Deleted
	 * comments will not include their original content.
	 * @opt_param int pageSize The maximum number of comments to return per page.
	 * @opt_param string pageToken The token for continuing a previous list request
	 * on the next page. This should be set to the value of 'nextPageToken' from the
	 * previous response.
	 * @opt_param string startModifiedTime The minimum value of 'modifiedTime' for
	 * the result comments (RFC 3339 date-time).
	 * @return GoogleGAL_Service_DriveT_CommentList
	 */
	public function listComments($fileId, $optParams = array())
	{
		$params = array('fileId' => $fileId);
		$params = array_merge($params, $optParams);
		return $this->call('list', array($params), "GoogleGAL_Service_DriveT_CommentList");
	}
	/**
	 * Updates a comment with patch semantics. (comments.update)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $commentId The ID of the comment.
	 * @param GoogleGAL_Service_DriveT_Comment $postBody
	 * @param array $optParams Optional parameters.
	 * @return GoogleGAL_Service_DriveT_Comment
	 */
	public function update($fileId, $commentId, GoogleGAL_Service_DriveT_Comment $postBody, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'commentId' => $commentId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('update', array($params), "GoogleGAL_Service_DriveT_Comment");
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * The "files" collection of methods.
 * Typical usage is:
 *  <code>
 *   $driveService = new GoogleGAL_Service_DriveT(...);
 *   $files = $driveService->files;
 *  </code>
 */
class GoogleGAL_Service_DriveT_Resource_Files extends GoogleGAL_Service_Resource
{
	/**
	 * Creates a copy of a file and applies any requested updates with patch
	 * semantics. (files.copy)
	 *
	 * @param string $fileId The ID of the file.
	 * @param GoogleGAL_Service_DriveT_DriveFile $postBody
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool ignoreDefaultVisibility Whether to ignore the domain's
	 * default visibility settings for the created file. Domain administrators can
	 * choose to make all uploaded files visible to the domain by default; this
	 * parameter bypasses that behavior for the request. Permissions are still
	 * inherited from parent folders.
	 * @opt_param bool keepRevisionForever Whether to set the 'keepForever' field in
	 * the new head revision. This is only applicable to files with binary content
	 * in Drive.
	 * @opt_param string ocrLanguage A language hint for OCR processing during image
	 * import (ISO 639-1 code).
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @return GoogleGAL_Service_DriveT_DriveFile
	 */
	public function copy($fileId, GoogleGAL_Service_DriveT_DriveFile $postBody, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('copy', array($params), "GoogleGAL_Service_DriveT_DriveFile");
	}
	/**
	 * Creates a new file. (files.create)
	 *
	 * @param GoogleGAL_Service_DriveT_DriveFile $postBody
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool ignoreDefaultVisibility Whether to ignore the domain's
	 * default visibility settings for the created file. Domain administrators can
	 * choose to make all uploaded files visible to the domain by default; this
	 * parameter bypasses that behavior for the request. Permissions are still
	 * inherited from parent folders.
	 * @opt_param bool keepRevisionForever Whether to set the 'keepForever' field in
	 * the new head revision. This is only applicable to files with binary content
	 * in Drive.
	 * @opt_param string ocrLanguage A language hint for OCR processing during image
	 * import (ISO 639-1 code).
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @opt_param bool useContentAsIndexableText Whether to use the uploaded content
	 * as indexable text.
	 * @return GoogleGAL_Service_DriveT_DriveFile
	 */
	public function create(GoogleGAL_Service_DriveT_DriveFile $postBody, $optParams = array())
	{
		$params = array('postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('create', array($params), "GoogleGAL_Service_DriveT_DriveFile");
	}
	/**
	 * Permanently deletes a file owned by the user without moving it to the trash.
	 * If the file belongs to a Team Drive the user must be an organizer on the
	 * parent. If the target is a folder, all descendants owned by the user are also
	 * deleted. (files.delete)
	 *
	 * @param string $fileId The ID of the file.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 */
	public function delete($fileId, $optParams = array())
	{
		$params = array('fileId' => $fileId);
		$params = array_merge($params, $optParams);
		return $this->call('delete', array($params));
	}
	/**
	 * Permanently deletes all of the user's trashed files. (files.emptyTrash)
	 *
	 * @param array $optParams Optional parameters.
	 */
	public function emptyTrash($optParams = array())
	{
		$params = array();
		$params = array_merge($params, $optParams);
		return $this->call('emptyTrash', array($params));
	}
	/**
	 * Exports a Google Doc to the requested MIME type and returns the exported
	 * content. (files.export)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $mimeType The MIME type of the format requested for this
	 * export.
	 * @param array $optParams Optional parameters.
	 */
	public function export($fileId, $mimeType, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'mimeType' => $mimeType);
		$params = array_merge($params, $optParams);
		return $this->call('export', array($params));
	}
	/**
	 * Generates a set of file IDs which can be provided in create requests.
	 * (files.generateIds)
	 *
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param int count The number of IDs to return.
	 * @opt_param string space The space in which the IDs can be used to create new
	 * files. Supported values are 'drive' and 'appDataFolder'.
	 * @return GoogleGAL_Service_DriveT_GeneratedIds
	 */
	public function generateIds($optParams = array())
	{
		$params = array();
		$params = array_merge($params, $optParams);
		return $this->call('generateIds', array($params), "GoogleGAL_Service_DriveT_GeneratedIds");
	}
	/**
	 * Gets a file's metadata or content by ID. (files.get)
	 *
	 * @param string $fileId The ID of the file.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool acknowledgeAbuse Whether the user is acknowledging the risk
	 * of downloading known malware or other abusive files. This is only applicable
	 * when alt=media.
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @return GoogleGAL_Service_DriveT_DriveFile
	 */
	public function get($fileId, $optParams = array())
	{
		$params = array('fileId' => $fileId);
		$params = array_merge($params, $optParams);
		return $this->call('get', array($params), "GoogleGAL_Service_DriveT_DriveFile");
	}
	/**
	 * Lists or searches files. (files.listFiles)
	 *
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param string corpora Comma-separated list of bodies of items
	 * (files/documents) to which the query applies. Supported bodies are 'user',
	 * 'domain', 'teamDrive' and 'allTeamDrives'. 'allTeamDrives' must be combined
	 * with 'user'; all other values must be used in isolation. Prefer 'user' or
	 * 'teamDrive' to 'allTeamDrives' for efficiency.
	 * @opt_param string corpus The source of files to list. Deprecated: use
	 * 'corpora' instead.
	 * @opt_param bool includeTeamDriveItems Whether Team Drive items should be
	 * included in results.
	 * @opt_param string orderBy A comma-separated list of sort keys. Valid keys are
	 * 'createdTime', 'folder', 'modifiedByMeTime', 'modifiedTime', 'name',
	 * 'quotaBytesUsed', 'recency', 'sharedWithMeTime', 'starred', and
	 * 'viewedByMeTime'. Each key sorts ascending by default, but may be reversed
	 * with the 'desc' modifier. Example usage: ?orderBy=folder,modifiedTime
	 * desc,name. Please note that there is a current limitation for users with
	 * approximately one million files in which the requested sort order is ignored.
	 * @opt_param int pageSize The maximum number of files to return per page.
	 * @opt_param string pageToken The token for continuing a previous list request
	 * on the next page. This should be set to the value of 'nextPageToken' from the
	 * previous response.
	 * @opt_param string q A query for filtering the file results. See the "Search
	 * for Files" guide for supported syntax.
	 * @opt_param string spaces A comma-separated list of spaces to query within the
	 * corpus. Supported values are 'drive', 'appDataFolder' and 'photos'.
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @opt_param string teamDriveId ID of Team Drive to search.
	 * @return GoogleGAL_Service_DriveT_FileList
	 */
	public function listFiles($optParams = array())
	{
		$params = array();
		$params = array_merge($params, $optParams);
		return $this->call('list', array($params), "GoogleGAL_Service_DriveT_FileList");
	}
	/**
	 * Updates a file's metadata and/or content with patch semantics. (files.update)
	 *
	 * @param string $fileId The ID of the file.
	 * @param GoogleGAL_Service_DriveT_DriveFile $postBody
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param string addParents A comma-separated list of parent IDs to add.
	 * @opt_param bool keepRevisionForever Whether to set the 'keepForever' field in
	 * the new head revision. This is only applicable to files with binary content
	 * in Drive.
	 * @opt_param string ocrLanguage A language hint for OCR processing during image
	 * import (ISO 639-1 code).
	 * @opt_param string removeParents A comma-separated list of parent IDs to
	 * remove.
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @opt_param bool useContentAsIndexableText Whether to use the uploaded content
	 * as indexable text.
	 * @return GoogleGAL_Service_DriveT_DriveFile
	 */
	public function update($fileId, GoogleGAL_Service_DriveT_DriveFile $postBody, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('update', array($params), "GoogleGAL_Service_DriveT_DriveFile");
	}
	/**
	 * Subscribes to changes to a file (files.watch)
	 *
	 * @param string $fileId The ID of the file.
	 * @param GoogleGAL_Service_DriveT_Channel $postBody
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool acknowledgeAbuse Whether the user is acknowledging the risk
	 * of downloading known malware or other abusive files. This is only applicable
	 * when alt=media.
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @return GoogleGAL_Service_DriveT_Channel
	 */
	public function watch($fileId, GoogleGAL_Service_DriveT_Channel $postBody, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('watch', array($params), "GoogleGAL_Service_DriveT_Channel");
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * The "permissions" collection of methods.
 * Typical usage is:
 *  <code>
 *   $driveService = new GoogleGAL_Service_DriveT(...);
 *   $permissions = $driveService->permissions;
 *  </code>
 */
class GoogleGAL_Service_DriveT_Resource_Permissions extends GoogleGAL_Service_Resource
{
	/**
	 * Creates a permission for a file or Team Drive. (permissions.create)
	 *
	 * @param string $fileId The ID of the file or Team Drive.
	 * @param GoogleGAL_Service_DriveT_Permission $postBody
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param string emailMessage A custom message to include in the
	 * notification email.
	 * @opt_param bool sendNotificationEmail Whether to send a notification email
	 * when sharing to users or groups. This defaults to true for users and groups,
	 * and is not allowed for other requests. It must not be disabled for ownership
	 * transfers.
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @opt_param bool transferOwnership Whether to transfer ownership to the
	 * specified user and downgrade the current owner to a writer. This parameter is
	 * required as an acknowledgement of the side effect.
	 * @return GoogleGAL_Service_DriveT_Permission
	 */
	public function create($fileId, GoogleGAL_Service_DriveT_Permission $postBody, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('create', array($params), "GoogleGAL_Service_DriveT_Permission");
	}
	/**
	 * Deletes a permission. (permissions.delete)
	 *
	 * @param string $fileId The ID of the file or Team Drive.
	 * @param string $permissionId The ID of the permission.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 */
	public function delete($fileId, $permissionId, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'permissionId' => $permissionId);
		$params = array_merge($params, $optParams);
		return $this->call('delete', array($params));
	}
	/**
	 * Gets a permission by ID. (permissions.get)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $permissionId The ID of the permission.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @return GoogleGAL_Service_DriveT_Permission
	 */
	public function get($fileId, $permissionId, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'permissionId' => $permissionId);
		$params = array_merge($params, $optParams);
		return $this->call('get', array($params), "GoogleGAL_Service_DriveT_Permission");
	}
	/**
	 * Lists a file's or Team Drive's permissions. (permissions.listPermissions)
	 *
	 * @param string $fileId The ID of the file or Team Drive.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param int pageSize The maximum number of permissions to return per page.
	 * When not set for files in a Team Drive, at most 100 results will be returned.
	 * When not set for files that are not in a Team Drive, the entire list will be
	 * returned.
	 * @opt_param string pageToken The token for continuing a previous list request
	 * on the next page. This should be set to the value of 'nextPageToken' from the
	 * previous response.
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @return GoogleGAL_Service_DriveT_PermissionList
	 */
	public function listPermissions($fileId, $optParams = array())
	{
		$params = array('fileId' => $fileId);
		$params = array_merge($params, $optParams);
		return $this->call('list', array($params), "GoogleGAL_Service_DriveT_PermissionList");
	}
	/**
	 * Updates a permission with patch semantics. (permissions.update)
	 *
	 * @param string $fileId The ID of the file or Team Drive.
	 * @param string $permissionId The ID of the permission.
	 * @param GoogleGAL_Service_DriveT_Permission $postBody
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool removeExpiration Whether to remove the expiration date.
	 * @opt_param bool supportsTeamDrives Whether the requesting application
	 * supports Team Drives.
	 * @opt_param bool transferOwnership Whether to transfer ownership to the
	 * specified user and downgrade the current owner to a writer. This parameter is
	 * required as an acknowledgement of the side effect.
	 * @return GoogleGAL_Service_DriveT_Permission
	 */
	public function update($fileId, $permissionId, GoogleGAL_Service_DriveT_Permission $postBody, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'permissionId' => $permissionId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('update', array($params), "GoogleGAL_Service_DriveT_Permission");
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * The "replies" collection of methods.
 * Typical usage is:
 *  <code>
 *   $driveService = new GoogleGAL_Service_DriveT(...);
 *   $replies = $driveService->replies;
 *  </code>
 */
class GoogleGAL_Service_DriveT_Resource_Replies extends GoogleGAL_Service_Resource
{
	/**
	 * Creates a new reply to a comment. (replies.create)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $commentId The ID of the comment.
	 * @param GoogleGAL_Service_DriveT_Reply $postBody
	 * @param array $optParams Optional parameters.
	 * @return GoogleGAL_Service_DriveT_Reply
	 */
	public function create($fileId, $commentId, GoogleGAL_Service_DriveT_Reply $postBody, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'commentId' => $commentId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('create', array($params), "GoogleGAL_Service_DriveT_Reply");
	}
	/**
	 * Deletes a reply. (replies.delete)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $commentId The ID of the comment.
	 * @param string $replyId The ID of the reply.
	 * @param array $optParams Optional parameters.
	 */
	public function delete($fileId, $commentId, $replyId, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'commentId' => $commentId, 'replyId' => $replyId);
		$params = array_merge($params, $optParams);
		return $this->call('delete', array($params));
	}
	/**
	 * Gets a reply by ID. (replies.get)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $commentId The ID of the comment.
	 * @param string $replyId The ID of the reply.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool includeDeleted Whether to return deleted replies. Deleted
	 * replies will not include their original content.
	 * @return GoogleGAL_Service_DriveT_Reply
	 */
	public function get($fileId, $commentId, $replyId, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'commentId' => $commentId, 'replyId' => $replyId);
		$params = array_merge($params, $optParams);
		return $this->call('get', array($params), "GoogleGAL_Service_DriveT_Reply");
	}
	/**
	 * Lists a comment's replies. (replies.listReplies)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $commentId The ID of the comment.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool includeDeleted Whether to include deleted replies. Deleted
	 * replies will not include their original content.
	 * @opt_param int pageSize The maximum number of replies to return per page.
	 * @opt_param string pageToken The token for continuing a previous list request
	 * on the next page. This should be set to the value of 'nextPageToken' from the
	 * previous response.
	 * @return GoogleGAL_Service_DriveT_ReplyList
	 */
	public function listReplies($fileId, $commentId, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'commentId' => $commentId);
		$params = array_merge($params, $optParams);
		return $this->call('list', array($params), "GoogleGAL_Service_DriveT_ReplyList");
	}
	/**
	 * Updates a reply with patch semantics. (replies.update)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $commentId The ID of the comment.
	 * @param string $replyId The ID of the reply.
	 * @param GoogleGAL_Service_DriveT_Reply $postBody
	 * @param array $optParams Optional parameters.
	 * @return GoogleGAL_Service_DriveT_Reply
	 */
	public function update($fileId, $commentId, $replyId, GoogleGAL_Service_DriveT_Reply $postBody, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'commentId' => $commentId, 'replyId' => $replyId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('update', array($params), "GoogleGAL_Service_DriveT_Reply");
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * The "revisions" collection of methods.
 * Typical usage is:
 *  <code>
 *   $driveService = new GoogleGAL_Service_DriveT(...);
 *   $revisions = $driveService->revisions;
 *  </code>
 */
class GoogleGAL_Service_DriveT_Resource_Revisions extends GoogleGAL_Service_Resource
{
	/**
	 * Permanently deletes a revision. This method is only applicable to files with
	 * binary content in Drive. (revisions.delete)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $revisionId The ID of the revision.
	 * @param array $optParams Optional parameters.
	 */
	public function delete($fileId, $revisionId, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'revisionId' => $revisionId);
		$params = array_merge($params, $optParams);
		return $this->call('delete', array($params));
	}
	/**
	 * Gets a revision's metadata or content by ID. (revisions.get)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $revisionId The ID of the revision.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param bool acknowledgeAbuse Whether the user is acknowledging the risk
	 * of downloading known malware or other abusive files. This is only applicable
	 * when alt=media.
	 * @return GoogleGAL_Service_DriveT_Revision
	 */
	public function get($fileId, $revisionId, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'revisionId' => $revisionId);
		$params = array_merge($params, $optParams);
		return $this->call('get', array($params), "GoogleGAL_Service_DriveT_Revision");
	}
	/**
	 * Lists a file's revisions. (revisions.listRevisions)
	 *
	 * @param string $fileId The ID of the file.
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param int pageSize The maximum number of revisions to return per page.
	 * @opt_param string pageToken The token for continuing a previous list request
	 * on the next page. This should be set to the value of 'nextPageToken' from the
	 * previous response.
	 * @return GoogleGAL_Service_DriveT_RevisionList
	 */
	public function listRevisions($fileId, $optParams = array())
	{
		$params = array('fileId' => $fileId);
		$params = array_merge($params, $optParams);
		return $this->call('list', array($params), "GoogleGAL_Service_DriveT_RevisionList");
	}
	/**
	 * Updates a revision with patch semantics. (revisions.update)
	 *
	 * @param string $fileId The ID of the file.
	 * @param string $revisionId The ID of the revision.
	 * @param GoogleGAL_Service_DriveT_Revision $postBody
	 * @param array $optParams Optional parameters.
	 * @return GoogleGAL_Service_DriveT_Revision
	 */
	public function update($fileId, $revisionId, GoogleGAL_Service_DriveT_Revision $postBody, $optParams = array())
	{
		$params = array('fileId' => $fileId, 'revisionId' => $revisionId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('update', array($params), "GoogleGAL_Service_DriveT_Revision");
	}
}
  
/*
 * Copyright 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * The "teamdrives" collection of methods.
 * Typical usage is:
 *  <code>
 *   $driveService = new GoogleGAL_Service_DriveT(...);
 *   $teamdrives = $driveService->teamdrives;
 *  </code>
 */
class GoogleGAL_Service_DriveT_Resource_Teamdrives extends GoogleGAL_Service_Resource
{
	/**
	 * Creates a new Team Drive. (teamdrives.create)
	 *
	 * @param string $requestId An ID, such as a random UUID, which uniquely
	 * identifies this user's request for idempotent creation of a Team Drive. A
	 * repeated request by the same user and with the same request ID will avoid
	 * creating duplicates by attempting to create the same Team Drive. If the Team
	 * Drive already exists a 409 error will be returned.
	 * @param GoogleGAL_Service_DriveT_TeamDrive $postBody
	 * @param array $optParams Optional parameters.
	 * @return GoogleGAL_Service_DriveT_TeamDrive
	 */
	public function create($requestId, GoogleGAL_Service_DriveT_TeamDrive $postBody, $optParams = array())
	{
		$params = array('requestId' => $requestId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('create', array($params), "GoogleGAL_Service_DriveT_TeamDrive");
	}
	/**
	 * Permanently deletes a Team Drive for which the user is an organizer. The Team
	 * Drive cannot contain any untrashed items. (teamdrives.delete)
	 *
	 * @param string $teamDriveId The ID of the Team Drive
	 * @param array $optParams Optional parameters.
	 */
	public function delete($teamDriveId, $optParams = array())
	{
		$params = array('teamDriveId' => $teamDriveId);
		$params = array_merge($params, $optParams);
		return $this->call('delete', array($params));
	}
	/**
	 * Gets a Team Drive's metadata by ID. (teamdrives.get)
	 *
	 * @param string $teamDriveId The ID of the Team Drive
	 * @param array $optParams Optional parameters.
	 * @return GoogleGAL_Service_DriveT_TeamDrive
	 */
	public function get($teamDriveId, $optParams = array())
	{
		$params = array('teamDriveId' => $teamDriveId);
		$params = array_merge($params, $optParams);
		return $this->call('get', array($params), "GoogleGAL_Service_DriveT_TeamDrive");
	}
	/**
	 * Lists the user's Team Drives. (teamdrives.listTeamdrives)
	 *
	 * @param array $optParams Optional parameters.
	 *
	 * @opt_param int pageSize Maximum number of Team Drives to return.
	 * @opt_param string pageToken Page token for Team Drives.
	 * @return GoogleGAL_Service_DriveT_TeamDriveList
	 */
	public function listTeamdrives($optParams = array())
	{
		$params = array();
		$params = array_merge($params, $optParams);
		return $this->call('list', array($params), "GoogleGAL_Service_DriveT_TeamDriveList");
	}
	/**
	 * Updates a Team Drive's metadata (teamdrives.update)
	 *
	 * @param string $teamDriveId The ID of the Team Drive
	 * @param GoogleGAL_Service_DriveT_TeamDrive $postBody
	 * @param array $optParams Optional parameters.
	 * @return GoogleGAL_Service_DriveT_TeamDrive
	 */
	public function update($teamDriveId, GoogleGAL_Service_DriveT_TeamDrive $postBody, $optParams = array())
	{
		$params = array('teamDriveId' => $teamDriveId, 'postBody' => $postBody);
		$params = array_merge($params, $optParams);
		return $this->call('update', array($params), "GoogleGAL_Service_DriveT_TeamDrive");
	}
}
