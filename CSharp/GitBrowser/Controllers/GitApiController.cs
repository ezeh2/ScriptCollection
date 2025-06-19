using Microsoft.AspNetCore.Mvc;
using GitBrowser.Services; // Assuming IGitService will be used
using System.Linq; // For .ToList() and .Any()
using System.Threading.Tasks; // For async operations if needed later
using System.Collections.Generic; // For IEnumerable

namespace GitBrowser.Controllers
{
    [ApiController]
    [Route("api")]
    public class GitApiController : ControllerBase
    {
        private readonly IGitService _gitService;
        private readonly IApiGitService _apiGitService; // Added

        // Updated constructor
        public GitApiController(IGitService gitService, IApiGitService apiGitService)
        {
            _gitService = gitService;
            _apiGitService = apiGitService; // Added
        }

        [HttpGet("repos")]
        public ActionResult<IEnumerable<object>> GetRepositories()
        {
            var repos = _gitService.GetRepositories();
            if (repos == null || !repos.Any())
            {
                return Ok(new List<object>());
            }
            var apiRepos = repos.Select(r => new { Name = r.Name, Path = r.Name }).ToList();
            return Ok(apiRepos);
        }

        // New method for branches
        [HttpGet("repos/{repoName}/branches")]
        public ActionResult<IEnumerable<object>> GetBranches(string repoName)
        {
            var repositoryPath = _apiGitService.GetRepositoryPathByName(repoName);
            if (string.IsNullOrEmpty(repositoryPath))
            {
                return NotFound($"Repository '{repoName}' not found.");
            }

            var branches = _gitService.GetBranches(repositoryPath);
            if (branches == null || !branches.Any())
            {
                // It's possible a repo has no branches (e.g. newly initialized)
                // So returning an empty list is appropriate.
                return Ok(Enumerable.Empty<object>());
            }

            var apiBranches = branches.Select(b => new { Name = b.Name, IsRemote = b.IsRemote }).ToList();
            return Ok(apiBranches);
        }

        // New method for commits
        [HttpGet("repos/{repoName}/branches/{branchName}/commits")]
        public ActionResult<IEnumerable<GitCommit>> GetCommits(string repoName, string branchName)
        {
            var repositoryPath = _apiGitService.GetRepositoryPathByName(repoName);
            if (string.IsNullOrEmpty(repositoryPath))
            {
                return NotFound($"Repository '{repoName}' not found.");
            }

            // Ensure branchName is provided
            if (string.IsNullOrEmpty(branchName))
            {
                return BadRequest("Branch name must be provided.");
            }

            var commits = _gitService.GetCommits(repositoryPath, branchName);

            if (commits == null || !commits.Any())
            {
                // Check if the branch itself exists to differentiate between an empty branch and an invalid branch
                var branches = _gitService.GetBranches(repositoryPath);
                if (branches == null || !branches.Any(b => string.Equals(b.Name, branchName, StringComparison.OrdinalIgnoreCase)))
                {
                    return NotFound($"Branch '{branchName}' not found in repository '{repoName}'.");
                }
                // If branch exists but has no commits, return empty list
                return Ok(Enumerable.Empty<GitCommit>());
            }

            return Ok(commits);
        }

        [HttpGet("repos/{repoName}/commits/{commitSha}/changes")]
        public ActionResult<IEnumerable<GitCommitChange>> GetCommitChanges(string repoName, string commitSha)
        {
            var repositoryPath = _apiGitService.GetRepositoryPathByName(repoName);
            if (string.IsNullOrEmpty(repositoryPath))
            {
                return NotFound($"Repository '{repoName}' not found.");
            }

            if (string.IsNullOrEmpty(commitSha))
            {
                return BadRequest("Commit SHA must be provided.");
            }

            var changes = _gitService.GetCommitChanges(repositoryPath, commitSha);

            // GetCommitChanges in GitService returns an empty list if the process fails or no changes are found.
            // It's acceptable to return an empty list if the commitSha is invalid or yields no changes.
            if (changes == null) // Defensive check, though GitService currently initializes the list
            {
                return Ok(Enumerable.Empty<GitCommitChange>());
            }

            return Ok(changes);
        }
    }
}
