
$(document).ready(function () {
    
    $('.rateButton').click(function () {
        var clickedButton = $(this);
        $.ajax({
            url: '/Review/rateHelpfulness',
            dataType: "json",
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                reviewId: clickedButton.closest(".row").find('#reviewId').val(),
                rating: clickedButton.closest(".row").find('.hiddenRating').val(),
                authorId: clickedButton.closest(".row").find('#userId').val()
            }),
            async: true,
            processData: false,
            cache: false,
            success: function (data) {
                clickedButton.closest('.row').find('.averageRating').text(data);
            },
            error: function (xhr) {
                alert('error');
            }
        });
    });
    $('.star').click(function () {
        var id = $(this).attr('id');
        var rateNum = 0;
        if (id == "star1") {
            rateNum = 1;
        }
        if (id == "star2") {
            rateNum = 2;
        }
        if (id == "star3") {
            rateNum = 3;
        }
        if (id == "star4") {
            rateNum = 4;
        }
        if (id == "star5") {
            rateNum = 5;
        }
        $(this).closest('.ratingStars').find(".hiddenRating").val(rateNum);
        var count = 0;
        var stars = $(this).closest('.ratingStars').find('.star');
        while (count < 5) {
            var starId = "#star".concat(count);
            var currentStar = stars[4 - count];
            if (count < rateNum) {
                currentStar.textContent = "\u2605";
            }
            else {
                currentStar.textContent = "\u2606";
            }
            count++;
        }
    });
});
