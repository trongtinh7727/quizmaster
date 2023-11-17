
var qlc = $('#quizLineChart')[0].getContext('2d');
// Define chart data
var quizLineChartData = {
    labels: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'],
    datasets: [{
        label: 'Number of Quizzes',
        data: [5, 7, 13, 2, 4, 10, 9],
        borderColor: '#1DD3B0',
        borderWidth: 2,
        fill: true
    }]
};
// Create the line chart using Chart.js
var quizLineChart = new Chart(qlc, {
    type: 'line',
    data: quizLineChartData,
    options: {
        responsive: true,
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});

// Quiz pie chart
// Get the canvas context
var qpc = $('#quizPieChart')[0].getContext('2d');

// Define chart data
var quizPieChartData = {
    labels: ['Easy', 'Medium', 'Hard'],
    datasets: [{
        data: [30, 20, 15],
        backgroundColor: ['green', 'yellow', 'red']
    }]
};

// Create the pie chart using Chart.js
var quizPieChart = new Chart(qpc, {
    type: 'pie',
    data: quizPieChartData
});