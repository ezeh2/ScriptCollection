using Microsoft.AspNetCore.Mvc;
using GitBrowser.Services; // Assuming IGitService will be used
using System.Linq; // For .ToList() and .Any()
using System.Threading.Tasks; // For async operations if needed later
using System.Collections.Generic; // For IEnumerable
using System.Text.RegularExpressions; // For Regex

namespace GitBrowser.Controllers
{
    [ApiController]
    [Route("api")]
    public class GitApiController : ControllerBase
    {
        private readonly IGitService _gitService;
        private readonly IApiGitService _apiGitService;

        // Regex for basic SHA-1 validation (40 hex characters)
        // Allows for shorter SHAs as well (e.g. 7 characters)
        private static readonly Regex ShaRegex = new Regex("^[0-9a-fA-F]{7,40}$");
        // Invalid characters for repo names (simplified) - e.g. path separators, control chars
        private static readonly char[] InvalidRepoNameChars = new char[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|', '\0', '\n', '\r', '\t' };
        // Invalid characters for branch names (based on common Git restrictions)
        // See: https://git-scm.com/docs/git-check-ref-format
        private static readonly char[] InvalidBranchNameChars = new char[] { ' ', '~', '^', ':', '*', '?', '[', '\\' };
        private static readonly string[] InvalidBranchNameSubstrings = new string[] { "..", "@{" };


        public GitApiController(IGitService gitService, IApiGitService apiGitService)
        {
            _gitService = gitService;
            _apiGitService = apiGitService;
        }

        private bool IsValidRepoName(string repoName)
        {
            if (string.IsNullOrWhiteSpace(repoName) || repoName.IndexOfAny(InvalidRepoNameChars) >= 0 || repoName.Contains(".."))
            {
                return false;
            }
            // Further checks can be added, e.g. for names like "." or ".." if they are not already caught
            if (repoName == "." || repoName == "..") return false;
            return true;
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

        [HttpGet("repos/{repoName}/branches")]
        public ActionResult<IEnumerable<object>> GetBranches(string repoName)
        {
            if (!IsValidRepoName(repoName))
            {
                return BadRequest("Invalid repository name provided.");
            }

            var repositoryPath = _apiGitService.GetRepositoryPathByName(repoName);
            if (string.IsNullOrEmpty(repositoryPath))
            {
                return NotFound($"Repository '{repoName}' not found.");
            }

            var branches = _gitService.GetBranches(repositoryPath);
            if (branches == null || !branches.Any())
            {
                return Ok(Enumerable.Empty<object>());
            }

            var apiBranches = branches.Select(b => new { Name = b.Name, IsRemote = b.IsRemote }).ToList();
            return Ok(apiBranches);
        }

        [HttpGet("repos/{repoName}/branches/{branchName}/commits")]
        public ActionResult<IEnumerable<GitCommit>> GetCommits(string repoName, string branchName)
        {
            if (!IsValidRepoName(repoName))
            {
                return BadRequest("Invalid repository name provided.");
            }
            if (!IsValidBranchName(branchName))
            {
                return BadRequest("Invalid branch name provided.");
            }

            var repositoryPath = _apiGitService.GetRepositoryPathByName(repoName);
            if (string.IsNullOrEmpty(repositoryPath))
            {
                return NotFound($"Repository '{repoName}' not found.");
            }

            var commits = _gitService.GetCommits(repositoryPath, branchName);

            if (commits == null || !commits.Any())
            {
                var branches = _gitService.GetBranches(repositoryPath);
                // Use a case-insensitive comparison for branch names if appropriate for your Git service
                if (branches == null || !branches.Any(b => string.Equals(b.Name, branchName, StringComparison.Ordinal)))
                {
                    return NotFound($"Branch '{branchName}' not found in repository '{repoName}'.");
                }
                return Ok(Enumerable.Empty<GitCommit>());
            }

            return Ok(commits);
        }

        [HttpGet("repos/{repoName}/commits/{commitSha}/changes")]
        public ActionResult<IEnumerable<GitCommitChange>> GetCommitChanges(string repoName, string commitSha)
        {
            if (!IsValidRepoName(repoName))
            {
                return BadRequest("Invalid repository name provided.");
            }

            if (string.IsNullOrWhiteSpace(commitSha) || !ShaRegex.IsMatch(commitSha))
            {
                return BadRequest("Commit SHA must be a valid SHA identifier (7-40 hex characters).");
            }

            var repositoryPath = _apiGitService.GetRepositoryPathByName(repoName);
            if (string.IsNullOrEmpty(repositoryPath))
            {
                return NotFound($"Repository '{repoName}' not found.");
            }

            var changes = _gitService.GetCommitChanges(repositoryPath, commitSha);

            if (changes == null)
            {
                return Ok(Enumerable.Empty<GitCommitChange>());
            }

            return Ok(changes);
        }
    }
}
