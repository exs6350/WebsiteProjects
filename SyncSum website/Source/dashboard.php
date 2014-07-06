<?php 
require_once 'config.php';
$PageTitle = "Dashboard";
$current = "dashboard";
require_once 'header.php';
?>

<div class="container-fluid">
	<div class="row">
		<div class="col-md-3 col-md-offset-1" id="left-content">
			<div id="main-header">
				<!--Need a function to pull name and insert here-->
				<h3>Greetings. Kevin Lewis</h3>
			</div>
			<div class="img-thumbnail">
				<img src=<?php print IMAGES . "kevin.jpg"; ?> alt="Profile picture"/>
			</div>	
			<div class="profile-links">
				<span><img src=<?php print IMAGES . "example-icon.gif"; ?> alt="Icon"/> <h4>Edit Profile</h4></span>
				<span><img src=<?php print IMAGES . "inbox-icon.gif"; ?> alt="Icon"/> <h4>Inbox</h4></span>
				<span><img src=<?php print IMAGES . "files-icon.gif"; ?> alt="Icon"/> <h4>Files</h4></span>
			</div>
			<div class="profile-options">
				<div><h3><a id="profile-activate">Profile</a></h3></div>
				<div id="profile-dropdown">
					<div id="profile-status">
						<h3 style="color:rgb(114, 114, 114);">Profile Status:</h3>
						<h4 style="color:rgb(209, 201, 77);">Pending Approval</h4>
					</div>
					<div id="section-status">
						<div><h3>Email</h3></div>
						<div><h3>Membership</h3></div>
						<div><h3>Last Login</h3></div>
					</div>
				</div>
				<div><h3><a id="presence-activate">Presence</a></h3></div>
				<div id="presence-dropdown">
					
				</div>
				<div><h3><a id="content-activate">Content</a></h3></div>
				<div id="content-dropdown">
					
				</div>
			</div>
		</div>
		<!--This should be changed to a dynamic portion on updates for the user (ajax)-->
		<div class="col-md-4 col-md-offset-2">
			<div id="welcome">
				<div>
					<h2>Welcome!</h2>
					<p>If you haven't done so already, please go to your profile page to enter your
					information. If you are an employer , you can create a job postings there. If 
					you are an applicant, you can create a resume.</p>
					<p>Once you have this information entered, you can use the search page to locate
					possible applicants or positions that match your profile.</p>
				</div>
			</div>
			<!--Connect this to the server for "mail" functionality-->
			<div id="message-board">
				<div id="bot-header">
					<h2>Message Header</h2>
				</div>
				<div id="bottom-content">
					<p>Any kind of content or notification messages they may have being 
					updated on the dashboard.</p>
				</div>
			</div>
		</div>
	</div>
</div>

<?php
	require_once 'footer.php';
?>
