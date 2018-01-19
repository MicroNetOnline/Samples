<?php
  // This is the client id, this is called: API Key in the backoffice
  define('OAUTH_CLIENT_ID', '');

  // This is the client id, this is called: API Secret in the backoffice
  define('OAUTH_CLIENT_SECRET', '');

  // MySQL settings
  define('MYSQL_DSN', 'mysql:dbname=auth;host=127.0.0.1');
  define('MYSQL_USERNAME', 'root');
  define('MYSQL_PASSWORD', 'development');

  /****** DO NOT EDIT BELOW THIS LINE ******/
  // This is the return URI which we want oAuth to redirect to with our 'code'
  // which we later on exchange for a token.
  define('OAUTH_REDIRECT_URI', '/Samples/PHP/SSO/auth.php');

  // The base URI for the OAUTH API and Claims API - no trailing slash.
  define('OAUTH_BASE_URI', 'https://growthzoneapp.com');

  // This is the authorization URL to get an auth code, to exchange later on for a token.
  define('OAUTH_AUTHORIZE_URI', OAUTH_BASE_URI . '/oauth/authorize');

  // The URI to fetch the token from
  define('OAUTH_TOKEN_URI', OAUTH_BASE_URI . '/oauth/token');

  // The URI to fetch the tenant claims from
  define('CLAIMS_API_URI', OAUTH_BASE_URI . '/api/contacts/root/claims');
?>
