$(document).ready(function() {
    function autoGrow(textareaElement) {
        // Reset the height
        $(textareaElement).css('height', 'auto');
        // Set the height based on the scrollHeight
        $(textareaElement).css('height', $(textareaElement)[0].scrollHeight + 'px');
    }

    // Add an input event listener to the textareas
    $('.create-quiz-question, .create-quiz-answer, .quiz-sum-input').each(function() {
        $(this).on('input', function() {
            autoGrow(this);
        });
        // Initialize each textarea with the correct height
        autoGrow(this);
    });

    // Add click event listern to add question button
    $('#add-question').click(function () {
        var questionIndex = $('.create-quiz-card').length;
        
        const newQuestion = $(
            `<div class="create-quiz-card myCard mb-5" data-question-index="0">
            <div class="delete-icon">
                <i class="fas fa-trash-alt"></i>
            </div>

            <textarea name="Questions[${questionIndex}].QuestionText" class="create-quiz-question w-100 p-3 mb-3 mt-1 border-0" placeholder="Type your question here" rows="1"></textarea>

            <div class="d-flex flex-column gap12 mx-3">
                <div class="d-flex align-items-center">
                    <span class="fs-5 mx-2">A. </span>
                    <textarea name="Questions[${questionIndex}].Answers[0].Content" class="create-quiz-answer border-0 p-4 w-100" placeholder="Type answer options here" rows="1"></textarea>
                </div>
                <div class="d-flex align-items-center">
                    <span class="fs-5 mx-2">B. </span>
                    <textarea name="Questions[${questionIndex}].Answers[1].Content" class="create-quiz-answer border-0 p-4 w-100" placeholder="Type answer options here" rows="1"></textarea>
                </div>
                <div class="d-flex align-items-center">
                    <span class="fs-5 mx-2">C. </span>
                    <textarea name="Questions[${questionIndex}].Answers[2].Content" class="create-quiz-answer border-0 p-4 w-100" placeholder="Type answer options here" rows="1"></textarea>
                </div>
                <div class="d-flex align-items-center">
                    <span class="fs-5 mx-2">D. </span>
                    <textarea name="Questions[${questionIndex}].Answers[3].Content" class="create-quiz-answer border-0 p-4 w-100" placeholder="Type answer options here" rows="1"></textarea>
                </div>
            </div>

            <div class="create-quiz-score mt-3">
                <label>Score: </label>
                <input type="number" min="0" name="Questions[${questionIndex}].Score" class="border-0 px-3 py-1 mx-2" placeholder="Enter question score"/>
            </div>

            <div class="create-quiz-correct-answer mt-3">
                <label>Correct answer: </label>
                <select name="Questions[${questionIndex}].CorrectAnswer" name="correct-answer" class="border-0 p-3">
                    <option value="a">A</option>
                    <option value="b">B</option>
                    <option value="c">C</option>
                    <option value="d">D</option>
                </select>
            </div>
        </div>
        `);

        // Append the new question to the #question-creating container
        $('#question-creating').append(newQuestion);
    });

    // Add click event listener to the parent element (#question-creating) for dynamic elements
    $('#question-creating').on('click', '.delete-icon', function () {
        // Find the closest .create-quiz-card parent and remove it
        $(this).closest('.create-quiz-card').remove();
    });

    $("#level-select select").change(function () {
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

    // Handle form submission
    $("#submit").click(function () {
        var quizTitle = $("#quizTitle").val();
        var questions = [];

        // Collect question data
        $(".create-quiz-card").each(function () {
            var questionText = $(this).find(".create-quiz-question").val();
            var answers = [];

            // Collect answer data for each question
            $(this).find(".create-quiz-answer").each(function () {
                answers.push($(this).val());
            });

            var correctAnswer = $(this).find("select[name='correct-answer']").val();

            var questionData = {
                questionText: questionText,
                answers: answers,
                correctAnswer: correctAnswer
            };

            questions.push(questionData);
        });

        // Create an object to hold the form data
        var formData = {
            quizTitle: quizTitle,
            questions: questions
        };

        // Send the formData to the server via an AJAX request or use it as needed
        console.log(formData);
    });
});
