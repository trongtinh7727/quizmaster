$(document).ready(function () {
    function generateCode() {
        var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        var code = '';
        for (var i = 0; i < 8; i++) {
            code += characters.charAt(Math.floor(Math.random() * characters.length));
        }
        return code;
    }

    // Click the code field
    $('.library-details-code-btn').click(function () {
        var code = generateCode();
        $('.library-details-code-field-codefield').val(code);
    });

    $('.library-details-code-field-codefield').click(function () {
        $(this).select();
        document.execCommand('copy');
    });

    // Generate initial code
    /*    $('.library-details-code-btn').trigger('click');*/

    // When the Edit button is clicked in the detailModal
    $('[data-bs-target^="#editWarningModal"]').on('click', function () {
        var targetModalId = $(this).data('bs-target');
        var detailModalId = $(this).closest('.modal').attr('id');

        // Hide the detailModal
        $('#' + detailModalId).modal('hide');

        // Show the editWarningModal after a short delay to allow transition
        setTimeout(function () {
            $(targetModalId).modal('show');
        }, 500); // Adjust this timeout as needed
    });

    // When the Delete button is clicked in the detailModal
    $('[data-bs-target^="#deleteWarningModal"]').on('click', function () {
        var targetModalId = $(this).data('bs-target');
        var detailModalId = $(this).closest('.modal').attr('id');

        // Hide the detailModal
        $('#' + detailModalId).modal('hide');

        // Show the deleteWarningModal after a short delay to allow transition
        setTimeout(function () {
            $(targetModalId).modal('show');
        }, 500); // Adjust this timeout as needed
    });
});