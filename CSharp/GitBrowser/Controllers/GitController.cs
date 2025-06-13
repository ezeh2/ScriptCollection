// Controllers/GitController.cs
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;

public class GitController : Controller
{
    private readonly string _rootPath = @"c:\work";

    public IActionResult Index()
    {
        var repos = Directory.GetDirectories(_rootPath)
            .Where(d => Directory.Exists(Path.Combine(d, ".git")))
            .Select(d => new GitRepo
            {
                Name = Path.GetFileName(d),
                Path = d
            }).ToList();

        return View(repos);
    }
	
	public IActionResult Branches(string repoPath)
	{
		if (!IsValidRepoPath(repoPath))
		{
			return BadRequest("Invalid path.");
		}

		using var repo = new Repository(repoPath);
		var branches = repo.Branches
			.Where(b => !b.IsRemote)
			.Select(b => new GitBranch { Name = b.FriendlyName })
			.ToList();

		ViewBag.RepoPath = repoPath;
		return PartialView("_BranchesPartial", branches);
	}
	
	public IActionResult Log(string repoPath, string branchName)
	{
		if (!IsValidRepoPath(repoPath))
		{
			return BadRequest("Invalid path.");
		}

		using var repo = new Repository(repoPath);
		var branch = repo.Branches[branchName];
		if (branch == null)
		{
			return NotFound("Branch not found.");
		}

		var commits = branch.Commits
			.Take(50) // Limit to 50 commits for performance
			.Select(c => new GitCommit
			{
				Sha = c.Sha,
				Message = c.MessageShort,
				Author = c.Author.Name,
				AuthorDate = c.Author.When.DateTime,
				Committer = c.Committer.Name,// does not compile
				CommitterDate = c.Committer.When.DateTime				// does not compile
			}).ToList();

		return PartialView("_LogPartial", commits);
	}
	
	private bool IsValidRepoPath(string repoPath)
	{
		var fullPath = Path.GetFullPath(repoPath);
		var allowedPath = Path.GetFullPath(_rootPath);
		return fullPath.StartsWith(allowedPath, StringComparison.OrdinalIgnoreCase)
			   && Directory.Exists(Path.Combine(fullPath, ".git"));
	}
	
}
