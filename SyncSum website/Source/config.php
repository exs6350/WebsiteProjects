<?php
/*
 * Config file that holds global settings and utility functions. Any global
 variables should go here.
 */
 	//Paths may have to be changed depending on your folder structure
 	define("MAIN_TITLE", "SyncSum");
    define("IMAGES", "http://" . $_SERVER['HTTP_HOST'] . "/SyncSum/Images/");
	define("CSS", "http://" . $_SERVER['HTTP_HOST'] . "/SyncSum/css/");
	define("JS", "http://" . $_SERVER['HTTP_HOST'] . "/SyncSum/js/");
	define("SOURCE", "http://" . $_SERVER['HTTP_HOST'] . "/SyncSUm/Source/");
	$current = null;
	
	//Nav bar links
	$menu_options = array(
		'dash' => "Dashboard",
		'profile' => "Profile",
		'search' => "Search",
		'culture' => "Culture",
	);
	
	//Implement this to get picture or use a session to populate an object of user data
	function getPicture(){
		
	}
?>
