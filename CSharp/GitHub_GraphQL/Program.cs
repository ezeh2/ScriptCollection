using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;

class Program
{
    static async Task Main(string[] args)
    {
        string? gitHubAccessToken = null;
        string? queryFileName = null;
	string? query = null;

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

            Console.WriteLine($"GitHub Access Token: {gitHubAccessToken}");
            Console.WriteLine($"Query File Name: {queryFileName}");

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

            Console.WriteLine($"GitHub Access Token: {gitHubAccessToken}");
            Console.WriteLine($"Query from File: {query}");


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


        // GraphQL query as a string
        // string graphqlQuery = "query { repository(owner:\"ezeh2\",name:\"Lagoudia\") {name databaseId } }";

        // Construct the JSON payload
        var jsonPayload = new
        {
            query = query
        };

        // Convert the payload to JSON string
        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonPayload);

        // GitHub GraphQL API endpoint
        string apiUrl = "https://api.github.com/graphql";

        // Create HttpClient instance
        using (HttpClient httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler())))
        {
            // Set the authorization header with the GitHub personal access token
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {gitHubAccessToken }");

            // Set the User-Agent header
	    httpClient.DefaultRequestHeaders.Add("User-Agent", "YourAppName");
            
	    // Create the content for the POST request
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Send the POST request
            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

            // Check if the request was successful (status code 200)
            if (response.IsSuccessStatusCode)
            {
                // Read and display the response content
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from GitHub API:");
                Console.WriteLine(responseContent);
            }
            else
            {
                // Display the error status code
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
    }

    static void DisplayHelpText()
    {
        Console.WriteLine("Usage: YourAppName.exe <GitHubAccessToken> <QueryFileName>");
        Console.WriteLine("Example: YourAppName.exe abc123 query.txt");
    }
}

