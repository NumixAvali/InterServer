let ctx = document.getElementById('historicChart').getContext('2d');

function getData() {
	let data = [];
	
	try {
		$.ajax({
			type: "GET",
			url: "/api/v1/get-data",
			async: false,
			success: function (reply) {
				// console.log(reply)
				data.push(reply.data.loadVoltage.value * reply.data.loadVoltage.scale);
			},
			error: function (error) {
				console.error(`Refresh data (1) error!\n`, error);
				// Handle error appropriately
			}
		});
		
		$.ajax({
			type: "GET",
			url: "/api/v1/get-latest-cache",
			async: false,
			success: function (reply) {
				// console.log(reply)
				data.push(reply.data.data.loadVoltage.value * reply.data.data.loadVoltage.scale);
			},
			error: function (error) {
				console.error(`Refresh data (2) error!\n`, error);
				// Handle error appropriately
			}
		});
	} catch (error) {
		console.error("Error during synchronous AJAX calls:", error);
		// Handle errors gracefully
	}
	
	return data;
}


console.log(getData())

// Define your data
var data = {
	labels: ['Past', 'Present'],
	datasets: [{
		label: 'Battery level %',
		data: getData(),
		backgroundColor: [
			'rgba(255, 99, 132, 0.2)',
			'rgba(54, 162, 235, 0.2)',
			// 'rgba(255, 206, 86, 0.2)',
			// 'rgba(75, 192, 192, 0.2)',
			// 'rgba(153, 102, 255, 0.2)',
			// 'rgba(255, 159, 64, 0.2)'
		],
		borderColor: [
			'rgba(255, 99, 132, 1)',
			'rgba(54, 162, 235, 1)',
			// 'rgba(255, 206, 86, 1)',
			// 'rgba(75, 192, 192, 1)',
			// 'rgba(153, 102, 255, 1)',
			// 'rgba(255, 159, 64, 1)'
		],
		borderWidth: 1
	}]
};

// Create the chart
var myChart = new Chart(ctx, {
	type: 'line',
	data: data,
	options: {
		scales: {
			y: {
				beginAtZero: true
			}
		}
	}
});