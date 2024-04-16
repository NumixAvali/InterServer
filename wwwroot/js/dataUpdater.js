// Battery charge %
let opts1 = {
	lines: 12,
	angle: -.2,
	lineWidth: 0.2,
	pointer: {
		length: 0.7,
		strokeWidth: 0.035,
		color: '#000000'
	},
	limitMax: false,
	limitMin: false,
	colorStart: '#6FADCF',
	colorStop: '#8FC0DA',
	strokeColor: '#E0E0E0',
	generateGradient: true,
	highDpiSupport: true,
	radiusScale: 0.6,
	staticLabels: {
		font: "14px sans-serif",
		labels: [0,25,50,75,100],
		color: '#000000',
		fractionDigits: 0,
	},
	renderTicks: {
		divisions: 4,
		divWidth: 1.1,
		divLength: 0.5,
		divColor: '#333333',
		subDivisions: 5,
		subLength: 0.3,
		subWidth: 0.6,
		subColor: '#666666'
	},
	staticZones: [
		{strokeStyle: "#F03E3E", min: 0, max: 10}, // Red from 100 to 130
		{strokeStyle: "#FFDD00", min: 10, max: 30}, // Yellow
		{strokeStyle: "#30B32D", min: 30, max: 100}, // Green
		// {strokeStyle: "#FFDD00", min: 220, max: 260}, // Yellow
		// {strokeStyle: "#F03E3E", min: 260, max: 300}  // Red
	],
};
// Battery usage kW
let opts2 = {
	lines: 12,
	angle: -.2,
	lineWidth: 0.2,
	pointer: {
		length: 0.7,
		strokeWidth: 0.035,
		color: '#000000'
	},
	limitMax: false,
	limitMin: false,
	colorStart: '#6FADCF',
	colorStop: '#8FC0DA',
	strokeColor: '#E0E0E0',
	generateGradient: true,
	highDpiSupport: true,
	radiusScale: 0.6,
	staticLabels: {
		font: "14px sans-serif",
		labels: [-6,-3,-1,0,1,3,6],
		color: '#000000',
		fractionDigits: 0,
	},
	renderTicks: {
		divisions: 4,
		divWidth: 1.1,
		divLength: 0.5,
		divColor: '#333333',
		subDivisions: 5,
		subLength: 0.3,
		subWidth: 0.6,
		subColor: '#666666'
	},
	staticZones: [
		{strokeStyle: "#30B32D", min: -6, max: -1}, // Red from 100 to 130
		{strokeStyle: "#FFDD00", min: -1, max: 1}, // Yellow
		{strokeStyle: "#F03E3E", min: 1, max: 6}, // Green
	],
};
// Production and Consumption
let opts3 = {
	lines: 12,
	angle: -.2,
	lineWidth: 0.2,
	pointer: {
		length: 0.7,
		strokeWidth: 0.035,
		color: '#000000'
	},
	limitMax: false,
	limitMin: false,
	colorStart: '#6FADCF',
	colorStop: '#8FC0DA',
	strokeColor: '#E0E0E0',
	generateGradient: true,
	highDpiSupport: true,
	radiusScale: 0.6,
	staticLabels: {
		font: "14px sans-serif",
		labels: [0,2,4,6,8],
		color: '#000000',
		fractionDigits: 0,
	},
	renderTicks: {
		divisions: 4,
		divWidth: 1.1,
		divLength: 0.5,
		divColor: '#333333',
		subDivisions: 5,
		subLength: 0.3,
		subWidth: 0.6,
		subColor: '#666666'
	},
};


let target1 = document.getElementById('gaugeCanvas1');
let target2 = document.getElementById('gaugeCanvas2');
let target3 = document.getElementById('gaugeCanvas3');
let target4 = document.getElementById('gaugeCanvas4');

// Battery charge, %
let gauge1 = new Gauge(target1).setOptions(opts1);
gauge1.maxValue = 100;
gauge1.animationSpeed = 64;
gauge1.setMinValue(0);
gauge1.set(0);

// Battery usage, kW
let gauge2 = new Gauge(target2).setOptions(opts2);
gauge2.maxValue = 6;
gauge2.animationSpeed = 64;
gauge2.setMinValue(-6);
gauge2.set(0);

// Current production, kW
let gauge3 = new Gauge(target3).setOptions(opts3);
gauge3.maxValue = 8;
gauge3.animationSpeed = 64;
gauge3.setMinValue(0);
gauge3.set(0);

// Current consumption, kW
let gauge4 = new Gauge(target4).setOptions(opts3);
gauge4.maxValue = 8;
gauge4.animationSpeed = 64;
gauge4.setMinValue(0);
gauge4.set(0);

const tableKeysEnum = {
	system: [
		'communicationBoardVersion',
		'controlBoardVersion',
		'usageTime',
		'workMode',
		'smartLoadEnableStatus',
		'alert',
		'acTemperature',
		'dcTemperature',
		'gridConnectedStatus',
		'genConnectedStatus',
		'batteryStatus'
	],
	battery: [
		'batteryCurrent',
		'batterySoc',
		'batteryPower',
		'dailyBatteryCharge',
		'dailyBatteryDischarge',
		'totalBatteryCharge',
		'totalBatteryDischarge'
	],
	load:[
		'loadL1Power',
		'loadL2Power',
		'pv1Current',
		'pv2Current',
		'pv1Voltage',
		'pv2Voltage',
		'pv1Power',
		'pv2Power'
	],
	inverter: [
		'inverterStatus',
		'inverterL1Power',
		'inverterL2Power',
		'internalL1LoadPower',
		'internalL2LoadPower'
	],
	grid: [
		'gridConnectedStatus',
		'gridFrequency',
		'gridL1Current',
		'gridL2Current',
		'gridL1Voltage',
		'gridL2Voltage'
	],
	daily: [
		'dailyLoadConsumption',
		'dailyProduction',
		'dailyBatteryCharge',
		'dailyBatteryDischarge',
		'dailyEnergyBought',
		'dailyEnergySold',
		'dailyBatteryCharge',
		'dailyBatteryDischarge'
	],
	total: [
		'totalProduction',
		'totalBatteryCharge',
		'totalBatteryDischarge',
		'totalEnergyBought',
		'totalEnergySold',
		'totalLoadConsumption',
		'totalBatteryCharge',
		'totalBatteryDischarge'
	],
};


function refreshBtnClick() {
	// Before reply
	$('#refreshBtn').prop('disabled',true)
		.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>\n' +
			'  Refreshing...')
	
	$('#currentDataSystem').html('')
		.addClass(`lds-dual-ring`)
	$('#currentDataBattery').html('')
		.addClass(`lds-dual-ring`)
	$('#currentDataLoad').html('')
		.addClass(`lds-dual-ring`)
	$('#currentDataInverter').html('')
		.addClass(`lds-dual-ring`)
	$('#currentDataGrid').html('')
		.addClass(`lds-dual-ring`)
	$('#currentDataDaily').html('')
		.addClass(`lds-dual-ring`)
	$('#currentDataTotal').html('')
		.addClass(`lds-dual-ring`)
	
	$('#currentDataAll').addClass(`lds-dual-ring`)
		.html("")
	
	
	$.ajax({
		type: "GET",
		url: `${baseUrl}/api/v1/data/current`,
		success: function (reply) {
			// After reply
			// console.log(reply)
			$('#currentDataSystem').html(parseReplyToHtmlKeyed(reply,tableKeysEnum.system))
				.removeClass(`lds-dual-ring`)
			$('#currentDataBattery').html(parseReplyToHtmlKeyed(reply,tableKeysEnum.battery))
				.removeClass(`lds-dual-ring`)
			$('#currentDataLoad').html(parseReplyToHtmlKeyed(reply,tableKeysEnum.load))
				.removeClass(`lds-dual-ring`)
			$('#currentDataInverter').html(parseReplyToHtmlKeyed(reply,tableKeysEnum.inverter))
				.removeClass(`lds-dual-ring`)
			$('#currentDataGrid').html(parseReplyToHtmlKeyed(reply,tableKeysEnum.grid))
				.removeClass(`lds-dual-ring`)
			$('#currentDataDaily').html(parseReplyToHtmlKeyed(reply,tableKeysEnum.daily))
				.removeClass(`lds-dual-ring`)
			$('#currentDataTotal').html(parseReplyToHtmlKeyed(reply,tableKeysEnum.total))
				.removeClass(`lds-dual-ring`)
			
			$('#currentDataAll').html(parseReplyToHtml(reply))
				.removeClass(`lds-dual-ring`)
			
			
			$('#refreshBtn').prop('disabled',false)
				.html('Refresh')
		},
		error: function (error) {
			console.error(`Refresh button error!\n`,error)
			alert('Error updating UI.\nCheck the console for more info.')
		}
	});
}

function refreshDataOnLoad() {
	$('#refreshBtn').prop('disabled',true)
		.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>\n' +
			'  Loading...')
	
	$.ajax({
		type: "GET",
		url: `${baseUrl}/api/v1/cache/latest`,
		success: function (reply) {
			$('#currentDataAll').html(parseReplyToHtml(reply.data))
				.removeClass(`lds-dual-ring`)
			$('#currentDataSystem').html(parseReplyToHtmlKeyed(reply.data,tableKeysEnum.system))
				.removeClass(`lds-dual-ring`)
			$('#currentDataBattery').html(parseReplyToHtmlKeyed(reply.data,tableKeysEnum.battery))
				.removeClass(`lds-dual-ring`)
			$('#currentDataLoad').html(parseReplyToHtmlKeyed(reply.data,tableKeysEnum.load))
				.removeClass(`lds-dual-ring`)
			$('#currentDataInverter').html(parseReplyToHtmlKeyed(reply.data,tableKeysEnum.inverter))
				.removeClass(`lds-dual-ring`)
			$('#currentDataGrid').html(parseReplyToHtmlKeyed(reply.data,tableKeysEnum.grid))
				.removeClass(`lds-dual-ring`)
			$('#currentDataDaily').html(parseReplyToHtmlKeyed(reply.data,tableKeysEnum.daily))
				.removeClass(`lds-dual-ring`)
			$('#currentDataTotal').html(parseReplyToHtmlKeyed(reply.data,tableKeysEnum.total))
				.removeClass(`lds-dual-ring`)
			
			
			$('#refreshBtn').prop('disabled',false)
				.html('Refresh')
			// console.log(reply)
		},
		error: function (error) {
			console.error(`Refresh data error!\n`,error)
			alert('Error updating UI.\nCheck the console for more info.')
		}
	});
}

function parseReplyToHtml(replyObj) {
	let htmlOutput = '<table class="tableDecor">';
	
	updateGauges(replyObj)
	// Iterate through the data object
	for (const [key, value] of Object.entries(replyObj.data)) {
		// console.log(key, value)
		htmlOutput += '<tr>';
		htmlOutput += `<td>${value.title}</td>`;
		htmlOutput += `<td>${value.value * value.scale}${value.unit}</td>`;
		htmlOutput += '</tr>';
	}	
	
	htmlOutput += '</table>';
	return htmlOutput;
}

function parseReplyToHtmlKeyed(replyObj,keysArr) {
	let htmlOutput = '<table class="tableDecor">';
	
	// Iterate through the data object
	for (const [key, value] of Object.entries(replyObj.data)) {
		if (keysArr.includes(key)) {
			htmlOutput += '<tr>';
			htmlOutput += `<td>${value.title}</td>`;
			htmlOutput += `<td>${value.value * value.scale}${value.unit}</td>`;
			htmlOutput += '</tr>';
		}
	}
	
	htmlOutput += '</table>';
	return htmlOutput;
}


function updateGauges(valuesObj) {
	let label1 = document.getElementById('gauge-value-label1');
	let label2 = document.getElementById('gauge-value-label2');
	let label3 = document.getElementById('gauge-value-label3');
	let label4 = document.getElementById('gauge-value-label4');
	
	// console.log(valuesObj)
	
	$('#gauge-value-label1').html(`${valuesObj.data.batterySoc.value}`+`${valuesObj.data.batterySoc.unit}`)
	$('#gauge-value-label2').html(`${valuesObj.data.batteryPower.value}`+`${valuesObj.data.batteryPower.unit}`)
	$('#gauge-value-label3').html(`${valuesObj.data.pv1Power.value+valuesObj.data.pv2Power.value}`+`${valuesObj.data.pv1Power.unit}`)
	$('#gauge-value-label4').html(`${valuesObj.data.loadL1Power.value+valuesObj.data.loadL2Power.value}`+`${valuesObj.data.loadL1Power.unit}`)
	
	gauge1.set(valuesObj.data.batterySoc.value)
	gauge2.set(valuesObj.data.batteryPower.value/1000)
	gauge3.set((valuesObj.data.pv1Power.value+valuesObj.data.pv2Power.value)/1000)
	gauge4.set((valuesObj.data.loadL1Power.value+valuesObj.data.loadL2Power.value)/1000)
}



