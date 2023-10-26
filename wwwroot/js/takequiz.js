$(document).ready(function () {
    $('.take-quiz-answer').on('click', function () {
        var radioInput = $(this).find('input[type="radio"]');
        radioInput.prop('checked', true);
    });
    $('.take-quiz-clear').on('click', function () {
        $(this).siblings('.mt-3').find('input[type="radio"]').prop('checked', false);
    });
});