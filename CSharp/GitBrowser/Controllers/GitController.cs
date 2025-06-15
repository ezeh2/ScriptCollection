// Controllers/GitController.cs
using Microsoft.AspNetCore.Mvc;
using GitBrowser.Services; // Added
using System.Linq; // For .ToList() and .Any()

public class GitController : Controller
{
    private readonly IGitService _gitService;
    // private readonly string _rootPath = @"c:\work"; // Removed

    public GitController(IGitService gitService)
    {
        _gitService = gitService;
    }

    public IActionResult Index()
    {
        var repos = _gitService.GetRepositories().ToList(); // _rootPath removed
        return View(repos);
    }

    public IActionResult Branches(string repoPath)
    {
        if (!_gitService.IsValidRepoPath(repoPath)) // _rootPath removed
        {
            return BadRequest("Invalid path.");
        }

        var branches = _gitService.GetBranches(repoPath).ToList();
        ViewBag.RepoPath = repoPath;
        return PartialView("_BranchesPartial", branches);
    }

    public IActionResult Log(string repoPath, string branchName)
    {
        if (!_gitService.IsValidRepoPath(repoPath)) // _rootPath removed
        {
            return BadRequest("Invalid path.");
        }

        if (string.IsNullOrEmpty(branchName))
        {
            // This check was not in the original Log, but it's good practice.
            // The service's GetCommits would return empty for null/empty branchName if not handled.
            return NotFound("Branch not specified.");
        }

        var commits = _gitService.GetCommits(repoPath, branchName).ToList();

        // If GetCommits returns an empty list, it could be an empty branch or an invalid branch.
        // To replicate original behavior of NotFound("Branch not found."):
        if (!commits.Any())
        {
            var branches = _gitService.GetBranches(repoPath);
            if (!branches.Any(b => b.Name == branchName))
            {
                return NotFound("Branch not found.");
            }
        }
		ViewBag.RepoName = Path.GetFileName(repoPath);
		ViewBag.BranchName = branchName;		
        return PartialView("_LogPartial", commits);
    }
}
