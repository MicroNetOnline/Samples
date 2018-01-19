<?php
  session_start();

  require_once('config.php');
  require_once('functions.php');
?>

<?php if (!empty($_SESSION)): ?>
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
    <div class="container">
      <?php
        if (!empty($_SESSION)) {

          $verification = verifyUserSession(
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

          if ($verification) {
            echo '<h1>Session Information</h1>';
            echo '<pre>';
            print_r($_SESSION);
            echo '</pre>';

            echo '<pre>';

            echo '</pre>';
            echo 'Hello, ' . $_SESSION['FirstName'] . ' ' . $_SESSION['LastName'];

            echo '<div style="margin-top: 20px;"><a href="logout.php">Logout</a></div>';
          }
          else
          {
            header('Location: logout.php');
            exit;
          }
        }
      ?>
    </div>
  </body>
</html>
<?php else:
  header('Location: index.php');
  exit;
endif; ?>
