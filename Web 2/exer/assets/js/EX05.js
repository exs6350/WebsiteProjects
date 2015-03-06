/*Ernesto Soltero*/
//New Javascript coding style attribute
/*jslint node: true */
"use strict";

function jsStyle() {
// function to change style
// Change the color and the size of the font
// in the paragraph with id='text'
    //grab the object by id
    var ob;
    ob = document.getElementById("text");
    
    //Change the color
    ob.style.color = "red";
    //Change the size of the font
    ob.style.fontSize = "16px";
}

function getFormValues() {
// function to send first and last names
// to an 'alert' message.

    //Get the names first
    var first, last;
    first = document.getElementById("fname");
    last = document.getElementById("lname");
    
    //Send the names to an alert message
    alert("The name in the form is " + first.value + " " + last.value);
}

function getOptions() {
// function to display options
    //get the element
    var dropdown;
    dropdown = document.getElementById("mySelect");

    //Create an event to simulate the click
    if (document.createEvent) {
        var e = document.createEvent("MouseEvents");
        //initialize the event
        e.initMouseEvent("mousedown", true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
        //fire the mouse click
        dropdown[0].dispatchEvent(e);
    } 
    //else if the event has already been dispacthed then fire the mousedown to bring the menu back up 
    else if (element.fireEvent) {
        dropdown[0].fireEvent("onmousedown");
    }
}

function init() {
    document.getElementById('rb').addEventListener("mouseout", mouseOut);
    document.getElementById('rb').addEventListener("mouseover", mouseOver);
}

function mouseOver() {
// set the font color to red
    var rb;
    rb = document.getElementById("rb");
  
    //Change the color of rb on mouseover
    rb.style.color = "red";
}

function mouseOut() {
// set the font color to black
    var rb;
    rb = document.getElementById("rb");
    
    //Reverse it back to black
    rb.style.color = "black";
}

function multiply() {
    var expression1, expression2, answer;
    
    //Get the values entered
    expression1 = document.getElementById("firstoperand").value;
    expression2 = document.getElementById("secondoperand").value;
    
    //evaluate the answer
    answer = expression1 * expression2;
    
    //Send back the result
    document.getElementById("result").innerHTML = answer;
}

function divide() {
    var expression1, expression2, answer;
    
    //Get the values entered
    expression1 = document.getElementById("firstoperand").value;
    expression2 = document.getElementById("secondoperand").value;
    
    //evaluate the answer
    answer = expression1 / expression2;
    
    //Send back the result
    document.getElementById("result").innerHTML = answer;
}