<?php
	require_once 'config.php';
	$current = "landing";
	require_once 'header.php'; 
?>

<div class="container-fluid" style="padding-left: 0px;padding-right: 0px;">
	<div id="land">
		<!--Main page header-->
		<div class="row center-block landing-space" >
			<div class="col-md-12" style="margin-top:2%;">
				<h1 class="text-center">Job Matching. With Personality</h1>
			</div>
		</div>
		
		<!--Middle glyph description-->
		<div class="row text-center">
			<div class="col-md-4">
				<img src="../Images/icon1.png" style="height: 46px; width: 61px"/>
				<p>Connecting applicants and employers</p>
			</div>
			<div class="col-md-4">
				<img src="../Images/icon2.png" style="height: 57px; width: 68px"/>
				<p>Test your skills. Measure career progress.</p>
			</div>
			<div class="col-md-4">
				<img src="../Images/icon3.png" style="height: 56px; width: 60px"/>
				<p>Easy. Smart. Logical.</p>
			</div>
		</div>
		
		<!--Middle paragraph-->
		<div class="landing-space">
			<div class="col-md-12">
				<div id="wrapper-bottom-landing">
					<p>Join our community now to meet new people; share photos, videos
					 and music; create your own blog; post ads; chat online and more!</p>
				</div>
			</div>
		</div>
		
		<!--Last button-->
		<div class="landing-space">
			<div style="height: 40%;">
				<button type="submit" class="btn btn-info btn-lg center-block">Join Us!</button>
			</div>
		</div>
	</div>
</div>

<?php
	require_once 'footer.php';
?>
