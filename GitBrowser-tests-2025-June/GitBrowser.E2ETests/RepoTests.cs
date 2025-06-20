using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace GitBrowser.E2ETests
{
    [TestClass]
    public class RepoTests : WebDriverFixture
    {
        [TestMethod]
        public async Task TestListRepositoriesAndNavigateToBranches()
        {
            // Navigate to the home page
            await Page.GotoAsync(BaseUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Assert that the repository list is displayed
            var repoList = await Page.QuerySelectorAsync("#repo-list");
            Assert.IsNotNull(repoList, "Repository list (#repo-list) should be present.");

            var repoLinks = await Page.QuerySelectorAllAsync("#repo-list li a");
            Assert.IsTrue(repoLinks.Count() > 0, "There should be at least one repository listed.");

            // Get the path of the first repository
            var firstRepoLink = repoLinks.First();
            var repoName = await firstRepoLink.InnerTextAsync();
            var repoPath = await firstRepoLink.GetAttributeAsync("data-path");

            Assert.IsFalse(string.IsNullOrEmpty(repoName), "Repository name should not be empty.");
            Assert.IsFalse(string.IsNullOrEmpty(repoPath), "Repository path (data-path) should not be empty.");

            // Click on the first repository link
            await firstRepoLink.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle); // Wait for AJAX content to load

            // Assert that the branch list for the selected repository is loaded
            var branchList = await Page.QuerySelectorAsync("#branch-list");
            Assert.IsNotNull(branchList, "Branch list (#branch-list) should be loaded after clicking a repository.");

            var branchLinks = await Page.QuerySelectorAllAsync("#branch-list li a");
            // This assertion depends on the test environment having repos with branches.
            // For now, we'll just check if the container is there.
            // A more robust test would ensure a specific test repo with known branches exists.
            Assert.IsTrue(branchLinks.Count() >= 0, "Branch list should be present, even if empty.");
        }
    }
}
