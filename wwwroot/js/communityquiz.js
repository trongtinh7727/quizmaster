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

    // When the Edit button is clicked in the detailModal
    $('[data-bs-target^="#rankingModal"]').on('click', function () {
        var targetModalId = $(this).data('bs-target');
        var detailModalId = $(this).closest('.modal').attr('id');

        // Hide the detailModal
        $('#' + detailModalId).modal('hide');

        // Show the rankingModal after a short delay to allow transition
        setTimeout(function () {
            $(targetModalId).modal('show');
        }, 500); // Adjust this timeout as needed
    });

    function searchQuizzes() {
        var searchQuery = $("#searchQuery").val();
        var difficultyFilter = $("#difficultyFilter").val();

        // Khởi tạo output trước khi bắt đầu vòng lặp

        $.ajax({
            url: '/APIs/GetCommunityQuiz',
            type: 'GET',
            data: {
                searchQuery: searchQuery,
                difficultyFilter: difficultyFilter,
                pageIndex: 1,
                pageSize: 16
            },
            success: function (response) {
                var output =""
                $.each(response.quizzes, function (index, quiz) {
                    var difficultyText = "";
                    if (quiz.level === 1) {
                        difficultyText = "Easy";
                    }
                    else if (quiz.level === 2) {
                        difficultyText = "Medium";
                    }
                    else if (quiz.level === 3) {
                        difficultyText = "Hard";
                    }

                    var difficultyClass = "";
                    if (quiz.level === 1) {
                        difficultyClass = "text-success";
                    }
                    else if (quiz.level === 2) {
                        difficultyClass = "text-warning";
                    }
                    else if (quiz.level === 3) {
                        difficultyClass = "text-danger";
                    }

                    var questionsNumText = "";
                    if (quiz.quizQuestions.length === 1) {
                        questionsNumText = "1 Question";
                    }
                    else {
                        questionsNumText = quiz.quizQuestions.length + " Questions";
                    }

                    output += 
                    `
                        <div class="col-12 col-xl-3 col-lg-4 col-md-6 col-sm-6 mb-4" data-difficulty="@quiz.Level" data-bs-toggle="modal" data-bs-target="#detailModal-@quiz.Id">
                            <div class="communityQuizCard">
                                <h5 class="community-quiz-title">${quiz.title}</h5>

                                <span class="community-quiz-summary text-gray-dark">${quiz.summary}</span>

                                <span class="community-quiz-questions">${questionsNumText}</span>

                                <span class="community-quiz-difficulty">
                                    Difficulty:
                                    <strong class="${difficultyClass}">
                                         ${difficultyText}
                                    </strong>
                                </span>
                            </div>
                        </div>
                    `
                });

                // Cập nhật #searchResults với chuỗi HTML hoàn chỉnh
                $("#searchResults").html(output);
            },
            error: function (xhr, status, error) {
                console.error("Error in AJAX request: " + error);
            }
        });
    }

    searchQuizzes();
    $("#searchQuery").on('input', function () {
        searchQuizzes();
    });


})