<?php
  session_start();

  require_once('config.php');
  require_once('functions.php');

?>
<html>
  <head>
    <title>GrowthZone Oauth 2.0 Example</title>
    <style>
      .container {
        margin-left: 10px;
      }
    </style>
  </head>
<body>
  <?php if (!empty($_GET['code']) && empty($_SESSION['LoginId'])) : ?>
    <div class="container">
      <?php
        $payload = json_decode(
          getAccessToken(
            array(
              'url' => OAUTH_TOKEN_URI,
              'code' => $_GET['code'],
              'client_id' => OAUTH_CLIENT_ID,
              'client_secret' => OAUTH_CLIENT_SECRET,
              'redirect_uri' => 'http://' . $_SERVER['HTTP_HOST'] . OAUTH_REDIRECT_URI,
            )
          ), TRUE
        );
      ?>
      <h1>oauth/token (<?php echo OAUTH_TOKEN_URI; ?>) - returned payload</h1>
      <pre><?php
        if (!empty($payload)) {
          print_r($payload);
        }
        else
        {
          echo 'No authentication payload was returned.';
        }
        ?></pre>
        <pre><?php
          if (!empty($payload['access_token'])) {
            $claims_payload = json_decode(
                getTenantClaims(
                  array(
                    'url' => CLAIMS_API_URI,
                    'token' => $payload['access_token'],
                  )
                ), TRUE
            );

            if (!empty($claims_payload)) {
              $parsed_claims = parse_claims_data($claims_payload);

              // store the information in a session, the token only goes in the DB
              $_SESSION['EmailId'] = $parsed_claims['EmailId'];
              $_SESSION['FirstName'] = $parsed_claims['FirstName'];
              $_SESSION['LastName'] = $parsed_claims['LastName'];
              $_SESSION['TenantId'] = $parsed_claims['TenantId'];
              $_SESSION['LoginId'] = $parsed_claims['LoginId'];
              $_SESSION['Username'] = $parsed_claims['Username'];

              setSessionInformation(
                array(
                  'config' => array(
                    'dsn' => MYSQL_DSN,
                    'username' => MYSQL_USERNAME,
                    'password' => MYSQL_PASSWORD,
                  ),
                  'data' => array(
                    'loginId' => $parsed_claims['LoginId'],
                    'emailId' => $parsed_claims['EmailId'],
                    'tenantId' => $parsed_claims['TenantId'],
                    'firstName' => $parsed_claims['FirstName'],
                    'lastName' => $parsed_claims['LastName'],
                    'username' => $parsed_claims['Username'],
                    'token' => $payload['access_token'],
                    'expires' => time() + $payload['expires_in'],
                  )
                )
              );

              echo '<a href="protected.php">See protected resource</a>';
            }
            else
            {
              echo 'No claims payload was returned.';
            }
          }
          else
          {
            echo 'Unable to fetch claims without a token';
          }
        ?></pre>
    </div>
  <?php else: ?>
    <?php if (empty($_SESSION['LoginId'])): ?>
      <div>Sorry no code was returned.</div>
    <?php else: ?>
    <?php echo '<div><a href="protected.php">See protected resource</a></div>'; ?>
    <?php endif; ?>
  <?php endif; ?>
</body>
</html>
