// CSharp/GitBrowser/Services/GitService.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public List<GitCommitChange> GetCommitChanges(string repoPath, string commitSha)
        {
            var changes = new List<GitCommitChange>();
            if (string.IsNullOrEmpty(repoPath) || string.IsNullOrEmpty(commitSha))
            {
                // Or throw an ArgumentException, depending on desired error handling
                return changes;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"diff-tree --no-commit-id --name-status -r {commitSha}",
                WorkingDirectory = repoPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true, // It's good practice to redirect error stream as well
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    // Log error or handle appropriately
                    return changes; // Or throw an exception
                }

                string output = process.StandardOutput.ReadToEnd();
                string errorOutput = process.StandardError.ReadToEnd(); // Capture error output
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    // Log the error (errorOutput) or throw an exception
                    // For now, returning an empty list or could throw new InvalidOperationException($"Git command failed with error: {errorOutput}");
                    Console.Error.WriteLine($"Git diff-tree error for {repoPath}@{commitSha}: {errorOutput}");
                    return changes; // Or throw
                }

                var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        var statusAbbreviation = parts[0].Trim();
                        var fileName = parts[1].Trim();
                        string status = statusAbbreviation switch
                        {
                            "A" => "Added",
                            "M" => "Modified",
                            "D" => "Deleted",
                            "R" // R seguida por um número, por exemplo R100
                               when statusAbbreviation.StartsWith("R") => "Renamed",
                            "C" // C seguida por um número, por exemplo C100
                               when statusAbbreviation.StartsWith("C") => "Copied",
                            _ => statusAbbreviation // fallback to abbreviation if unknown
                        };

                        // If Renamed or Copied, there might be an old filename as well
                        if ((status == "Renamed" || status == "Copied") && parts.Length > 2)
                        {
                            fileName = $"{parts[2].Trim()} (from {parts[1].Trim()})"; // New (from Old)
                        }


                        changes.Add(new GitCommitChange { Status = status, FileName = fileName });
                    }
                }
            }
            return changes;
        }
    }
}
