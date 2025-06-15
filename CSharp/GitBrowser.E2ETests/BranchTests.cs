using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaywrightSharp;
using System.Linq;
using System.Threading.Tasks;

namespace GitBrowser.E2ETests
{
    [TestClass]
    public class BranchTests : WebDriverFixture
    {
        [TestMethod]
        public async Task TestListBranchesAndNavigateToCommits()
        {
            // Navigate to the home page
            await Page.GoToAsync(BaseUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Click on the first repository link (assuming at least one repo exists)
            var repoLinks = await Page.QuerySelectorAllAsync("#repo-list li a");
            Assert.IsTrue(repoLinks.Any(), "No repositories found to test branch listing.");
            await repoLinks.First().ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle); // Wait for branches to load

            // Assert that the branch list is displayed
            var branchList = await Page.QuerySelectorAsync("#branch-list");
            Assert.IsNotNull(branchList, "Branch list (#branch-list) should be loaded.");

            var branchLinks = await Page.QuerySelectorAllAsync("#branch-list li a");
            if (!branchLinks.Any())
            {
                // If there are no branches, we can't test navigation to commits.
                // This might be a valid state for some repos.
                // Consider adding a specific test repo with known branches for more robust testing.
                Assert.Inconclusive("No branches found in the first repository to test commit navigation. Skipping further checks.");
                return;
            }

            // Get the name of the first branch
            var firstBranchLink = branchLinks.First();
            var branchName = await firstBranchLink.TextContentAsync();
            Assert.IsFalse(string.IsNullOrEmpty(branchName), "Branch name should not be empty.");

            // Click on the first branch link
            await firstBranchLink.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle); // Wait for commits to load

            // Assert that the commit log for the selected branch is loaded
            var commitLog = await Page.QuerySelectorAsync(".logentries"); // Using class from _LogPartial.cshtml
            Assert.IsNotNull(commitLog, "Commit log (.logentries) should be loaded after clicking a branch.");

            var commitEntries = await Page.QuerySelectorAllAsync(".logentries .logentry");
            // Check if header row + at least one commit is present, or just header if no commits
            Assert.IsTrue(commitEntries.Count > 0, "Commit log should have at least a header row.");
        }
    }
}
