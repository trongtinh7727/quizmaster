$(document).ready(function () {
    $('#quizzes-table').DataTable({
        responsive: true
    });
    $('#user-table').DataTable({
        responsive: true
    });

    // Nav link
    $('.nav-link').on('click', function () {
        $('.nav-link').removeClass('active');
        $(this).addClass('active');
    });

    // Toggle the side navigation
    const sidebarToggle = $('#sidebarToggle');
    if (sidebarToggle.length) {
        if (localStorage.getItem('sb|sidebar-toggle') === 'true') {
            $('body').toggleClass('sb-sidenav-toggled');
        }
        sidebarToggle.on('click', function (event) {
            event.preventDefault();
            $('body').toggleClass('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', $('body').hasClass('sb-sidenav-toggled'));
        });
    }
});
