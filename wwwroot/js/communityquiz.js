$(document).ready(function () {
    $(".community-quiz-level select").change(function () {
        // Get the selected option value
        var selectedValue = $(this).val();
        var circle = $(".circle");
        // Remove existing color classes
        $(this).removeClass("text-success text-warning text-danger");
        circle.removeClass("bg-success bg-warning bg-danger");

        // Add the appropriate color class based on the selected option
        switch (selectedValue) {
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
})