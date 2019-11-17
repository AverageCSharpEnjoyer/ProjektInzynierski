// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(".custom-file-input").on("change", function () {
    var fileName = $(this).val().split("\\").pop();
    $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
});

var text_max = 1000;
$('#count_message').html('0/' + text_max + ' znaków');

$('#text').keyup(function () {
    var text_length = $('#text').val().length;

    $('#count_message').html(text_length + '/' + text_max + ' znaków');
});