using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


public class Github_Graphql_Client
{
    private bool verboseHttp = false;
    private string apiUrl;

    public Github_Graphql_Client(string apiUrl)
    {
        this.apiUrl = apiUrl;
    }

    public async Task Request(string query, string gitHubAccessToken, string responseFile)
    {

        // Construct the JSON payload
        var jsonPayload = new
        {
            query = query
        };

        // Convert the payload to JSON string
        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonPayload);


        HttpMessageHandler httpMessageHandler = new HttpClientHandler();
        if (verboseHttp)
        {
            httpMessageHandler = new LoggingHandler(httpMessageHandler);
        }
        using (HttpClient httpClient = new HttpClient(httpMessageHandler))
        {
            // Set the authorization header with the GitHub personal access token
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {gitHubAccessToken }");

            // Set the User-Agent header
	        httpClient.DefaultRequestHeaders.Add("User-Agent", "DotNetTestApp");

            httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            
	        // Create the content for the POST request
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Send the POST request
            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

            // Check if the request was successful (status code 200)
            if (response.IsSuccessStatusCode)
            {
                Console.Out.WriteLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");

                // Read and display the response content
                string responseContent = await response.Content.ReadAsStringAsync();

                string formattedJson = JsonConvert.SerializeObject(
                    JsonConvert.DeserializeObject(responseContent),
                    Formatting.Indented
                );

                System.IO.File.WriteAllText(responseFile, formattedJson);
                Console.Out.WriteLine($"Response written to: {responseFile}");

                Root? root = JsonConvert.DeserializeObject<Root>(responseContent);
                int? edgesCount = root?.Data?.SecurityVulnerabilities?.Edges?.Count;
                Console.Out.WriteLine($"EdgesCount: {edgesCount}");
            }
            else
            {
                // Display the error status code
                Console.Error.WriteLine($"Error: {(int)response.StatusCode} {response.ReasonPhrase}");
            }
        }
    }

}


