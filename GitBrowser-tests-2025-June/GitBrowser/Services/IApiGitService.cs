using System.Threading.Tasks; // Or remove if not using async initially

namespace GitBrowser.Services
{
    public interface IApiGitService
    {
        string GetRepositoryPathByName(string repoName);
        // Consider adding async version later if I/O becomes a bottleneck
        // Task<string> GetRepositoryPathByNameAsync(string repoName);
    }
}
