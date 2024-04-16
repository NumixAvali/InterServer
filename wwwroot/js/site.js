// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

const scrollToTopBtn = document.getElementById("scrollToTopBtn");

// When the user scrolls down 200px from the top of the document, show the button
window.onscroll = function () {
    if (document.body.scrollTop > 200 || document.documentElement.scrollTop > 200) {
        scrollToTopBtn.style.pointerEvents = "auto";
        scrollToTopBtn.style.opacity = "1";
    } else {
        scrollToTopBtn.style.pointerEvents = "none";
        scrollToTopBtn.style.opacity = "0";
    }
};

// When the button is clicked, scroll to the top of the document
scrollToTopBtn.addEventListener("click", function () {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
});

$(document).ready(function () {
    // Handling click event on Settings Button
    $('#settingsButton').click(function () {
        // Show the Settings Modal
        $('#settingsModal').modal('show');
    });
    
    $('#saveSettingsButton').click(function() {
        
        let settings= {
            ConfigName: $('#configSelection').val(),
            SerialNumber: $('#serialNumberInput').val(),
            DbName: $('#dbSettingsName').val(),
            DbPassword: $('#dbSettingsPass').val(),
            DbUsername: $('#dbSettingsUser').val(),
            DbIp: $('#dbSettingsIp').val(),
            EnableAutomaticDataGather: $('#workerProcessSwitch').val(),
            AutomaticGatherInterval: $('#gatherIntervalValue').val(),
            AutomaticGatherIntervalModifier: $('#gatherIntervalModifier').val(),
        }
        // console.log(settings)
        
        // TODO: finish making endpoint for that, or come up with other way to do it without endpoint.
        $.ajax({
            url: baseUrl,
            type: 'POST',
            data: JSON.stringify(settings),
            contentType: 'application/json',
            success: function(response) {
                // Handle success response
                console.log(response);
            },
            error: function(xhr, status, error) {
                alert("Error updating settings. Check console for more info.")
                console.error(error);
            }
        });
    });
    
    $('.modal').on('click', '.close', function(){
        $('#settingsModal').modal('hide');
    });
});
