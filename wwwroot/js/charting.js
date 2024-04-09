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
let dailyChart;
let totalChart;

radioButtons.forEach(button => {
	button.addEventListener('change', function() {
		if (this.checked) {
			timePeriodStr = this.nextElementSibling.textContent.trim();
			
			// Refresh charts with new user-selected data period
			updateCharts()
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
		case `all time`: return (Date.now() /1000 |0)-1; // This is bad...?
			// Later down the line this value gets subtracted from the current timestamp.
			// But passing current date to get 0 doesn't work for some reason, hence -1 part.
			// Basically it defaults to Jan 01 1970 00:00:01.
	}
}

async function getData() {
	const data = initializeData();
	
	try {
		const response = await sendRequest();
		populateData(response, data, timePeriodStr);
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

function populateData(response, data, timePeriod) {
	const dailyCategories = [
		'batterySoc',
		'dailyProduction',
		'dailyBatteryCharge',
		'dailyBatteryDischarge',
		'dailyLoadConsumption',
	];
	
	const totalCategories = [
		'totalProduction',
		'totalBatteryCharge',
		'totalBatteryDischarge',
		'totalEnergyBought',
		'totalEnergySold'
	];
	
	const timePeriodSeconds = strTimeToInt(timePeriod);
	const timeSpanSeconds = response.data[response.data.length - 1].timestamp - response.data[0].timestamp;
	const targetChunkCount = 200;
	const chunkSizeSeconds = Math.ceil(timeSpanSeconds / targetChunkCount);
	let currentChunkStart = response.data[0].timestamp;
	let currentChunkData = {};
	let currentChunkCount = 0;
	
	response.data.forEach((entry) => {
		if (entry.timestamp >= currentChunkStart + chunkSizeSeconds) {
			// Start a new chunk
			dailyCategories.forEach((category) => {
				data[category].labels.push(timeConverter(currentChunkStart));
				data[category].values.push(currentChunkData[category].length > 0
					? currentChunkData[category].reduce((a, b) => a + b, 0) / currentChunkData[category].length
					: null);
			});
			currentChunkStart = entry.timestamp;
			currentChunkData = {};
			currentChunkCount = 0;
		}
		
		dailyCategories.forEach((category) => {
			const categoryData = entry.data[category];
			if (!currentChunkData[category]) {
				currentChunkData[category] = [];
			}
			currentChunkData[category].push(categoryData.value * categoryData.scale);
		});
		
		currentChunkCount++;
	});
	
	// Push the last chunk of data for daily categories
	dailyCategories.forEach((category) => {
		data[category].labels.push(timeConverter(currentChunkStart));
		data[category].values.push(currentChunkData[category].length > 0
			? currentChunkData[category].reduce((a, b) => a + b, 0) / currentChunkData[category].length
			: null);
	});
	
	// Process total categories
	totalCategories.forEach((category) => {
		const categoryData = response.data[response.data.length - 1].data[category];
		data[category].unitOfMeasurement = categoryData.unit;
		data[category].labels = ['Latest'];
		data[category].values = [categoryData.value * categoryData.scale];
	});
}



async function updateCharts() {
	let requestData = await getData()
	// console.log((Date.now() /1000 |0)-strTimeToInt(timePeriodStr))

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
				borderWidth: 1,
				pointRadius: 1,
				hitRadius: 10
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
				borderWidth: 1,
				pointRadius: 1,
				hitRadius: 10
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
				borderWidth: 1,
				pointRadius: 1,
				hitRadius: 10
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
				borderWidth: 1,
				pointRadius: 1,
				hitRadius: 10
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
				borderWidth: 1,
				pointRadius: 1,
				hitRadius: 10
			}
		]
	};
	
	let dataTotal = {
		labels: [
			`Total production ${requestData.totalProduction.unitOfMeasurement}`,
			`Energy bought ${requestData.totalEnergyBought.unitOfMeasurement}`,
			`Energy sold ${requestData.totalEnergySold.unitOfMeasurement}`,
			`Battery charge ${requestData.totalBatteryCharge.unitOfMeasurement}`,
			`Battery discharge ${requestData.totalBatteryDischarge.unitOfMeasurement}`
		],
		datasets: [
			{
				label: 'Total Energy',
				minBarLength: 10,
				data: [
					requestData.totalProduction.values[requestData.totalProduction.values.length - 1],
					requestData.totalEnergyBought.values[requestData.totalEnergyBought.values.length - 1],
					requestData.totalEnergySold.values[requestData.totalEnergySold.values.length - 1],
					requestData.totalBatteryCharge.values[requestData.totalBatteryCharge.values.length - 1],
					requestData.totalBatteryDischarge.values[requestData.totalBatteryDischarge.values.length - 1],
				],
				backgroundColor: [
					'rgba(255, 99, 132, 0.2)',
					'rgba(54, 162, 235, 0.2)',
					'rgba(255, 206, 86, 0.2)',
					'rgba(75, 192, 192, 0.2)',
					'rgba(153, 102, 255, 0.2)',
				],
				borderColor: [
					'rgba(255, 99, 132, 1)',
					'rgba(54, 162, 235, 1)',
					'rgba(255, 206, 86, 1)',
					'rgba(75, 192, 192, 1)',
					'rgba(153, 102, 255, 1)',
				],
				borderWidth: 1,
			},
		]
	};


// Create charts if they don't exist, otherwise update them
	if (!dailyChart || !totalChart) {
		dailyChart = new Chart(dailyChartContext, {
			type: 'line',
			data: dataDaily,
			options: {
				scales: {
					y: {
						beginAtZero: false
					}
				},
				stepped: 'middle',
			}
		});
		
		totalChart = new Chart(totalChartContext, {
			type: 'bar',
			data: dataTotal,
			options: {
				scales: {
					y: {
						beginAtZero: true
					}
				}
			}
		});
	} else {
		updateChartData(dataDaily, dataTotal);
	}
}

function updateChartData(dataDaily, dataTotal) {
	dailyChart.data = dataDaily;
	dailyChart.update();
	
	totalChart.data = dataTotal;
	totalChart.update();
}

updateCharts();
