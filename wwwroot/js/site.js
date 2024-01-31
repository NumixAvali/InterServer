// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function refreshBtnClick() {
    alert('Not implemented yet.');
    // $.ajax({
    //     type: "POST",
    //     url: "/ControllerName/ActionName",
    //     success: function (data) {
    //         // Handle the response from the server, e.g., update the UI.
    //     },
    //     error: function (error) {
    //         // Handle errors, if any.
    //     }
    // });
}

let scrollToTopBtn = document.getElementById("scrollToTopBtn");

// When the user scrolls down 200px from the top of the document, show the button
window.onscroll = function () {
    if (document.body.scrollTop > 200 || document.documentElement.scrollTop > 200) {
        scrollToTopBtn.style.opacity = "1";
    } else {
        scrollToTopBtn.style.opacity = "0";
    }
};

// When the button is clicked, scroll to the top of the document
scrollToTopBtn.addEventListener("click", function () {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
});
