"use strict";

$(document).ready(function(){
    $('li img').on('click',function(){
        var src = $(this).attr('src');
        var img = '<img src="' + src + '" class="modal-image"/>';
        
        $('#myModal').modal();
        $('#myModal').on('shown.bs.modal',function(){
            $('#myModal .modal-body').html(img);
        });
        $('#myModal').on('hidden.bs.modal', function(){
            $('#myModal .modal-body').html('');
        });
    });
})