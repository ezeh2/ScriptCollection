// Controllers/GitController.cs
using Microsoft.AspNetCore.Mvc;
using GitBrowser.Services;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions; // Added for Regex

public class GitController : Controller
{
    private readonly IGitService _gitService;

    // Regex for basic SHA-1 validation (40 hex characters)
    // Allows for shorter SHAs as well (e.g. 7 characters)
    private static readonly Regex ShaRegex = new Regex("^[0-9a-fA-F]{7,40}$");

    // Invalid characters for branch names (based on common Git restrictions)
    // See: https://git-scm.com/docs/git-check-ref-format
    private static readonly char[] InvalidBranchNameChars = new char[] { ' ', '~', '^', ':', '*', '?', '[', '\\' };
    private static readonly string[] InvalidBranchNameSubstrings = new string[] { "..", "@{" };
    private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars().Concat(new char[] { '?', '*', ':', '<', '>', '|' }).Distinct().ToArray();


    public GitController(IGitService gitService)
    {
        _gitService = gitService;
    }

    private bool IsValidBranchName(string branchName)
    {
        if (string.IsNullOrWhiteSpace(branchName) ||
            branchName.IndexOfAny(InvalidBranchNameChars) >= 0 ||
            InvalidBranchNameSubstrings.Any(s => branchName.Contains(s)) ||
            branchName.StartsWith(".") || branchName.EndsWith(".") ||
            branchName.EndsWith("/"))
        {
            return false;
        }
        return true;
    }

    private bool IsPathPotentiallyValid(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        // Basic check for invalid characters before hitting the service layer
        if (path.Any(c => InvalidPathChars.Contains(c))) return false;
        // Path traversal check (simplified) - service layer should have more robust checks
        if (path.Contains("..")) return false; // Simple check, might need refinement
        return true;
    }


    public IActionResult Index()
    {
        var repos = _gitService.GetRepositories().ToList();
        return View(repos);
    }

    public IActionResult Branches(string repoPath)
    {
        if (!IsPathPotentiallyValid(repoPath))
        {
            return BadRequest("Repository path contains invalid characters or format.");
        }
        if (!_gitService.IsValidRepoPath(repoPath))
        {
            return BadRequest("Invalid repository path provided.");
        }

        var branches = _gitService.GetBranches(repoPath).ToList();
        ViewBag.RepoPath = repoPath;
        return PartialView("_BranchesPartial", branches);
    }

    public IActionResult Log(string repoPath, string branchName)
    {
        if (!IsPathPotentiallyValid(repoPath))
        {
            return BadRequest("Repository path contains invalid characters or format.");
        }
        if (!_gitService.IsValidRepoPath(repoPath))
        {
            return BadRequest("Invalid repository path provided.");
        }

        if (!IsValidBranchName(branchName))
        {
            return BadRequest("Invalid branch name provided.");
        }

        var commits = _gitService.GetCommits(repoPath, branchName).ToList();

        if (!commits.Any())
        {
            var branches = _gitService.GetBranches(repoPath);
            // Using Ordinal comparison for branch names for consistency with potential Git behavior
            if (!branches.Any(b => string.Equals(b.Name, branchName, System.StringComparison.Ordinal)))
            {
                return NotFound("Branch not found.");
            }
        }
		ViewBag.RepoName = Path.GetFileName(repoPath); // Ensure repoPath is not null/empty before this
		ViewBag.BranchName = branchName;		
        return PartialView("_LogPartial", commits);
    }

    public IActionResult GetCommitChanges(string repoPath, string commitSha)
    {
        if (!IsPathPotentiallyValid(repoPath))
        {
            return BadRequest("Repository path contains invalid characters or format.");
        }
        if (!_gitService.IsValidRepoPath(repoPath)) // Added consistent validation
        {
            return BadRequest("Invalid repository path provided.");
        }

        if (string.IsNullOrWhiteSpace(commitSha) || !ShaRegex.IsMatch(commitSha))
        {
            return BadRequest("Commit SHA must be a valid SHA identifier (7-40 hex characters).");
        }

        try
        {
            var changes = _gitService.GetCommitChanges(repoPath, commitSha);
            ViewBag.RepoPath = repoPath;
            ViewBag.CommitSha = commitSha;
            return PartialView("_ChangesPartial", changes);
        }
        catch (System.Exception ex)
        {
            // Log the exception (implementation of logging is outside this scope)
            // Elmah.ErrorSignal.FromCurrentContext().Raise(ex); // Example if using Elmah
            return StatusCode(500, $"An error occurred while retrieving commit changes: {ex.Message}");
        }
    }
}
