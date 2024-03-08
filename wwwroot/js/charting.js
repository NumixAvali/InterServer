let ctx = document.getElementById('historicChart').getContext('2d');

function timeConverter(unixTimestamp){
	let a = new Date(unixTimestamp * 1000);
	let months = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
	let year = a.getFullYear();
	let month = months[a.getMonth()];
	let date = a.getDate();
	let hour = a.getHours();
	let min = a.getMinutes();
	let sec = a.getSeconds();
	let time = date + ' ' + month + ' ' + year + ' ' + hour + ':' + min + ':' + sec ;
	return time;
}

function getData() {
	let data = {
		batterySoc: {
			labels: [],
			values: []
		}
	}
	
	try {
		$.ajax({
			type: "POST",
			url: "/api/v1/cache/timestamp-range",
			async: false,
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				timestampStart: 1709216936,
				timestampEnd: 1709319936,
				token: "a"
			}),
			success: function (response) {
				console.log(response);
				
				response.data.forEach((entry) => {
					data.batterySoc.labels.push(timeConverter(entry.timestamp))
					data.batterySoc.values.push(entry.data.batterySoc.value * entry.data.batterySoc.scale)
				})
				// data.push(response.data.data.loadVoltage.value * response.data.data.loadVoltage.scale)
			},
			error: function (error) {
				console.error("Error retrieving cache:", error);
				// Handle error appropriately
			}
		})
	} catch (error) {
		console.error("Error during synchronous AJAX calls:", error);
		// Handle errors gracefully
	}
	
	return data;
}


// console.log(getData())

// Define your data
let data = {
	labels: getData().batterySoc.labels,
	datasets: [{
		label: 'Battery level %',
		data: getData().batterySoc.values,
		backgroundColor: [
			'rgba(255, 99, 132, 0.2)',
			// 'rgba(54, 162, 235, 0.2)',
			// 'rgba(255, 206, 86, 0.2)',
			// 'rgba(75, 192, 192, 0.2)',
			// 'rgba(153, 102, 255, 0.2)',
			// 'rgba(255, 159, 64, 0.2)'
		],
		borderColor: [
			'rgba(255, 99, 132, 1)',
			// 'rgba(54, 162, 235, 1)',
			// 'rgba(255, 206, 86, 1)',
			// 'rgba(75, 192, 192, 1)',
			// 'rgba(153, 102, 255, 1)',
			// 'rgba(255, 159, 64, 1)'
		],
		borderWidth: 1
	}]
};

// Create the chart
let myChart = new Chart(ctx, {
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