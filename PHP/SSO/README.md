# PHP SSO Example

PHP example on how to achieve an SSO using oAuth 2.0 in GrowthZone.

## The example assumes a few things:

1.  PHP5.x, PHP7.x is used.
1.  PHP has both the Curl, and PDO (with mysql driver) enabled.
1.  MySQL 5.x

**Note**

This is provided as an example and should be not used as final product.

## Setup

To setup the example:

1.  Setup Tenant according to documentation provided by product team.
1.  git clone https://github.com/MicroNetOnline/Samples.git
1.  Copy or symlink the Samples directory to be the server's (nginx/apache) document root.
1.  Setup a MySQL database:
  1.  CREATE DATABASE auth
  1.  mysql -u root -p auth < sql/session.table.create.sql
1.  Edit config.php 
  1.  Change OAUTH_CLIENT_ID to be the client_id from the GrowthZone > Setup > Client Applications page.
  1.  Change OAUTH_CLIENT_SECRET to be the client_secret from the GrowthZone > Setup > Client Applications page.
  1.  Change MYSQL_DSN to match the proper database and hostname.
  1.  Change MYSQL_USERNAME to match the proper mysql connection username.
  1.  Change MYSQL_PASSWORD to match the proper mysql connection password.

## Usage

To use the example:

1.  Navigate to [SSO Example](http://<yourhost>/Samples/PHP/SSO)
1.  Click the Authenticate link.
1.  Enter Username and Password and click 'Log In'.
1.  Click the Grant button.
1.  Client is logged in.

## Code Flow Summary

The SSO authentication process, uses oAuth 2.0's *Authorization Code Grant Flow*.  This document is not meant as
an explanation of how oAuth 2.0 functions, for more information on that topic, please consult resources such as:

1. [oAuth.net](https://oauth.net/2/)
2. [auth0 - authorization code grant flow](https://auth0.com/docs/api-auth/tutorials/authorization-code-grant)

Here is the flow (which is *Authorization Code Grant Flow*) that the example follows.

1. Link user to get authorize code using [https://www.growthzoneapp.com/oauth/authorize](https://www.growthzoneapp.com/oauth/authorize)
1. URI to use as a redirect URI for authorize code to be parsed and exchanged for an access token.
1. Use the access token to obtain Claims from the Claims API to get information about the user.
1. Store the Claims information into a database table along with token information.
1. Use sessions database table along with Claims API to determine user's current authentication state based on current token.
