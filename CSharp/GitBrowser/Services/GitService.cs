// CSharp/GitBrowser/Services/GitService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.Extensions.Options;

namespace GitBrowser.Services
{
    public class GitService : IGitService
    {
        private readonly string _rootPath;
        private readonly int _maxSearchDepth;

        public GitService(IOptions<GitSettings> settings)
        {
            _rootPath = settings.Value.RootPath;
            if (string.IsNullOrEmpty(_rootPath))
            {
                throw new ArgumentNullException(nameof(settings), "GitSettings:RootPath is not configured in appsettings.json");
            }
            _maxSearchDepth = settings.Value.MaxSearchDepth > 0 ? settings.Value.MaxSearchDepth : 1;
        }

        public IEnumerable<GitRepo> GetRepositories()
        {
            var foundRepos = new List<GitRepo>();
            if (string.IsNullOrEmpty(_rootPath) || !Directory.Exists(_rootPath))
            {
                return Enumerable.Empty<GitRepo>();
            }
            FindRepositoriesRecursive(_rootPath, 0, foundRepos);
            return foundRepos;
        }

        private void FindRepositoriesRecursive(string currentSearchPath, int currentRelativeDepth, List<GitRepo> foundRepos)
        {
            if (currentRelativeDepth >= _maxSearchDepth)
            {
                return;
            }

            try
            {
                foreach (var subDir in Directory.GetDirectories(currentSearchPath))
                {
                    if (Directory.Exists(Path.Combine(subDir, ".git")))
                    {
                        foundRepos.Add(new GitRepo
                        {
                            Name = Path.GetFileName(subDir),
                            Path = subDir
                        });
                    }
                    // IMPORTANT: Recursive call should be outside the .git check,
                    // to search deeper even if current subDir is not a repo.
                    FindRepositoriesRecursive(subDir, currentRelativeDepth + 1, foundRepos);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip directories that cannot be accessed.
            }
            catch (DirectoryNotFoundException)
            {
                // Skip if a directory is deleted during enumeration.
            }
        }

        public IEnumerable<GitBranch> GetBranches(string repoPath)
        {
            if (string.IsNullOrEmpty(repoPath) || !Repository.IsValid(repoPath))
            {
                return Enumerable.Empty<GitBranch>();
            }
            using var repo = new Repository(repoPath);
            return repo.Branches
                .Where(b => !b.IsRemote)
                .Select(b => new GitBranch { Name = b.FriendlyName })
                .ToList();
        }

        public IEnumerable<GitCommit> GetCommits(string repoPath, string branchName)
        {
            if (string.IsNullOrEmpty(repoPath) || !Repository.IsValid(repoPath) || string.IsNullOrEmpty(branchName))
            {
                return Enumerable.Empty<GitCommit>();
            }
            using var repo = new Repository(repoPath);
            var branch = repo.Branches[branchName];
            if (branch == null)
            {
                return Enumerable.Empty<GitCommit>();
            }
            return branch.Commits
                .Take(50) // Limit to 50 commits for performance
                .Select(c => new GitCommit
                {
                    Sha = c.Sha,
                    MessageShort = c.MessageShort,					
                    Message = c.Message,
                    Author = c.Author.Name,
                    AuthorDate = c.Author.When.DateTime,
                    Committer = c.Committer.Name,
                    CommitterDate = c.Committer.When.DateTime
                }).ToList();
        }

        public bool IsValidRepoPath(string repoPath)
        {
            if (string.IsNullOrEmpty(repoPath) || string.IsNullOrEmpty(_rootPath))
            {
                return false;
            }
            try
            {
                var fullPath = Path.GetFullPath(repoPath);
                var allowedPath = Path.GetFullPath(_rootPath);
                return fullPath.StartsWith(allowedPath, StringComparison.OrdinalIgnoreCase)
                       && Directory.Exists(Path.Combine(fullPath, ".git"));
            }
            catch (Exception)
            {
                // Path exceptions, security exceptions etc.
                return false;
            }
        }
    }
}
