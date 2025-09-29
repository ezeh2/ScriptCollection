using AzBuildRunsWebApp.Models;

namespace AzBuildRunsWebApp.Services;

public interface IAzureBuildRunService
{
    Task<IReadOnlyList<BuildRun>> GetBuildRunsAsync(CancellationToken cancellationToken = default);
}
