using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AzBuildRunsWebApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzBuildRunsWebApp.Services;

public sealed class AzureCliBuildRunService : IAzureBuildRunService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureCliBuildRunService> _logger;

    public AzureCliBuildRunService(IConfiguration configuration, ILogger<AzureCliBuildRunService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IReadOnlyList<BuildRun>> GetBuildRunsAsync(CancellationToken cancellationToken = default)
    {
        var settings = LoadSettings();

        var startInfo = new ProcessStartInfo
        {
            FileName = "az",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("pipelines");
        startInfo.ArgumentList.Add("runs");
        startInfo.ArgumentList.Add("list");
        startInfo.ArgumentList.Add("--org");
        startInfo.ArgumentList.Add($"https://dev.azure.com/{settings.Organization}");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(settings.Project);
        startInfo.ArgumentList.Add("--requested-for");
        startInfo.ArgumentList.Add(settings.User);
        startInfo.ArgumentList.Add("--output");
        startInfo.ArgumentList.Add("json");

        _logger.LogInformation(
            "Executing Azure CLI: az {Arguments}",
            string.Join(' ', startInfo.ArgumentList.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)));

        using var process = new Process { StartInfo = startInfo };
        if (!process.Start())
        {
            throw new InvalidOperationException("Failed to start Azure CLI process.");
        }

        using var output = process.StandardOutput;
        using var error = process.StandardError;

        var outputTask = output.ReadToEndAsync(cancellationToken);
        var errorTask = error.ReadToEndAsync(cancellationToken);

        await Task.WhenAll(outputTask, errorTask);

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            _logger.LogError("Azure CLI exited with {ExitCode}: {Error}", process.ExitCode, errorTask.Result);
            throw new InvalidOperationException($"Azure CLI failed with exit code {process.ExitCode}: {errorTask.Result}");
        }

        var json = outputTask.Result;
        if (string.IsNullOrWhiteSpace(json))
        {
            return Array.Empty<BuildRun>();
        }

        try
        {
            var runs = JsonSerializer.Deserialize<List<PipelineRun>>(json, SerializerOptions) ?? [];
            return runs
                .Select(run => new BuildRun(
                    run.Id,
                    run.Name ?? "(unnamed)",
                    run.RequestedFor?.DisplayName ?? run.RequestedFor?.UniqueName ?? "Unknown",
                    run.Result ?? run.Status ?? "Unknown",
                    CreateRunLink(run, settings)))
                .ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse Azure CLI output");
            throw;
        }
    }

    private (string Organization, string Project, string User) LoadSettings()
    {
        var organization = _configuration["AzureDevOps:Organization"];
        var project = _configuration["AzureDevOps:Project"];
        var user = _configuration["AzureDevOps:User"];

        if (string.IsNullOrWhiteSpace(organization) || string.IsNullOrWhiteSpace(project) || string.IsNullOrWhiteSpace(user))
        {
            throw new InvalidOperationException("Organization, Project, and User must be configured in appsettings.json.");
        }

        return (organization, project, user);
    }

    private static Uri CreateRunLink(PipelineRun run, (string Organization, string Project, string User) settings)
    {
        if (Uri.TryCreate(run.WebUrl, UriKind.Absolute, out var uri))
        {
            return uri;
        }

        if (Uri.TryCreate(run.Url, UriKind.Absolute, out uri))
        {
            return uri;
        }

        var fallback = $"https://dev.azure.com/{settings.Organization}/{settings.Project}/_build/results?buildId={run.Id}";
        return new Uri(fallback, UriKind.Absolute);
    }

    private sealed record PipelineRun
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Url { get; init; }
        public string? WebUrl { get; init; }
        public string? Result { get; init; }
        public string? Status { get; init; }
        public Identity? RequestedFor { get; init; }
    }

    private sealed record Identity
    {
        public string? DisplayName { get; init; }
        public string? UniqueName { get; init; }
    }
}
