using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaywrightSharp;
using System.IO;
using System.Threading.Tasks;

namespace GitBrowser.E2ETests
{
    [TestClass]
    public class WebDriverFixture
    {
        protected static IPlaywright Playwright { get; private set; }
        protected static IBrowser Browser { get; private set; }
        protected static IPage Page { get; private set; }
        protected static string BaseUrl { get; private set; }

        [AssemblyInitialize]
        public static async Task AssemblyInitialize(TestContext context)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            BaseUrl = configuration["BaseUrlForE2ETesting"];

            Playwright = await PlaywrightSharp.Playwright.CreateAsync();
            Browser = await Playwright.Chromium.LaunchAsync(new LaunchOptions { Headless = true }); // Use Headless = false for debugging
            Page = await Browser.NewPageAsync();
        }

        [AssemblyCleanup]
        public static async Task AssemblyCleanup()
        {
            if (Page != null)
            {
                await Page.CloseAsync();
            }
            if (Browser != null)
            {
                await Browser.CloseAsync();
            }
            Playwright?.Dispose();
        }

        [TestInitialize]
        public virtual async Task TestInitialize()
        {
            // Optional: Add per-test setup, e.g., navigate to a default page
            // await Page.GoToAsync(BaseUrl);
        }

        [TestCleanup]
        public virtual async Task TestCleanup()
        {
            // Optional: Add per-test cleanup
        }
    }
}
