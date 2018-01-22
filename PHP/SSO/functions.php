<?php

/**
 * getAccessToken
 *
 * @param array $config - An array of configurations to use to fetch an
 *                        access token from GrowthZone.
 *
 * array(
 *   'url' => 'http://growthzoneapp.com/oauth/token', - the URI to get the token from.
 *   'code' => '....', - the code from the authorize call.
 *   'client_id' => '....', - your client id.
 *   'client_secrent' => '...', - your client secret.
 *   'redirect_uri' => 'http://localhost/login.php' - the URI you redirected to in the authorize call.
 * )
 *
 * @access public
 * @return string|NULL - json encoded object on success, NULL on failure.
 */
function getAccessToken($config = array()) {
  // config elements that are required
  $required = array(
    'url',
    'code',
    'client_id',
    'client_secret',
    'redirect_uri',
  );

  // minimal $config check, if we have extra elements, or too few we back out.
  // not 'real' validation of the elements.
  if ((count($required) !== count($config)) || (array_diff(array_keys($config), array_values($required)))) {
    return NULL;
  }

  $params = array(
		'grant_type' => 'authorization_code',
		'code' => $config['code'],
		'redirect_uri' => $config['redirect_uri'],
		'client_id' => $config['client_id'],
		'client_secret' => $config['client_secret'],
	);

  $ch = curl_init();

  curl_setopt($ch, CURLOPT_URL, $config['url']);
  curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
  curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($params));
  curl_setopt($ch, CURLOPT_POST, 1);

  $headers = array();
  $headers[] = "Content-Type: application/x-www-form-urlencoded";
  curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);

  $result = curl_exec($ch);

  // if curl gives us an error code, echo the error code and back out.
  if (curl_errno($ch)) {
    echo 'Error:' . curl_error($ch);
    return NULL;
  }

  curl_close ($ch);

  // we either have an error from the server
  // ie:
  // {
  //   "error":"invalid_client"
  // }
  //
  // or a valid JSON payload with the token details
  // ie:
  // {
  //   "access_token": "....",
  //   "token_type": "bearer",
  //   "expires_in": "...,
  //   "refresh_token": "..."
  // }
  return $result;
}

/**
 * getTenantClaims
 *
 * @param array $config - An array of configuration details to use to fetch user claims
 *                        from GrowthZone.
 * @access public
 * @return string|NULL - json encoded object with user claim details on success or NULL on failure.
 */
function getTenantClaims($config = array()) {
  // config elements that are required
  $required = array(
    'url',
    'token',
  );

  // minimal $config check, if we have extra elements, or too few we back out.
  // not 'real' validation of the elements.
  if ((count($required) !== count($config)) || (array_diff(array_keys($config), array_values($required)))) {
    return NULL;
  }

  $ch = curl_init();

  curl_setopt($ch, CURLOPT_URL, $config['url']);
  curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);

  $headers = array();
  $headers[] = "authorization: Bearer " . $config['token'];
  $headers[] = 'content-type: application/json';

  curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);

  $result = curl_exec($ch);

  // if curl gives us an error code, echo the error code and back out.
  if (curl_errno($ch)) {
    echo 'Error:' . curl_error($ch);
    return NULL;
  }

  curl_close ($ch);

  // we either have an error from the server
  // ie:
  // {
  //   "error":""
  // }
  //
  // or a valid JSON payload with the token details
  // ie:
  // {
  //  "Claims": [
  //   {"Name":"IsMultipleTenants","Value":"True"},
  //   {"Name":"ImageUrl","Value":"//www.gravatar.com/avatar/a3944037ffaa0e1c4b767235f87b81d1?s=40&"},
  //   {"Name":"LoginSource","Value":"1"},{"Name":"LoginType","Value":"1"},
  //   {"Name":"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name","Value":"Firstname Lastname"},
  //   {"Name":"FirstName","Value":"FirstName"},
  //   {"Name":"LastName","Value":"LastName"},
  //   {"Name":"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress","Value":"user.email@yourdomain.com"},
  //   {"Name":"EmailId","Value":""},
  //   {"Name":"LoginId","Value":""},
  //   {"Name":"ContactId","Value":""},
  //   {"Name":"TenantId","Value":""},
  //   {"Name":"TenantKey","Value":""},
  //   {"Name":"TenantName","Value":"LastName Dev"},
  //   {"Name":"RoleCount","Value":"1"},
  //   {"Name":"OrganizationId","Value":"4083895"},
  //   {"Name":"CurrentOrg","Value":"LastName Dev"},
  //   {"Name":"LoggedInUser.*","Value":"Full_Control"},
  //   {"Name":"Overall_System.*","Value":"Full_Control"},
  //   {"Name":"Username","Value":"user.email@yourdomain.com"},
  //   {"Name":"FeatureLevel","Value":"0"},
  //   {"Name":"iss","Value":"https://{BaseHost}"},
  //   {"Name":"aud","Value":""},
  //   {"Name":"exp","Value":"1519045139"},
  //   {"Name":"nbf","Value":"1516366739"},
  //   {"Name":".properties","Value":""}
  //   ]
  // }
  return $result;
}

/**
 * parse_claims_data
 *
 * Parse the data returned by the claims API endpoint.
 *
 * Data from the Claims API comes back as:
 *
 * array(
 *   0 => array(
 *     'name' => '...'
 *     'value' => '...'
 * )
 *
 * This function converts it to:
 *
 * array(
 *   'FirstName' => '...',
 *   ...
 * )
 *
 * @param array $data - Array of data to parse.
 * @access public
 * @return array - The data parsed into the format mentioned above.
 */
function parse_claims_data($data) {
  $parsed_data = array();

  if (!empty($data['Claims'])) {
    foreach ($data['Claims'] as $current_data) {
      if (!empty($current_data['Name']) && !empty($current_data['Value'])) {
        $parsed_data[$current_data['Name']] = $current_data['Value'];
      }
    }
  }
  return $parsed_data;
}

/**
 * getMysqlConnection
 *
 * Get a connection to the mysql server.
 *
 * @param array $params
 * @access public
 * @return object|FALSE - PDO connection object on success.  FALSE on failure.
 */
function getMysqlConnection($params = array()) {
  try {

    return new PDO($params['dsn'], $params['username'], $params['password']);

  } catch (Exception $e) {

    // failure
    return FALSE;
  }
}

/**
 * setSessionInformation
 *
 * Store the session information into the DB table.
 *
 * @param array $params
 *
 * array(
 *   'config' => array(
 *     'dsn' => dsn to connect with
 *     'username' => username to connect with
 *     'password' => password for username to connect with
 *   ),
 *   'data' => array(
 *     ...
 *   )
 * )
 *
 * @access public
 * @return boolean - TRUE on success, FALSE on failure.
 */
function setSessionInformation($params) {
  $dbh = getMysqlConnection(
    array(
      'dsn' => $params['config']['dsn'],
      'username' => $params['config']['username'],
      'password' => $params['config']['password'],
    )
  );

  if ($dbh) {
    $sth = $dbh->prepare('SELECT count(loginId) AS idCount FROM sessions WHERE loginId = ?');
    try {

      $sth->execute(
        array(
          $params['data']['loginId']
        )
      );

      $count = $sth->fetchAll(PDO::FETCH_ASSOC);
    } catch (Exception $e) {
      print $e->getMessage();
    }

    if ($count[0]['idCount'] > 0 ) {
      try {
        // UPDATE
        $sth = $dbh->prepare('UPDATE sessions SET tenantId = ?, emailId = ?, token=?, expires=?, firstName = ?, lastName = ? WHERE loginId = ?');
        $sth->execute(
          array(
            $params['data']['tenantId'],
            $params['data']['emailId'],
            $params['data']['token'],
            $params['data']['expires'],
            $params['data']['firstName'],
            $params['data']['lastName'],
            $params['data']['loginId'],
          )
        );
      } catch (Exception $e) {
        print $e->getMessage();
      }
    }
    else
    {
      try {
        // CREATE
        $sth = $dbh->prepare('INSERT INTO sessions (token, expires, loginId, firstName, lastName, emailId, tenantId, username) VALUES (?, ?, ?, ?, ?, ?, ?, ?)');
        $sth->execute(
          array(
            $params['data']['token'],
            $params['data']['expires'],
            $params['data']['loginId'],
            $params['data']['firstName'],
            $params['data']['lastName'],
            $params['data']['emailId'],
            $params['data']['tenantId'],
            $params['data']['username'],
          )
        );
      } catch (Exception $e) {
        print $e->getMessage();
      }
    }
    return TRUE;
  }

  return FALSE;
}

/**
 * getSessionInformation
 *
 * Get the information from the mysql DB about a session.
 *
 * @param array $params
 *
 * array(
 *   'config' => array(
 *     'dsn' => dsn to connect with
 *     'username' => username to connect with
 *     'password' => password for username to connect with
 *   ),
 *   'data' => array(
 *     'loginId' => '...'
 *   )
 * )
 *
 * @access public
 * @return array - Array of data on success, empty array on failure.
 */
function getSessionInformation($params) {
  $dbh = getMysqlConnection(
    array(
      'dsn' => $params['config']['dsn'],
      'username' => $params['config']['username'],
      'password' => $params['config']['password'],
    )
  );

  if ($dbh) {
    try {
    $sth = $dbh->prepare('SELECT loginId, username, emailId, firstName, lastName, token, expires FROM sessions WHERE loginId = ? LIMIT 1');
    $sth->execute(array($params['data']['loginId']));

    return $sth->fetchAll(PDO::FETCH_ASSOC);

    } Catch (Exception $e) {
      print $e->getMessage();
    }
  }

  return array();
}

/**
 * deleteSessionInformation
 *
 * Deletes the sesssion information from the DB.
 *
 * @param array $params
 *
 * array(
 *   'config' => array(
 *     'dsn' => dsn to connect with
 *     'username' => username to connect with
 *     'password' => password for username to connect with
 *   ),
 *   'data' => array(
 *     'loginId' => '...'
 *   )
 * )
 *
 * @access public
 * @return boolean - TRUE on success, FALSE on failrue
 */
function deleteSessionInformation($params) {
  $dbh = getMysqlConnection(
    array(
      'dsn' => $params['config']['dsn'],
      'username' => $params['config']['username'],
      'password' => $params['config']['password'],
    )
  );

  if ($dbh) {
    try {
    $sth = $dbh->prepare('DELETE FROM sessions WHERE loginId = ? LIMIT 1');
    $sth->execute(array($params['data']['loginId']));

    return ($sth->rowCount() >= 1) ? TRUE : FALSE;

    } Catch (Exception $e) {
      print $e->getMessage();
    }
  }

  return array();
}

/**
 * verifyUserSession
 * 
 * Verify a user's session against the Claims API.
 * This is to verify that the session is in fact still valid.
 *
 * On success, 
 *   1) The token, is NOT expired in the database.
 *   2) The token, returns a valid payload and NOT an authentication error.
 *   3) The loginId the Claims API matches what is in the DB.
 *
 * @param mixed $params
 * @access public
 * @return boolean - TRUE if the user is authenticated properly.  FALSE otherwise.
 */
function verifyUserSession($params) {

  $tenantInfo = getSessionInformation($params);

  if (!empty($tenantInfo)) {
    if (!empty($tenantInfo[0]['token']) && !empty($tenantInfo[0]['expires'])) {
      if (time() < $tenantInfo[0]['expires']) {
        $claims_payload = json_decode(
          getTenantClaims(
            array(
              'url' => CLAIMS_API_URI,
              'token' => $tenantInfo[0]['token'],
            )
          ), TRUE
        );

        if (!empty($claims_payload)) {
          $parsed_claims = parse_claims_data($claims_payload);
          if (!empty($tenantInfo[0]['loginId']) && !empty($parsed_claims['LoginId'])) {
            return (!strcmp($tenantInfo[0]['loginId'], $parsed_claims['LoginId'])) ? TRUE : FALSE;
          }
        }
      }
    }
  }

  return FALSE;
}

?>
