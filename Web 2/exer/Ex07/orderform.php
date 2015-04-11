<! DOCTYPE html>
<html lang="en">
<head>
<title>Pizza Order</title>

<?php 
include 'header.php';
?>

<div id="container">
<h2>Order a Delicious Pizza!</h2>


<form name="OrderForm" action="orderprocess.php"  onsubmit="return validateForm();" method="post">

<p>Name:  <input type="text" name="customerName"  /></p>
<p>ID:  <input type="text"  name="customerID"  /></p>
<p> Check the size of Pizza you would like to order:<br />
<input type="radio" name = "pizzaSize" value = "P"/>Personal<br />
<input type="radio" name = "pizzaSize" value = "S"/>Small<br />
<input type="radio" name = "pizzaSize" value = "M" checked="true"/>Medium<br />
<input type="radio" name = "pizzaSize" value = "L"/>Large<br />
</p>
<p>
    Select the number of toppings you want on your pizza:<br/>
    <input type="radio" name="NoTopping" value="0">No Extra Toppings<br/>
    <input type="radio" name="One" value="2.00">One Topping<br/>
    <input type="radio" name="Two" value="3.00">Two Toppings<br/>
    <input type="radio" name="Three" value="3.75">Three Toppings<br/>
</p>
<p>

<input type="submit"  name="Submit"  value=" Send Form"  />

</p>
</form>
</div> 
<?php 
include 'footer.php';
?>
</body>
</html>