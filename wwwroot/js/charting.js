let dailyChartContext = document.getElementById('historicChartDaily').getContext('2d');
let totalChartContext = document.getElementById('historicChartTotal').getContext('2d');

function timeConverter(unixTimestamp, useYear = false, useMonth = false){
	//TODO: Make month label optional
	let a = new Date(unixTimestamp * 1000);
	let months = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
	let year = useYear? a.getFullYear() : '';
	let month = months[a.getMonth()];
	let date = a.getDate();
	let hour = a.getHours();
	let min = a.getMinutes();
	let sec = a.getSeconds();
	let time = date + ' ' + month + ' ' + year + ' ' + hour + ':' + min + ':' + sec ;
	return time;
}

const radioButtons = document.querySelectorAll('.btn-check');
let timePeriodStr = `Week`;

radioButtons.forEach(button => {
	button.addEventListener('change', function() {
		if (this.checked) {
			const selectedValue = this.nextElementSibling.textContent.trim();
			// Perform action based on the selected radio button value
			console.log(`Selected value: ${selectedValue}`);
			timePeriodStr = selectedValue;
			// Example: You can trigger functions or update content based on the selected value
			// For demonstration purpose, let's log the selected value to the console
		}
	});
});

function strTimeToInt(string) {
	// day - 86400
	// week - 604800
	// month - 2592000
	// year - 31536000
	
	switch (string.toLowerCase()) {
		case `day`: return 86400;
		case `week`: return 604800;
		case `month`: return 2592000;
		case `year`: return 31536000;
	}
}

async function getData() {
	const data = initializeData();
	
	try {
		const response = await sendRequest();
		populateData(response, data);
	} catch (error) {
		console.error("Error during AJAX call:", error);
	}
	
	return data;
}

function initializeData() {
	return {
		batterySoc: initializeCategory(),
		dailyProduction: initializeCategory(),
		dailyBatteryCharge: initializeCategory(),
		dailyBatteryDischarge: initializeCategory(),
		dailyLoadConsumption: initializeCategory(),
		totalBatteryCharge: initializeCategory(),
		totalBatteryDischarge: initializeCategory(),
		totalEnergyBought: initializeCategory(),
		totalEnergySold: initializeCategory(),
		totalProduction: initializeCategory(),
	};
}

function initializeCategory() {
	return {
		unitOfMeasurement: "",
		labels: [],
		values: []
	};
}

function sendRequest() {
	return new Promise((resolve, reject) => {
		$.ajax({
			type: "POST",
			url: `${baseUrl}/api/v1/cache/timestamp-range`,
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				timestampStart: (Date.now() /1000 |0)-strTimeToInt(timePeriodStr),
				timestampEnd: Date.now() /1000 |0, // Gets current timestamp
				token: "a"
			}),
			success: resolve,
			error: reject
		});
	});
}

function populateData(response, data) {
	const categories = [
		'batterySoc',
		'dailyProduction',
		'dailyBatteryCharge',
		'dailyBatteryDischarge',
		'dailyLoadConsumption',
		'totalProduction',
		'totalBatteryCharge',
		'totalBatteryDischarge',
		'totalEnergyBought',
		'totalEnergySold'
	];
	
	response.data.forEach((entry) => {
		categories.forEach((category) => {
			const categoryData = response.data[0].data[category];
			data[category].unitOfMeasurement = categoryData.unit;
			data[category].labels.push(timeConverter(entry.timestamp));
			data[category].values.push(categoryData.value * categoryData.scale);
		});
	});
}


async function updateCharts() {
	let requestData = await getData()
	// console.log(requestData)

	// Define chart data
	let dataDaily = {
		labels: requestData.batterySoc.labels,
		datasets: [
			{
				label: `Battery level ${requestData.batterySoc.unitOfMeasurement}`,
				data: requestData.batterySoc.values,
				backgroundColor: [
					'rgba(255, 99, 132, 0.2)',
				],
				borderColor: [
					'rgba(255, 99, 132, 1)',
				],
				borderWidth: 1
			},
			{
				label: `Daily production ${requestData.dailyProduction.unitOfMeasurement}`,
				data: requestData.dailyProduction.values,
				backgroundColor: [
					'rgba(54, 162, 235, 0.2)',
				],
				borderColor: [
					'rgba(54, 162, 235, 1)',
				],
				borderWidth: 1
			},
			{
				label: `Daily battery charge ${requestData.dailyBatteryCharge.unitOfMeasurement}`,
				data: requestData.dailyBatteryCharge.values,
				backgroundColor: [
					'rgba(255, 206, 86, 0.2)',
				],
				borderColor: [
					'rgba(255, 206, 86, 1)',
				],
				borderWidth: 1
			},
			{
				label: `Daily battery discharge ${requestData.dailyBatteryDischarge.unitOfMeasurement}`,
				data: requestData.dailyBatteryDischarge.values,
				backgroundColor: [
					'rgba(75, 192, 192, 0.2)',
				],
				borderColor: [
					'rgba(75, 192, 192, 1)',
				],
				borderWidth: 1
			},
			{
				label: `Daily consumption ${requestData.dailyLoadConsumption?.unitOfMeasurement}`,
				data: requestData.dailyLoadConsumption.values,
				backgroundColor: [
					'rgba(153, 102, 255, 0.2)',
				],
				borderColor: [
					'rgba(153, 102, 255, 1)',
				],
				borderWidth: 1
			}
		]
	};
	
	let dataTotal = {
		labels: requestData.batterySoc.labels,
		datasets: [
			{
				label: `Total production ${requestData.totalProduction.unitOfMeasurement}`,
				data: requestData.totalProduction.values,
				backgroundColor: [
					'rgba(255, 99, 132, 0.2)',
				],
				borderColor: [
					'rgba(255, 99, 132, 1)',
				],
				borderWidth: 1
			},
			{
				label: `Energy bought ${requestData.totalEnergyBought.unitOfMeasurement}`,
				data: requestData.totalEnergyBought.values,
				backgroundColor: [
					// 'rgba(255, 99, 132, 0.2)',
					'rgba(54, 162, 235, 0.2)',
				],
				borderColor: [
					// 'rgba(255, 99, 132, 1)',
					'rgba(54, 162, 235, 1)',
				],
				borderWidth: 1
			},
			{
				label: `Energy sold ${requestData.totalEnergySold.unitOfMeasurement}`,
				data: requestData.totalEnergySold.values,
				backgroundColor: [
					'rgba(255, 206, 86, 0.2)',
				],
				borderColor: [
					'rgba(255, 206, 86, 1)',
				],
				borderWidth: 1
			},
			{
				label: `Battery charge ${requestData.totalBatteryCharge.unitOfMeasurement}`,
				data: requestData.totalBatteryCharge.values,
				backgroundColor: [
					'rgba(75, 192, 192, 0.2)',
				],
				borderColor: [
					'rgba(75, 192, 192, 1)',
				],
				borderWidth: 1
			},
			{
				label: `Battery discharge ${requestData.totalBatteryDischarge.unitOfMeasurement}`,
				data: requestData.totalBatteryDischarge.values,
				backgroundColor: [
					'rgba(153, 102, 255, 0.2)',
				],
				borderColor: [
					'rgba(153, 102, 255, 1)',
				],
				borderWidth: 1
			}
		]
	};


// Create charts
	let dailyChart = new Chart(dailyChartContext, {
		type: 'line',
		data: dataDaily,
		options: {
			scales: {
				y: {
					beginAtZero: true
				}
			}
		}
	});
	
	let totalChart = new Chart(totalChartContext, {
		type: 'line',
		data: dataTotal,
		options: {
			scales: {
				y: {
					beginAtZero: false
				}
			}
		}
	});
}

updateCharts();
