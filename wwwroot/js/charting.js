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

function getData() {
	let data = {
		batterySoc: {
			unitOfMeasurement:"",
			labels: [],
			values: []
		},
		dailyProduction: {
			unitOfMeasurement:"",
			labels: [],
			values: []
		},
		dailyBatteryCharge: {
			unitOfMeasurement:"",
			labels: [],
			values: []
		},
		dailyBatteryDischarge: {
			unitOfMeasurement:"",
			labels: [],
			values: []
		},
		dailyConsumption: {
			unitOfMeasurement:"",
			labels: [],
			values: []
		},
		
		totalBatteryCharge: {
			unitOfMeasurement:"",
			labels: [],
			values: []
		},
		totalBatteryDischarge: {
			unitOfMeasurement:"",
			labels: [],
			values: []
		},
		totalEnergyBought: {
			unitOfMeasurement:"",
			labels: [],
			values: []
		},
		totalEnergySold: {
			unitOfMeasurement:"",
			labels: [],
			values: []
		},
		totalProduction: {
			unitOfMeasurement:"",
			labels: [],
			values: []
		},
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
				// console.log(response);
				data.dailyConsumption.unitOfMeasurement = response.data[0].data.dailyLoadConsumption.unit
				data.dailyProduction.unitOfMeasurement = response.data[0].data.dailyProduction.unit
				data.batterySoc.unitOfMeasurement = response.data[0].data.batterySoc.unit
				data.dailyBatteryCharge.unitOfMeasurement = response.data[0].data.dailyBatteryCharge.unit
				data.dailyBatteryDischarge.unitOfMeasurement = response.data[0].data.dailyBatteryDischarge.unit
				
				data.totalProduction.unitOfMeasurement = response.data[0].data.totalProduction.unit
				data.totalBatteryCharge.unitOfMeasurement = response.data[0].data.totalBatteryCharge.unit
				data.totalBatteryDischarge.unitOfMeasurement = response.data[0].data.totalBatteryDischarge.unit
				data.totalEnergyBought.unitOfMeasurement = response.data[0].data.totalEnergyBought.unit
				data.totalEnergySold.unitOfMeasurement = response.data[0].data.totalEnergySold.unit
				
				response.data.forEach((entry) => {
					// Daily
					data.batterySoc.labels.push(timeConverter(entry.timestamp))
					data.batterySoc.values.push(entry.data.batterySoc.value * entry.data.batterySoc.scale)
					
					data.dailyProduction.labels.push(timeConverter(entry.timestamp))
					data.dailyProduction.values.push(entry.data.dailyProduction.value * entry.data.dailyProduction.scale)
					
					data.dailyBatteryCharge.labels.push(timeConverter(entry.timestamp))
					data.dailyBatteryCharge.values.push(entry.data.dailyBatteryCharge.value * entry.data.dailyBatteryCharge.scale)
					
					data.dailyBatteryDischarge.labels.push(timeConverter(entry.timestamp))
					data.dailyBatteryDischarge.values.push(entry.data.dailyBatteryDischarge.value * entry.data.dailyBatteryDischarge.scale)
					
					data.dailyConsumption.labels.push(timeConverter(entry.timestamp))
					data.dailyConsumption.values.push(entry.data.dailyLoadConsumption.value * entry.data.dailyLoadConsumption.scale)
					
					// Total
					data.totalProduction.labels.push(timeConverter(entry.timestamp))
					data.totalProduction.values.push(entry.data.totalProduction.value * entry.data.totalProduction.scale)
					
					data.totalEnergyBought.labels.push(timeConverter(entry.timestamp))
					data.totalEnergyBought.values.push(entry.data.totalEnergyBought.value * entry.data.totalEnergyBought.scale)
					
					data.totalEnergySold.labels.push(timeConverter(entry.timestamp))
					data.totalEnergySold.values.push(entry.data.totalEnergySold.value * entry.data.totalEnergySold.scale)
					
					data.totalBatteryCharge.labels.push(timeConverter(entry.timestamp))
					data.totalBatteryCharge.values.push(entry.data.totalBatteryCharge.value * entry.data.totalBatteryCharge.scale)
					
					data.totalBatteryDischarge.labels.push(timeConverter(entry.timestamp))
					data.totalBatteryDischarge.values.push(entry.data.totalBatteryDischarge.value * entry.data.totalBatteryDischarge.scale)
					
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


let requestData = getData()
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
		},
		{
			label: `Daily production ${requestData.dailyProduction.unitOfMeasurement}`,
			data: requestData.dailyProduction.values,
			backgroundColor: [
				// 'rgba(255, 99, 132, 0.2)',
				'rgba(54, 162, 235, 0.2)',
				// 'rgba(255, 206, 86, 0.2)',
				// 'rgba(75, 192, 192, 0.2)',
				// 'rgba(153, 102, 255, 0.2)',
				// 'rgba(255, 159, 64, 0.2)'
			],
			borderColor: [
				// 'rgba(255, 99, 132, 1)',
				'rgba(54, 162, 235, 1)',
				// 'rgba(255, 206, 86, 1)',
				// 'rgba(75, 192, 192, 1)',
				// 'rgba(153, 102, 255, 1)',
				// 'rgba(255, 159, 64, 1)'
			],
			borderWidth: 1
		},
		{
			label: `Daily battery charge ${requestData.dailyBatteryCharge.unitOfMeasurement}`,
			data: requestData.dailyBatteryCharge.values,
			backgroundColor: [
				// 'rgba(255, 99, 132, 0.2)',
				// 'rgba(54, 162, 235, 0.2)',
				'rgba(255, 206, 86, 0.2)',
				// 'rgba(75, 192, 192, 0.2)',
				// 'rgba(153, 102, 255, 0.2)',
				// 'rgba(255, 159, 64, 0.2)'
			],
			borderColor: [
				// 'rgba(255, 99, 132, 1)',
				// 'rgba(54, 162, 235, 1)',
				'rgba(255, 206, 86, 1)',
				// 'rgba(75, 192, 192, 1)',
				// 'rgba(153, 102, 255, 1)',
				// 'rgba(255, 159, 64, 1)'
			],
			borderWidth: 1
		},
		{
			label: `Daily battery discharge ${requestData.dailyBatteryDischarge.unitOfMeasurement}`,
			data: requestData.dailyBatteryDischarge.values,
			backgroundColor: [
				// 'rgba(255, 99, 132, 0.2)',
				// 'rgba(54, 162, 235, 0.2)',
				// 'rgba(255, 206, 86, 0.2)',
				'rgba(75, 192, 192, 0.2)',
				// 'rgba(153, 102, 255, 0.2)',
				// 'rgba(255, 159, 64, 0.2)'
			],
			borderColor: [
				// 'rgba(255, 99, 132, 1)',
				// 'rgba(54, 162, 235, 1)',
				// 'rgba(255, 206, 86, 1)',
				'rgba(75, 192, 192, 1)',
				// 'rgba(153, 102, 255, 1)',
				// 'rgba(255, 159, 64, 1)'
			],
			borderWidth: 1
		},
		{
			label: `Daily consumption ${requestData.dailyConsumption.unitOfMeasurement}`,
			data: requestData.dailyConsumption.values,
			backgroundColor: [
				// 'rgba(255, 99, 132, 0.2)',
				// 'rgba(54, 162, 235, 0.2)',
				// 'rgba(255, 206, 86, 0.2)',
				// 'rgba(75, 192, 192, 0.2)',
				'rgba(153, 102, 255, 0.2)',
				// 'rgba(255, 159, 64, 0.2)'
			],
			borderColor: [
				// 'rgba(255, 99, 132, 1)',
				// 'rgba(54, 162, 235, 1)',
				// 'rgba(255, 206, 86, 1)',
				// 'rgba(75, 192, 192, 1)',
				'rgba(153, 102, 255, 1)',
				// 'rgba(255, 159, 64, 1)'
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
		},
		{
			label: `Energy bought ${requestData.totalEnergyBought.unitOfMeasurement}`,
			data: requestData.totalEnergyBought.values,
			backgroundColor: [
				// 'rgba(255, 99, 132, 0.2)',
				'rgba(54, 162, 235, 0.2)',
				// 'rgba(255, 206, 86, 0.2)',
				// 'rgba(75, 192, 192, 0.2)',
				// 'rgba(153, 102, 255, 0.2)',
				// 'rgba(255, 159, 64, 0.2)'
			],
			borderColor: [
				// 'rgba(255, 99, 132, 1)',
				'rgba(54, 162, 235, 1)',
				// 'rgba(255, 206, 86, 1)',
				// 'rgba(75, 192, 192, 1)',
				// 'rgba(153, 102, 255, 1)',
				// 'rgba(255, 159, 64, 1)'
			],
			borderWidth: 1
		},
		{
			label: `Energy sold ${requestData.totalEnergySold.unitOfMeasurement}`,
			data: requestData.totalEnergySold.values,
			backgroundColor: [
				// 'rgba(255, 99, 132, 0.2)',
				// 'rgba(54, 162, 235, 0.2)',
				'rgba(255, 206, 86, 0.2)',
				// 'rgba(75, 192, 192, 0.2)',
				// 'rgba(153, 102, 255, 0.2)',
				// 'rgba(255, 159, 64, 0.2)'
			],
			borderColor: [
				// 'rgba(255, 99, 132, 1)',
				// 'rgba(54, 162, 235, 1)',
				'rgba(255, 206, 86, 1)',
				// 'rgba(75, 192, 192, 1)',
				// 'rgba(153, 102, 255, 1)',
				// 'rgba(255, 159, 64, 1)'
			],
			borderWidth: 1
		},
		{
			label: `Battery charge ${requestData.totalBatteryCharge.unitOfMeasurement}`,
			data: requestData.totalBatteryCharge.values,
			backgroundColor: [
				// 'rgba(255, 99, 132, 0.2)',
				// 'rgba(54, 162, 235, 0.2)',
				// 'rgba(255, 206, 86, 0.2)',
				'rgba(75, 192, 192, 0.2)',
				// 'rgba(153, 102, 255, 0.2)',
				// 'rgba(255, 159, 64, 0.2)'
			],
			borderColor: [
				// 'rgba(255, 99, 132, 1)',
				// 'rgba(54, 162, 235, 1)',
				// 'rgba(255, 206, 86, 1)',
				'rgba(75, 192, 192, 1)',
				// 'rgba(153, 102, 255, 1)',
				// 'rgba(255, 159, 64, 1)'
			],
			borderWidth: 1
		},
		{
			label: `Battery discharge ${requestData.totalBatteryDischarge.unitOfMeasurement}`,
			data: requestData.totalBatteryDischarge.values,
			backgroundColor: [
				// 'rgba(255, 99, 132, 0.2)',
				// 'rgba(54, 162, 235, 0.2)',
				// 'rgba(255, 206, 86, 0.2)',
				// 'rgba(75, 192, 192, 0.2)',
				'rgba(153, 102, 255, 0.2)',
				// 'rgba(255, 159, 64, 0.2)'
			],
			borderColor: [
				// 'rgba(255, 99, 132, 1)',
				// 'rgba(54, 162, 235, 1)',
				// 'rgba(255, 206, 86, 1)',
				// 'rgba(75, 192, 192, 1)',
				'rgba(153, 102, 255, 1)',
				// 'rgba(255, 159, 64, 1)'
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