<?php

session_start();

require_once('config.php');
require_once('functions.php');

if (!empty($_SESSION['LoginId'])) {
  deleteSessionInformation(
    array(
      'config' => array(
        'dsn' => MYSQL_DSN,
        'username' => MYSQL_USERNAME,
        'password' => MYSQL_PASSWORD,
      ),
      'data' => array(
        'loginId' => $_SESSION['LoginId'],
      )
    )
  );
}

session_destroy();

$_SESSION = [];

header('Location: index.php');
exit;

?>
