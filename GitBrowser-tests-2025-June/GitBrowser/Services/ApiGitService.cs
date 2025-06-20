using System;
using System.Linq;
using Microsoft.Extensions.Options; // If settings were needed, but not for this specific task

namespace GitBrowser.Services
{
    public class ApiGitService : IApiGitService
    {
        private readonly IGitService _gitService;

        public ApiGitService(IGitService gitService)
        {
            _gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
        }

        public string GetRepositoryPathByName(string repoName)
        {
            if (string.IsNullOrEmpty(repoName))
            {
                return null; // Or throw ArgumentNullException
            }

            var repositories = _gitService.GetRepositories();
            if (repositories == null)
            {
                return null;
            }

            var repo = repositories.FirstOrDefault(r =>
                string.Equals(r.Name, repoName, StringComparison.OrdinalIgnoreCase));

            return repo?.Path;
        }
    }
}
