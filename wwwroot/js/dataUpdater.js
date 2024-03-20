function refreshBtnClick() {
	$('#currentData').addClass(`lds-dual-ring`)
		.html("")
	$.ajax({
		type: "GET",
		url: `${baseUrl}/api/v1/data/current`,
		success: function (reply) {
			// console.log(reply)
			$('#currentData').html(parseReplyToHtml(reply))
				.removeClass(`lds-dual-ring`)
			
		},
		error: function (error) {
			console.error(`Refresh button error!\n`,error)
			alert('Error updating UI.\nCheck the console for more info.')
		}
	});
}

function refreshDataOnLoad() {
	$.ajax({
		type: "GET",
		url: `${baseUrl}/api/v1/data/current`,
		success: function (reply) {
			$('#currentData').html(parseReplyToHtml(reply))
				.removeClass(`lds-dual-ring`)
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
