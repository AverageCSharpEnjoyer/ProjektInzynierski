// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function allowDownload() {
    var x = document.getElementById("fileToDownload");
    if (x.style.display === "none") {
        x.style.display = "block";
    } 
}