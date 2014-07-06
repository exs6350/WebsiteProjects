/**
 * @author Ernesto Soltero (ernesto.soltero11@gmail.com)
 */

$(document).ready(function(){
	//left side profile drop downs 
	$('#profile-activate').on('click', function(){
		$('#profile-dropdown').toggle();
	});
	
	$('#presence-activate').on('click', function(){
		$('#presence-dropdown').toggle();
	});
	
	$('#content-activate').on('click', function(){
		$('#content-dropdown').toggle();
	});
});
