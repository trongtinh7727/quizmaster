$(document).ready(function () {
    $(".community-quiz-level select").change(function () {
        // Get the selected option value
        var selectedValue = $(this).val();
        var circle = $(".circle");
        // Remove existing color classes
        $(this).removeClass("text-dark text-warning text-danger");
        circle.removeClass("bg-dark bg-warning bg-danger");

        // Add the appropriate color class based on the selected option
        switch (selectedValue) {
            case "all":
                $(this).addClass("text-dark");
                circle.addClass("bg-dark");
                break;
            case "1":
                $(this).addClass("text-success");
                circle.addClass("bg-success");
                break;
            case "2":
                $(this).addClass("text-warning");
                circle.addClass("bg-warning");
                break;
            case "3":
                $(this).addClass("text-danger");
                circle.addClass("bg-danger");
                break;
            default:
            // If none of the above, you can add a default class here
        }
    });
    $('#difficultyFilter').change(function () {
        var selectedDifficulty = $(this).val();
        $('.col-12').each(function () {
            var itemDifficulty = $(this).data('difficulty').toString();
            if (selectedDifficulty === 'all') {
                $(this).show();
            } else {
                if (itemDifficulty === selectedDifficulty) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
            }
        });
    });
})