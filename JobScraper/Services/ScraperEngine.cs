using JobScraper.Abstractions;
using JobScraper.Models;
using Serilog.Core;

namespace JobScraper.Services
{
    public class ScraperEngine
    {
        private readonly Logger _logger;
        private readonly IBrowserService _browserService;
        private readonly IJobParser _jobParser;
        private readonly IDataStorage _dataStorage;

        private readonly string _baseUrl = "https://www.linkedin.com/jobs/search";


        public ScraperEngine(Logger logger, IBrowserService browserService, IJobParser jobParser, IDataStorage dataStorage)
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
                _logger.Information("Starting scraping process for URL: {Url}", url);
                var html = await _browserService.FetchHtmlAsync(url);
                var jobs = await _jobParser.ParseAsync(html);
                _logger.Information("Parsed {Count} job listings.", jobs.Count);
                await _dataStorage.SaveAsync(jobs);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred during the scraping process.");
            }
            finally
            {
                _logger.Information("Saved job listings to storage.");
                await _browserService.DisposeAsync();
            }
        }
    }
}
