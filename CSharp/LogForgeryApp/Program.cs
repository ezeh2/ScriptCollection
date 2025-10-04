var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.MapGet("/", () => "Hello from the log forging demo!");

app.MapGet("/log", (string? message, ILogger<Program> logger) =>
{
    // Vulnerable: user-supplied content is concatenated directly into a log entry.
    // An attacker can inject newlines or log prefixes to forge log entries.
    var rawMessage = message ?? "(no message provided)";
    logger.LogInformation("Incoming log entry: " + rawMessage);

    return Results.Ok(new
    {
        status = "logged",
        message = rawMessage
    });
});

app.Run();
