using AzBuildRunsWebApp.Models;
using AzBuildRunsWebApp.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AzBuildRunsWebApp.Pages;

public sealed class IndexModel : PageModel
{
    private readonly IAzureBuildRunService _buildRunService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IAzureBuildRunService buildRunService, ILogger<IndexModel> logger)
    {
        _buildRunService = buildRunService;
        _logger = logger;
    }

    public List<BuildRun> BuildRuns { get; private set; } = new();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync()
    {
        try
        {
            var runs = await _buildRunService.GetBuildRunsAsync(HttpContext.RequestAborted);
            BuildRuns = runs.OrderByDescending(r => r.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load build runs");
            ErrorMessage = "Unable to load build runs. Please check the server logs for more details.";
        }
    }
}
