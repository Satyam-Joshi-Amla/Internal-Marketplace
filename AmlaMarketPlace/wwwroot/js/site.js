// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



// Function to hide the success message after 10 seconds
setTimeout(function () {
    var messageElement = document.getElementById('orderPlacedMessage');
    if (messageElement) {
        messageElement.style.display = 'none';
    }
}, 10000); // 10 seconds (10000 milliseconds)