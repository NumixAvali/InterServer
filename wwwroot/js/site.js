// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

const scrollToTopBtn = document.getElementById("scrollToTopBtn");
const darkModeToggle = document.querySelector('#dark-mode-toggle');

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

darkModeToggle.addEventListener('change', () => {
    const newTheme = darkModeToggle.checked ? 'dark' : 'light';
    document.documentElement.setAttribute('data-bs-theme', newTheme);
});

$(document).ready(function () {
    // Handling click event on Settings Button
    $('#settingsButton').click(function () {
        // Show the Settings Modal
        $('#settingsModal').modal('show');
    });
    
    $('#saveSettingsButton').click(function() {
        
        let settings = {
            ConfigName: $('#configSelection').val() + '.yaml',
            SerialNumber: $('#serialNumberInput').val(),
            InverterIp: $('#ip-input').val(),
            InverterPort: $('#port-input').val(),
            DbName: $('#dbSettingsName').val(),
            DbPassword: $('#dbSettingsPass').val(),
            DbUsername: $('#dbSettingsUser').val(),
            DbIp: $('#dbSettingsIp').val(),
            EnableAutomaticDataGather: $('#workerProcessSwitch').prop('checked'),
            AutomaticGatherInterval: $('#gatherIntervalValue').val(),
            AutomaticGatherIntervalModifier: $('#gatherIntervalModifier').val(),
            pathPrefix: baseUrl.startsWith('/') ? baseUrl.substring(1) : baseUrl,
        };
        console.log(settings)
        
        $.ajax({
            url: baseUrl+"/internal/submit-settings",
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
    
    // Dark mode handling
    const savedTheme = Cookies.get('dark_mode_theme') || 'dark';
    document.documentElement.setAttribute('data-bs-theme', savedTheme);
    
    darkModeToggle.checked = savedTheme === 'dark';
    
    darkModeToggle.addEventListener('change', () => {
        const newTheme = darkModeToggle.checked ? 'dark' : 'light';
        document.documentElement.setAttribute('data-bs-theme', newTheme);
        
        Cookies.set('dark_mode_theme', newTheme);
    });
});
