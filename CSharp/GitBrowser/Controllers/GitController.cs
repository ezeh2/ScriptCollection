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
        using var repo = new Repository(repoPath);
        var branches = repo.Branches
            .Where(b => !b.IsRemote)
            .Select(b => new GitBranch { Name = b.FriendlyName })
            .ToList();

        ViewBag.RepoPath = repoPath;
        return View(branches);
    }

    public IActionResult Log(string repoPath, string branchName)
    {
        using var repo = new Repository(repoPath);
        var branch = repo.Branches[branchName];
        var commits = branch.Commits
            .Select(c => new GitCommit
            {
                Sha = c.Sha,
                Message = c.MessageShort,
                Author = c.Author.Name,
                Date = c.Author.When.DateTime
            }).ToList();

        return View(commits);
    }
}
