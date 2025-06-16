// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).on('click', '.repo-link', function(e) {
    e.preventDefault();
    var path = $(this).data('path');
    $('#commits').addClass('commits-stale');
    $.get('/Git/Branches', { repoPath: path }, function(data) {
        $('#branches').html(data);
    });
});

$(document).on('click', '.branch-link', function(e) {
    e.preventDefault();
    var path = $(this).data('path');
    var branch = $(this).data('branch');
    $('#commits').removeClass('commits-stale');
    $.get('/Git/Log', { repoPath: path, branchName: branch }, function(data) {
        // Store repoPath and branchName on the #commits div
        $('#commits').html(data).data('repo-path', path).data('branch-name', branch);
    });
});

$(document).ready(function () {
    // Event listener for commit clicks
    // It's important to use event delegation if commits are loaded dynamically
    $('#commits').on('click', '.logentry', function () {
        // Assuming the commit SHA is available as a data attribute or can be extracted
        // For example, if the commit SHA is in a div with class 'logentry__commitId'
        var commitSha = $(this).find('.logentry__commitId').text().trim();

        // Also need the current repository path. This might be stored globally
        // or on a parent element when branches/commits are loaded.
        // For this example, let's assume it's stored on the #commits div as a data attribute
        // You might need to adjust how currentRepoPath is obtained based on your existing JS structure.
        var currentRepoPath = $('#commits').data('repo-path');
        var currentBranchName = $('#commits').data('branch-name'); // If needed for context, though not directly for diff-tree

        if (!currentRepoPath) {
            console.error("Repository path not found. Make sure it's set as a data attribute on #commits or available globally.");
            $('#changes').html('<p>Error: Repository path not specified.</p>');
            return;
        }

        if (commitSha) {
            // Show a loading message in the 'changes' panel
            $('#changes').html('<p>Loading changes...</p>');

            $.ajax({
                url: '/Git/GetCommitChanges', // Make sure this URL is correct
                type: 'GET',
                data: {
                    repoPath: currentRepoPath,
                    commitSha: commitSha
                },
                success: function (data) {
                    $('#changes').html(data);
                },
                error: function (xhr, status, error) {
                    console.error("Error loading commit changes:", error);
                    $('#changes').html('<p>Error loading changes. Check console for details.</p>');
                }
            });
        } else {
            console.warn("Commit SHA not found for the clicked entry.");
            // Optionally clear the changes panel or show a message
            // $('#changes').html('');
        }
    });

    // IMPORTANT: When loading commits into the #commits div,
    // you MUST store `repo-path` as a data attribute on the #commits div.
    // For example, in the success callback of the AJAX request that loads commits:
    //
    // $('#commits').html(data).data('repo-path', repoPath).data('branch-name', branchName);
    //
    // Make sure this is done where $('#commits').html(data) is called for _LogPartial.cshtml
});



