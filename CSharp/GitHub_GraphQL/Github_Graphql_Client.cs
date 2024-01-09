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

    public async Task<Root?> Request(string query, string gitHubAccessToken)
    {
        Root? root = null;

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
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {gitHubAccessToken}");

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
                if (verboseHttp)
                {
                    Console.Out.WriteLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                }

                // deserialize response content
                string responseContent = await response.Content.ReadAsStringAsync();
                root = JsonConvert.DeserializeObject<Root>(responseContent);
                return root;
            }
            else
            {
                // Display the error status code
                Console.Error.WriteLine($"Error: {(int)response.StatusCode} {response.ReasonPhrase}");
            }
        }

        return root;
    }

}


