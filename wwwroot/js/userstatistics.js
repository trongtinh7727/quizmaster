var ulc = $('#userLineChart')[0].getContext('2d');
// Define chart data for the User Line Chart with 12 months
var userLineChartData = {
    labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'], // Updated labels
    datasets: [{
        label: 'Number of Users',
        data: [10, 15, 20, 12, 18, 25, 22, 30, 28, 35, 32, 27],
        borderColor: '#1DD3B0',
        borderWidth: 2,
        fill: true
    }]
};
// Create the line chart for users using Chart.js
var userLineChart = new Chart(ulc, {
    type: 'line',
    data: userLineChartData,
    options: {
        responsive: true,
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});
