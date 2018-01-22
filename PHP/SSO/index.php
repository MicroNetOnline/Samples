<?php
  require_once('config.php');

  // build the redirect URI based on the current HTTP_HOST
  $redirect_uri = 'http://' . $_SERVER['HTTP_HOST'] . OAUTH_REDIRECT_URI;

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
    <div>
      <h1>Click the link below to login</h1>
      <div class="container">
        <a href="<?php echo OAUTH_AUTHORIZE_URI; ?>?client_id=<?php echo OAUTH_CLIENT_ID; ?>&response_type=code&redirect_uri=<?php echo $redirect_uri; ?>">
          Authenticate
        </a>
      </div>
    </div>
  </body>
</html>
