// CSharp/GitBrowser/Services/IGitService.cs
using System.Collections.Generic;

namespace GitBrowser.Services
{
    public interface IGitService
    {
        IEnumerable<GitRepo> GetRepositories(); // rootPath parameter removed
        IEnumerable<GitBranch> GetBranches(string repoPath);
        IEnumerable<GitCommit> GetCommits(string repoPath, string branchName);
        bool IsValidRepoPath(string repoPath); // rootPath parameter removed
    }
}
