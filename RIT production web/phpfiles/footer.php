<?php
	$CopyRight = "Copyright &copy Rochester Institute of Technology. All Rights Reserved. | Disclaimer | Copyright Infringement";
	require_once 'config.php';
?>
<div id="footer">
	<div id="top_footer">
		<div id="footer_title">
				<?php 
					echo MAIN_TITLE;
				?>
		</div>
		<table id="footer_menu">
			<tr>
				<td class="top_table" style="padding-right: 100px;" >
					<a href="home.php"><?php print $menu_options["home"]; ?> </a>
				</td>
				<td class="top_table" style="padding-right: 100px">
					<a href="#"><?php print $menu_options["grants"]; ?> </a>
				</td>
				<td class="top_table">
					<a href="contact-us.php"><?php print $menu_options["contact"]; ?></a>
				</td>
			</tr>
			<tr>
				<td class="bottom_table">
					<a href="#"><?php print $menu_options["about"]; ?></a>
				</td>
				<td class="bottom_table">
					<a href="#"><?php print $menu_options["cap"]; ?></a>
				</td>
			</tr>
		</table>
	</div>
	<div id="bottom_footer">
		<div id="bottom_footer_block">
			<div id="RIT_logo">
				<img alt="Rit" src=<?php print IMAGES . "RIT_foot.gif"; ?> />
				<span id="RIT_address">
					Rochester Institute of Technology <br />
					1 Lomb Memorial Drive,<br />
					Rochester, NY 14623
				</span>
			</div>
			<div id="copyright">
					<?php echo $CopyRight; ?>
			</div>
		</div>
	</div>
</div>
