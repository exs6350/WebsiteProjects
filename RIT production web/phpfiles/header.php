<!DOCTYPE html>
<html>
		<head>
			<meta http-equiv="content-type" content="text/html; charset=UTF-8">
			<script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
  			<script src="//code.jquery.com/ui/1.10.4/jquery-ui.js"></script>
  			<link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.4/themes/smoothness/jquery-ui.css" />
			<script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.4/jquery-ui.min.js"></script>
			<script src="/ritprod/javascript/functions.js"></script>
			<link rel="stylesheet" type="text/css" href= <?php print "http://" . $_SERVER['HTTP_HOST'] . "/ritprod/CSS/header.css"; ?> />
			<link rel="stylesheet" type="text/css" href=<?php print "http://" . $_SERVER['HTTP_HOST'] . "/ritprod/CSS/Style.css"; ?> />
			<link rel="stylesheet" type="text/css" href=<?php print "http://" . $_SERVER['HTTP_HOST'] . "/ritprod/CSS/footer.css"; ?> />
			<!--[if lt IE 9]>
			<script src="javascript/html5shiv.min.js"></script>
			<![endif]-->
			<?php require_once 'config.php'; ?>
			<title><?= isset($PageTitle) ? $PageTitle: MAIN_TITLE ?></title>
		</head>
		<body>
			<div id="header_wrapper">
				<div id="RIT_background">
					<div id="RIT_block">
						<a href="http://www.rit.edu/" id="top_header_content_left">
							<span id="RIT_name">
							</span>
						</a>
						<div id="top_header_content_right">
							<a id="RIT_directories" href="http://www.rit.edu/directories">Directories</a>
							<div id="RIT_search">
								<form id="RIT_search_form">
									<input type="text" placeholder="Search RIT"/>
									<button title="Search"><i id="search_button"></i></button>
								</form>
							</div>
						</div>
					</div>
				</div>
				<div id="second_row_background">
					<div id="second_row_block">
						<div id="rectangle">
							<img alt="Resiliency" src=<?php print IMAGES . "Resiliency.gif"; ?> />
						</div>
						
						<div id="second_row_header">
								<?php print MAIN_TITLE; ?>
						</div>
					</div>
				</div>
					
				<div id="third_row_background">	
					
						<div id="header_menu">
							<ul id="menu_contents">
								<?php menu(); ?>
							</ul>
						</div>	
					<div id="third_row_block">		
						<div id="img_wrapper">
							<img alt="thumb1" src=<?php print IMAGES . "thumb1.gif"; ?> />
							<img alt="thumb2" src=<?php print IMAGES . "thumb2.gif"; ?> />
							<img alt="thumb3" src=<?php print IMAGES . "thumb3.gif"; ?> />
							<img alt="thumb4" src=<?php print IMAGES . "thumb4.gif"; ?> />
							<img alt="thumb5" src=<?php print IMAGES . "thumb5.gif"; ?> />
							<img alt="thumb6" src=<?php print IMAGES . "thumb6.gif"; ?> />
						</div>
					</div>
				</div>


			</div>
			
		</body>
</html>
