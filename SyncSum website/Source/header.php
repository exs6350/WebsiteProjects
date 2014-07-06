<!DOCTYPE html>
<html>
	<head>
		<?php require_once 'config.php'; ?>
		<meta charset="utf-8">
		<meta http-equiv="X-UA-Compatible" content="IE=edge">
		<meta name="viewport" content="width=device-width, initial-scale=1">
		<link rel="stylesheet" href=<?php print CSS . "bootstrap.min.css"; ?>  type="text/css">
		<link rel="stylesheet" href=<?php print CSS . "header.css"; ?> type="text/css">
		<link rel="stylesheet" href=<?php print CSS . "footer.css"; ?> type="text/css">
		<?php switch($current){
			case "landing":
				print "<link rel='stylesheet' href='" . CSS . "landing.css' type='text/css'>"; 
			case "dashboard":
				print "<link rel='stylesheet' href='" . CSS . "dashboard.css' type='text/css'>"; 
		}?>
		<!--[if lt IE 9]>
			<script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
      		<script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
		<![endif]-->
		<!--If the title is not defined for the page will use default title.-->
		<title><?= isset($PageTitle) ? $PageTitle: MAIN_TITLE ?></title>
	</head>
	<body>
	
	<!--Need to check if the user is logged in. If user is logged in display the header with nav links
		otherwise display a generic header-->
	<div class="containter-fluid">
		<!--This will change depending on your authentication system-->
		<div id="header">
			
				<!--left side header-->
				<div id="title" class="col-md-3 col-md-offset-2">
					<img src=<?php print IMAGES . "logo.png"; ?> id="logo" class="img-responsive">
				</div>
				<?php if($current != 'landing'){
				//top nav bar after the user has logged in. You have to connect this to the authentication process this is only the view.-->
				print '<div id="navigation" class="nav col-md-5 col-md-offset-2">
					<div class="navbar-header">
						<ul class="list-inline">';
							//Make sure to set the server variable 'current' for every page so we know what our active page is
							foreach($menu_options as $item){
								if($current === $item){
									print "<li class='active'><a href=" . SOURCE . $item . "php" . ">$item</a></li>";
								}
								else{
									print "<li><a href=" . SOURCE . $item . "php" . ">$item</a></li>";
								}
							}
						print '</ul>
					</div>
					<img class="col-md-1 col-sm-1" src=' . IMAGES . 'inbox-white.gif />
					<img class="col-md-1 col-sm-1" src=' . IMAGES . 'profile.gif />
					<button class="col-md-2 col-sm-2 btn btn-primary btn-sm" type="submit">Logout</button>
				</div>';
			}
			else{
			//This is the landing page to log in the user-->
			print '<div id="login-form" class="col-md-6 col-md-offset-1">
				<div class="row">
					<form class="inline-form" id="login-form">
						<input type="text" class="col-md-2 col-sm-2" id="user" placeholder="Username">
						<input type="password" class="col-md-2 col-sm-2" id="password" placeholder="Password">
						<button class="col-md-2 col-sm-2 btn btn-primary btn-sm" style="margin-left:2%;" type="submit">Login</button>
						<button class="col-md-2 col-sm-2 btn btn-info btn-sm" style="margin-left:2%;" type="submit">Join</button>
					</form>
				</div>
				<div class="row">
					<label class="col-md-3"><input type="checkbox"> Remember Me</label>
					<label class="col-md-3"><a href="#">Forgot Password</a></label>
				</div>
				
				</div>
			</div>';
			} ?>
		</div>
	</div>

	<script src="//code.jquery.com/jquery-1.11.0.min.js"></script>
	<script src=<?php print JS . "bootstrap.min.js"; ?>></script>
	<script src=<?php print JS . "utility.js"; ?>></script>
	</body>
</html>