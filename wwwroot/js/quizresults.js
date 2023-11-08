$(document).ready(function () {
    var tagClasses = ['left-tag-1', 'left-tag-2', 'left-tag-3'];
    var currentIndex = 0;

    $('.results-container-item').each(function (index) {
        // Add the class based on the currentIndex
        $(this).addClass(tagClasses[currentIndex]);

        // Increment the currentIndex and reset if it reaches the end of the tagClasses array
        currentIndex = (currentIndex + 1) % tagClasses.length;
    });
});
