using JobScraper.Abstractions;
using JobScraper.Models;
using Microsoft.Extensions.Logging;

namespace JobScraper.Services
{
    public class ScraperEngine
    {
        private readonly ILogger<ScraperEngine> _logger;
        private readonly IBrowserService _browserService;
        private readonly IJobParser _jobParser;
        private readonly IDataStorage _dataStorage;

        private readonly string _baseUrl = "https://www.linkedin.com/jobs/search";


        public ScraperEngine(ILogger<ScraperEngine> logger, IBrowserService browserService, IJobParser jobParser, IDataStorage dataStorage)
        {
            _logger = logger;
            _browserService = browserService;
            _jobParser = jobParser;
            _dataStorage = dataStorage;
        }

        public async Task RunAsync(JobSettings jobSettings)
        {
            try
            {
                var url = $"{_baseUrl}?keywords={Uri.EscapeDataString(jobSettings.SearchQuery)}&location={Uri.EscapeDataString(jobSettings.Country)}";
                _logger.LogInformation("Starting scraping process for URL: {Url}", url);
                var html = await _browserService.FetchHtmlAsync(url);
                var jobs = await _jobParser.ParseAsync(html);
                _logger.LogInformation("Parsed {Count} job listings.", jobs.Count);
                await _dataStorage.SaveAsync(jobs, jobSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the scraping process.");
            }
            finally
            {
                _logger.LogInformation("Saved job listings to storage.");
                await _browserService.DisposeAsync();
            }
        }
    }
}
