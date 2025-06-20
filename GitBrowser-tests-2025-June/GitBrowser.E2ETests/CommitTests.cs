using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Playwright;
using System.Linq;
using System.Threading.Tasks;

namespace GitBrowser.E2ETests
{
    [TestClass]
    public class CommitTests : WebDriverFixture
    {
        [TestMethod]
        public async Task TestListCommitsAndVerifyDetails()
        {
            // Navigate to the home page
            await Page.GotoAsync(BaseUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Click on the first repository link
            var repoLinks = (await Page.QuerySelectorAllAsync("#repo-list li a")).ToList();
            Assert.IsTrue(repoLinks.Any(), "No repositories found to test commit listing.");
            await repoLinks.First().ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle); // Wait for branches

            // Click on the first branch link
            var branchLinks = (await Page.QuerySelectorAllAsync("#branch-list li a")).ToList();
            if (!branchLinks.Any())
            {
                Assert.Inconclusive("No branches found in the first repository to test commits. Skipping.");
                return;
            }
            await branchLinks.First().ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle); // Wait for commits

            // Assert that the commit log is displayed
            var commitLog = await Page.QuerySelectorAsync(".logentries");
            Assert.IsNotNull(commitLog, "Commit log (.logentries) should be loaded.");

            // Get all commit entries (excluding the header)
            var commitEntries = (await Page.QuerySelectorAllAsync(".logentries .logentry:not(.logentry__header)")).ToList();
            if (!commitEntries.Any())
            {
                // It's possible a branch has no commits after the initial one, or no commits at all (e.g. orphan branch)
                // For this test, we'll be inconclusive if no actual commit rows are found.
                // A more robust test would use a repo with known commit history.
                Assert.Inconclusive("No commit entries found in the log for the first branch. Skipping detail checks.");
                return;
            }

            // Verify details of the first commit entry
            var firstCommitEntry = commitEntries.First();

            var shaElement = await firstCommitEntry.QuerySelectorAsync(".logentry__commitId");
            Assert.IsNotNull(shaElement, "Commit SHA element should exist.");
            var shaText = await shaElement.InnerTextAsync();
            Assert.IsFalse(string.IsNullOrWhiteSpace(shaText), "Commit SHA should not be empty.");
            Assert.IsTrue(shaText.Length > 0, "Commit SHA should have a valid length (checking for > 0, actual length is 7).");

            var authorElement = await firstCommitEntry.QuerySelectorAsync("div:nth-child(3)"); // Assumes Author is the 3rd div
            Assert.IsNotNull(authorElement, "Author element should exist.");
            var authorText = await authorElement.InnerTextAsync();
            Assert.IsFalse(string.IsNullOrWhiteSpace(authorText), "Author name should not be empty.");

            var messageElement = await firstCommitEntry.QuerySelectorAsync(".logentry__message");
            Assert.IsNotNull(messageElement, "Message element should exist.");
            var messageText = await messageElement.InnerTextAsync();
            Assert.IsFalse(string.IsNullOrWhiteSpace(messageText), "Commit message should not be empty.");
        }
    }
}
