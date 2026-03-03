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
                SlowMo = Random.Shared.Next(50, 150) // Human behavior simulation. Random delay for every action.
            });

            if(_browser == null)
            {
                _logger.LogError("Failed to initialize Playwright browser.");
                throw new Exception("Playwright browser initialization failed.");
            }

            _logger.LogInformation("Playwright browser initialized successfully.");
        }

        /// <summary>
        /// Opens the page and scrolls to ensure all lazy loaded data is rendered, then returns the page HTML.
        /// </summary>
        public async Task<string> FetchHtmlAsync(string url, int maxJobs)
        {
            await InitializeAsync();

            var context = await _browser!.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
            });


            _page = await context.NewPageAsync();
            await _page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

            await ScrollPageAsync(maxJobs);

            return await _page.ContentAsync();
        }

        /// <summary>
        /// Scrolls the page in increments to trigger lazy loading of content. It scrolls down by 400 pixels every 200ms until it reaches the required number of jobs, the bottom or the maximum scroll limit (20,000 pixels).
        /// </summary>
        public async Task ScrollPageAsync(int maxJobs)
        {
            if (_page == null) return;

            await _page.EvaluateAsync(@"
                async (limit) => {
                    await new Promise((resolve) => {
                        let totalHeight = 0;
                        const MAX_SCROLL_LIMIT = 50000; // Maximum scroll limit in pixels

                        const scrollStep = () => {
                            // Count the number of job cards currently loaded on the page (li inside the jobs search list)
                            let cardsCount = document.querySelectorAll('.jobs-search__results-list li').length;


                            let showMoreButton = document.querySelector('button[data-tracking-control-name=""infinite-scroller_show-more""], .infinite-scroller__show-more-button');
                            if (showMoreButton && showMoreButton.offsetParent !== null) {
                                showMoreButton.click(); // To click the 'See more jobs' button if it exists. After +- 120 jobs, LinkedIn shows this button to load more jobs instead of infinite scrolling.
                                let loadingDelay = Math.floor(Math.random() * 1000) + 1000; // Waiting for new jobs to load after clicking the button.
                                setTimeout(scrollStep, loadingDelay);
                                return;
                            }

                            let distance = Math.floor(Math.random() * 200) + 300; // Random distance between 300 and 500 pixels
                            window.scrollBy(0, distance);
                            totalHeight += distance;

                            let isBottom = (window.innerHeight + window.scrollY) >= document.body.scrollHeight;

                            if(cardsCount >= limit || isBottom || totalHeight > MAX_SCROLL_LIMIT){
                                resolve();
                            }
                            else {
                                let randomDelay = Math.floor(Math.random() * 250) + 150;
                                setTimeout(scrollStep, randomDelay);
                            }
                        };
                        scrollStep();   
                    });
                }"
            , maxJobs);

            // A small delay after scrolling to ensure all content is loaded before we fetch the HTML.
            await Task.Delay(2000);
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
