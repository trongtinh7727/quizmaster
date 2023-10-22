$(document).ready(function() {
    function autoGrow(textareaElement) {
        // Reset the height
        $(textareaElement).css('height', 'auto');
        // Set the height based on the scrollHeight
        $(textareaElement).css('height', $(textareaElement)[0].scrollHeight + 'px');
    }

    // Add an input event listener to the textareas
    $('.create-quiz-question, .create-quiz-answer').each(function() {
        $(this).on('input', function() {
            autoGrow(this);
        });
        // Initialize each textarea with the correct height
        autoGrow(this);
    });

    // Add click event listern to add question button
    $('#add-question').click(function () {
        // Create a new question element
        const newQuestion = $(
            '<div class="create-quiz-card myCard mb-5">' +
                '<div class="delete-icon">' +
                    '<i class="fas fa-trash-alt"></i>' +
                '</div>' +

                '<textarea class="create-quiz-question w-100 p-3 mb-3 mt-1 border-0" placeholder="Type your question here" rows="1"></textarea>' +
                    '<div class="d-flex flex-column gap12 mx-3">' +
                        '<div class="d-flex align-items-center">' +
                            '<span class="fs-5 mx-2">A. </span>' +
                            '<textarea class="create-quiz-answer border-0 p-3 w-100" placeholder="Type answer options here" rows="1"></textarea>' +
                        '</div>' +
                        '<div class="d-flex align-items-center">' +
                            '<span class="fs-5 mx-2">B. </span>' +
                            '<textarea class="create-quiz-answer border-0 p-3 w-100" placeholder="Type answer options here" rows="1"></textarea>' +
                        '</div>' +
                        '<div class="d-flex align-items-center">' +
                            '<span class="fs-5 mx-2">C. </span>' +
                            '<textarea class="create-quiz-answer border-0 p-3 w-100" placeholder="Type answer options here" rows="1"></textarea>' +
                        '</div>' +
                        '<div class="d-flex align-items-center">' +
                            '<span class="fs-5 mx-2">D. </span>' +
                            '<textarea class="create-quiz-answer border-0 p-3 w-100" placeholder="Type answer options here" rows="1"></textarea>' +
                        '</div>' +
                '</div>' +
            '</div>');

        // Append the new question to the #question-creating container
        $('#question-creating').append(newQuestion);
    });

    // Add click event listener to the parent element (#question-creating) for dynamic elements
    $('#question-creating').on('click', '.delete-icon', function () {
        // Find the closest .create-quiz-card parent and remove it
        $(this).closest('.create-quiz-card').remove();
    });
});
