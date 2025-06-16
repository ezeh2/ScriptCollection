// CSharp/GitBrowser/Services/GitService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.Extensions.Options; // Added
// No using statement for Models needed as they are in global namespace

namespace GitBrowser.Services
{
    public class GitService : IGitService
    {
        private readonly string _rootPath;

        public GitService(IOptions<GitSettings> settings)
        {
            _rootPath = settings.Value.RootPath;
            if (string.IsNullOrEmpty(_rootPath))
            {
                // Fallback or throw if RootPath is critical and not set
                // For now, let's throw an exception if it's not configured.
                throw new ArgumentNullException(nameof(settings), "GitSettings:RootPath is not configured in appsettings.json");
            }
        }

        public IEnumerable<GitRepo> GetRepositories()
        {
            if (string.IsNullOrEmpty(_rootPath) || !Directory.Exists(_rootPath))
            {
                // Or throw a specific exception / handle error appropriately
                return Enumerable.Empty<GitRepo>();
            }

            return Directory.GetDirectories(_rootPath)
                .Where(d => Directory.Exists(Path.Combine(d, ".git")))
                .Select(d => new GitRepo
                {
                    Name = Path.GetFileName(d),
                    Path = d
                }).ToList();
        }

        public IEnumerable<GitBranch> GetBranches(string repoPath)
        {
            // IsValidRepoPath should be called by the controller or higher level service
            // before calling this method.
            if (string.IsNullOrEmpty(repoPath) || !Repository.IsValid(repoPath))
            {
                 // Or throw
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
            // Assuming repoPath is validated and branchName is not null/empty
            if (string.IsNullOrEmpty(repoPath) || !Repository.IsValid(repoPath) || string.IsNullOrEmpty(branchName))
            {
                // Or throw
                return Enumerable.Empty<GitCommit>();
            }

            using var repo = new Repository(repoPath);
            var branch = repo.Branches[branchName];
            if (branch == null)
            {
                // Or throw new NotFoundException($"Branch '{branchName}' not found in repo '{repoPath}'.");
                return Enumerable.Empty<GitCommit>();
            }

            return branch.Commits
                .Take(50) // Limit to 50 commits for performance
                .Select(c => {
                    var gitCommit = new GitCommit
                    {
                        Sha = c.Sha,
                        Message = c.MessageShort,
                        Author = c.Author.Name,
                        AuthorDate = c.Author.When.DateTime,
                        Committer = c.Committer.Name,
                        CommitterDate = c.Committer.When.DateTime
                        // Decorations and Notes are initialized by default
                    };

                    // Populate Decorations
                    foreach (var reference in repo.Refs)
                    {
                        if (reference is DirectReference directRef && directRef.TargetIdentifier == c.Sha)
                        {
                            var friendlyName = reference.FriendlyName;
                            if (friendlyName.StartsWith("refs/heads/"))
                            {
                                friendlyName = friendlyName.Substring("refs/heads/".Length);
                            }
                            else if (friendlyName.StartsWith("refs/tags/"))
                            {
                                friendlyName = friendlyName.Substring("refs/tags/".Length);
                            }
                            else if (friendlyName.StartsWith("refs/remotes/"))
                            {
                                 friendlyName = friendlyName.Substring("refs/remotes/".Length);
                            }
                            // Add other prefixes if needed
                            gitCommit.Decorations.Add(friendlyName);
                        }
                    }

                    // Populate Notes
                    foreach (var note in c.Notes)
                    {
                        gitCommit.Notes.Add(note.Message);
                    }

                    return gitCommit;
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
                var allowedPath = Path.GetFullPath(_rootPath); // Use field
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
