function refreshBtnClick() {
	// alert('Not implemented yet.');
	$.ajax({
		type: "GET",
		url: "/api/v1/get-data",
		success: function (reply) {
			$('#currentData').html(JSON.stringify(reply))
			console.log(reply)
		},
		error: function (error) {
			console.error(error)
			alert('Error updating UI.\nCheck the console for more info.')
		}
	});
}

function refreshDataOnLoad() {
	$.ajax({
		type: "GET",
		url: "/api/v1/get-data",
		success: function (reply) {
			$('#currentData').html(JSON.stringify(reply))
				.removeClass(`lds-dual-ring`)
			console.log(reply)
		},
		error: function (error) {
			console.error(error)
			alert('Error updating UI.\nCheck the console for more info.')
		}
	});
}
