using System;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // GitHub GraphQL API endpoint
        string apiUrl = "https://api.github.com/graphql";
        string? gitHubAccessToken = null;
        string? queryFileName = null;
	    string? query = null;
        bool verboseCommandLineArguments = false;
        bool verbose = false;

        // Check if the user provided no arguments
        if (args.Length == 0)
        {
            DisplayHelpText();

            // Terminate the application
            Environment.Exit(0);
        }
        // Check if the user provided exactly two arguments
        else if (args.Length == 2)
        {
            gitHubAccessToken = args[0];
            queryFileName = args[1];

            // Read the content of the file into the 'query' string
            try
            {
                query = File.ReadAllText(queryFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading query file: {ex.Message}");
                // Terminate the application
                Environment.Exit(1);
                return;
            }

            if (verboseCommandLineArguments)
            {
                Console.WriteLine($"GitHub Access Token: {gitHubAccessToken}");
                Console.WriteLine($"Query from File: {query}");
            }

            // Here you can proceed with using gitHubAccessToken and queryFileName as needed.
            // Example: Load the GraphQL query from the file and perform API requests.
        }
        // Invalid number of arguments provided
        else
        {
            Console.WriteLine("Invalid number of arguments. Please provide GitHub Access Token and Query File Name.");
            DisplayHelpText();

            // Terminate the application
            Environment.Exit(1); // You can use a different exit code if needed
        }

        string responseFile = $"{queryFileName}_response.json";
        Github_Graphql_Client github_Graphql_Client = new Github_Graphql_Client(apiUrl);

        string? cursor = null;

        List<Edge> edgesList = new List<Edge>();
        do
        {
            if (verbose)
            {
                Console.WriteLine($"Cursor: {cursor}");
            }

            string queryWithCursor = query.Replace("@0", cursor!= null ? $"\"{cursor}\"" : "null");
            Root? root = await github_Graphql_Client.Request(queryWithCursor, gitHubAccessToken);
            int? edgesCount = root?.Data?.SecurityVulnerabilities?.Edges?.Count;
            if (verbose)
            {
                Console.Out.WriteLine($"EdgesCount: {edgesCount}");
            }
            else
            {
                Console.Out.Write(".");
            }

            cursor = null;
            List<Edge>? edges = root?.Data?.SecurityVulnerabilities?.Edges;
            bool? b = edges?.Any();
            if (b.GetValueOrDefault(false))
            {
                cursor = root?.Data?.SecurityVulnerabilities?.Edges?.Last().Cursor;
                edgesList.AddRange(edges);
            }
        }
        while(cursor != null);

        Console.Out.WriteLine();
        Console.Out.WriteLine($"Read edges: {edgesList.Count}");

        string formattedJson = JsonConvert.SerializeObject(edgesList, Formatting.Indented);
        File.WriteAllText(responseFile, formattedJson);
        Console.Out.WriteLine($"Response written to: {responseFile}");
    }

    static void DisplayHelpText()
    {
        Console.WriteLine("Usage: YourAppName.exe <GitHubAccessToken> <QueryFileName>");
        Console.WriteLine("Example: YourAppName.exe abc123 query.txt");
    }
}

