namespace AzBuildRunsWebApp.Models;

public record BuildRun(
    int Id,
    string Name,
    string RequestedBy,
    string Status,
    Uri WebLink);
