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
    $('.library-details-code-btn').trigger('click');
});