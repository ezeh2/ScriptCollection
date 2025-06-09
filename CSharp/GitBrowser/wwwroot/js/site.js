// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).on('click', '.repo-link', function(e) {
    e.preventDefault();
    var path = $(this).data('path');
    $('#commits').empty();
    $.get('/Git/Branches', { repoPath: path }, function(data) {
        $('#branches').html(data);
    });
});

$(document).on('click', '.branch-link', function(e) {
    e.preventDefault();
    var path = $(this).data('path');
    var branch = $(this).data('branch');
    $.get('/Git/Log', { repoPath: path, branchName: branch }, function(data) {
        $('#commits').html(data);
    });
});




