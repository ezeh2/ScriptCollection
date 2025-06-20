using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Text.Json; // For System.Text.Json
using System.Collections.Generic;
using GitBrowser.Models; // For deserializing to expected models
using System.Linq; // For .Any() and .First()

namespace GitBrowser.E2ETests
{
    public class ApiEndpointsTests
    {
        private readonly HttpClient _client;
        private const string BaseUrl = "http://localhost:5163"; // Adjust if your launchSettings.json uses a different port for HTTP

        // A known repository name that exists in your test environment's GitSettings.RootPath
        // IMPORTANT: This repo MUST exist for tests to pass.
        // You might need to create a dummy repo in your 'c:\work\repos' (or configured RootPath)
        // e.g., 'c:\work\repos\test-repo1'
        private const string TestRepoName = "test-repo1"; // Example repo name
        private const string TestBranchName = "main"; // Or "master", depending on the test repo
        private string _testCommitSha = ""; // Will be fetched dynamically

        public ApiEndpointsTests()
        {
            _client = new HttpClient { BaseAddress = new System.Uri(BaseUrl) };
        }

        private async Task<JsonElement> GetJsonResponse(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Fail test if not 2xx
            var content = await response.Content.ReadAsStringAsync();
            // It's better to deserialize to JsonElement if you're just checking structure or specific fields
            // For lists, you might deserialize to List<YourModel> if you have the model, or List<JsonElement>
            return JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Helper to get a valid commit SHA from the test repo and branch
        private async Task InitializeTestCommitSha()
        {
            if (!string.IsNullOrEmpty(_testCommitSha)) return;

            var response = await _client.GetAsync($"/api/repos/{TestRepoName}/branches/{TestBranchName}/commits");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // Ensure PropertyNameCaseInsensitive is true for deserialization
                var commits = JsonSerializer.Deserialize<List<GitCommit>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (commits != null && commits.Count > 0)
                {
                    _testCommitSha = commits[0].Sha; // Get the latest commit SHA
                }
            }
            Assert.False(string.IsNullOrEmpty(_testCommitSha), $"Could not fetch a valid commit SHA for {TestRepoName}/{TestBranchName}. Ensure the test repository and branch exist and have commits.");
        }

        [Fact]
        public async Task GetRepositories_ReturnsListOfRepositories()
        {
            var json = await GetJsonResponse("/api/repos");
            Assert.True(json.ValueKind == JsonValueKind.Array);
            // Check if TestRepoName is among the returned repos
            bool foundTestRepo = false;
            foreach (var repoElement in json.EnumerateArray())
            {
                if (repoElement.TryGetProperty("name", out JsonElement nameElement)) // Assuming 'name' property
                {
                    if (nameElement.GetString() == TestRepoName)
                    {
                        foundTestRepo = true;
                        break;
                    }
                }
            }
            Assert.True(foundTestRepo, $"Test repository '{TestRepoName}' not found in API response. Ensure '{TestRepoName}' exists in GitSettings.RootPath and the web application is running at {BaseUrl}.");
        }

        [Fact]
        public async Task GetBranches_ReturnsBranchesForValidRepo()
        {
            var json = await GetJsonResponse($"/api/repos/{TestRepoName}/branches");
            Assert.True(json.ValueKind == JsonValueKind.Array);
            Assert.True(json.EnumerateArray().Any(), $"No branches found for repository '{TestRepoName}'. Ensure it has branches (e.g., '{TestBranchName}').");
            // Check for the main/master branch
             bool foundTestBranch = false;
            foreach (var branchElement in json.EnumerateArray())
            {
                if (branchElement.TryGetProperty("name", out JsonElement nameElement))
                {
                    if (nameElement.GetString() == TestBranchName)
                    {
                        foundTestBranch = true;
                        break;
                    }
                }
            }
            Assert.True(foundTestBranch, $"Test branch '{TestBranchName}' not found in repo '{TestRepoName}'.");
        }

        [Fact]
        public async Task GetBranches_ReturnsNotFoundForInvalidRepo()
        {
            var response = await _client.GetAsync($"/api/repos/invalid-repo-name-that-does-not-exist/branches");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCommits_ReturnsCommitsForValidRepoAndBranch()
        {
            await InitializeTestCommitSha(); // Ensures _testCommitSha is set
            var json = await GetJsonResponse($"/api/repos/{TestRepoName}/branches/{TestBranchName}/commits");
            Assert.True(json.ValueKind == JsonValueKind.Array);
            Assert.True(json.EnumerateArray().Any(), $"No commits found for branch '{TestBranchName}' in repository '{TestRepoName}'. Ensure it has commits.");
             // Check if the _testCommitSha is among the returned commits
            bool foundTestCommit = false;
            foreach (var commitElement in json.EnumerateArray())
            {
                if (commitElement.TryGetProperty("sha", out JsonElement shaElement))
                {
                    if (shaElement.GetString() == _testCommitSha)
                    {
                        foundTestCommit = true;
                        break;
                    }
                }
            }
            Assert.True(foundTestCommit, $"Specific test commit SHA '{_testCommitSha}' not found.");
        }

        [Fact]
        public async Task GetCommits_ReturnsNotFoundForInvalidBranch()
        {
            var response = await _client.GetAsync($"/api/repos/{TestRepoName}/branches/invalid-branch-name/commits");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCommitChanges_ReturnsChangesForValidCommit()
        {
            await InitializeTestCommitSha(); // Ensure _testCommitSha is populated
            Assert.False(string.IsNullOrEmpty(_testCommitSha), "Test commit SHA was not initialized. Check logs from InitializeTestCommitSha.");

            var json = await GetJsonResponse($"/api/repos/{TestRepoName}/commits/{_testCommitSha}/changes");
            Assert.True(json.ValueKind == JsonValueKind.Array, "Response should be a JSON array.");
            // We can't easily assert specific changes without knowing the repo content,
            // but we can check if the format is as expected (e.g., contains 'status' and 'fileName' properties if not empty)
            if (json.EnumerateArray().Any())
            {
                var firstChange = json.EnumerateArray().First();
                Assert.True(firstChange.TryGetProperty("status", out _), "Change object should have a 'status' property.");
                Assert.True(firstChange.TryGetProperty("fileName", out _), "Change object should have a 'fileName' property.");
            }
            // If the test repo's first commit is an initial commit with files, it should have changes.
            // Assert.True(json.EnumerateArray().Any(), $"No changes found for commit '{_testCommitSha}'. This might be an empty initial commit or an issue with GetCommitChanges.");
        }

        [Fact]
        public async Task GetCommitChanges_ReturnsEmptyArrayForInvalidSha()
        {
            // An invalid SHA might still result in a 200 OK with an empty list from the current service implementation
            var response = await _client.GetAsync($"/api/repos/{TestRepoName}/commits/invalid-commit-sha-abcdef123456/changes");
            response.EnsureSuccessStatusCode(); // Expecting 200 OK
            var content = await response.Content.ReadAsStringAsync();
            var changes = JsonSerializer.Deserialize<List<GitCommitChange>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(changes);
            Assert.Empty(changes);
        }
    }
}
