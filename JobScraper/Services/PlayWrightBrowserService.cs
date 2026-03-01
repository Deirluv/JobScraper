using JobScraper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace JobScraper.Services
{
    public class PlayWrightBrowserService : IBrowserService
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IPage? _page;

        private readonly ILogger<PlayWrightBrowserService> _logger;

        public PlayWrightBrowserService(ILogger<PlayWrightBrowserService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initializes the Playwright browser instance.
        /// </summary>
        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                SlowMo = 100
            });

            _logger.LogInformation("Playwright browser initialized successfully.");
        }

        /// <summary>
        /// Opens the page and scrolls to ensure all lazy loaded data is rendered, then returns the page HTML.
        /// </summary>
        public async Task<string> FetchHtmlAsync(string url)
        {
            await InitializeAsync();

            var context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
            });


            _page = await context.NewPageAsync();
            await _page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

            await ScrollPageAsync();

            return await _page.ContentAsync();
        }

        /// <summary>
        /// Scrolls the page in increments to trigger lazy loading of content. It scrolls down by 400 pixels every 150ms until it reaches the bottom or a maximum scroll limit (10,000 pixels).
        /// </summary>
        public async Task ScrollPageAsync()
        {
            if (_page == null) return;

            await _page.EvaluateAsync(@"
                async () => {
                    await new Promise((resolve) => {
                        let totalHeight = 0;
                        let distance = 400;
                        let timer = setInterval(() => {
                            let scrollHeight = document.body.scrollHeight;
                            window.scrollBy(0, distance);
                            totalHeight += distance;

                            if(totalHeight >= scrollHeight || totalHeight > 10000){
                                clearInterval(timer);
                                resolve();
                            }
                        }, 150);
                    });
                }"
            );

            // We add a small delay after scrolling to ensure all content is loaded before we fetch the HTML.
            await Task.Delay(1500);
        }

        /// <summary>
        /// Disposes of the Playwright browser and related resources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
                await _browser.DisposeAsync();
            }
            if (_page != null)
            {
                await _page.CloseAsync();
            }
            _playwright?.Dispose();
        }
    }
}
