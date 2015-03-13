/*Ernesto Soltero*/
//New Javascript coding style attribute
/*jslint node: true */
"use strict";
var moveright = 0;


function jsStyle() {
// function to change style
// Change the color and the size of the font
// in the paragraph with id='text'
    //grab the object by id
    var ob;
    ob = document.getElementById("text");
    
    //add 10px to the left value
    moveright += 10;
    ob.style.left = moveright + 'px';
}

function howmany() {
    //Get an array of the number of elements that have input
    var inputs;
    inputs = document.getElementsByTagName("input");
    
    //Create an alert of the number of inputs.
    //I subtract two from the number of inputs because two extra buttons are 
    //also counted from the page but we don't want that since its not in the form.
    alert("The number of inputs in the form is " + (inputs.length - 2));
}

function colorchanger() {
    //get the element
    var dropdown, color, divToChange;
    dropdown = document.getElementById("mySelect");
    
    //Get the selected color
    color = dropdown.options[dropdown.selectedIndex].value;
    
    //Get the div to change
    divToChange = document.getElementById("change-div");
    divToChange.style.backgroundColor = color;
}

function init() {
    document.getElementById('rb').addEventListener("mouseover", mouseOver);
    document.getElementById('rb').addEventListener("mouseout", mouseOut);
}

function mouseOver() {
    var rb, color, dropdown;
    rb = document.getElementById("rb");
    //get the dropdown and color selected
    dropdown = document.getElementById("mySelect");
    color = dropdown.options[dropdown.selectedIndex].value;
    
    //Change the color based on what is selected
    rb.style.color = color;
}

function mouseOut() {
    var rb;
    //Get the element
    rb = document.getElementById("rb");
    
    //Change the color back to black
    rb.style.color = "black";
}

function changeTextNode() {
    var element, node;
    //Get the p to change
    element = document.getElementById("inner");
    
    //Create a text node
    node = document.createTextNode("I changed this by adding a text node!");
    
    //Remove the <p> element currently there
    while (element.firstChild) {
        element.removeChild(element.firstChild);
    }
    
    //Replace it with a text node
    element.appendChild(node);
}