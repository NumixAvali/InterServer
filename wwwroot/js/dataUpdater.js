function refreshBtnClick() {
	$('#currentData').addClass(`lds-dual-ring`)
		.html("")
	$.ajax({
		type: "GET",
		url: "/api/v1/get-data",
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
		url: "/api/v1/get-data",
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
		htmlOutput += `<td class="rowDecor">${value.title}</td>`;
		htmlOutput += `<td class="rowDecor">${value.value * value.scale}${value.unit}</td>`;
		htmlOutput += '</tr>';
	}	
	
	htmlOutput += '</table>';
	return htmlOutput;
}
