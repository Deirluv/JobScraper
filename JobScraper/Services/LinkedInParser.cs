using AngleSharp;
using JobScraper.Abstractions;
using JobScraper.Models;

namespace JobScraper.Services
{
    public class LinkedInParser : IJobParser
    {
        /// <summary>
        /// Parses the provided HTML content to extract job information and returns a list of Jobs.
        /// </summary>

        public async Task<List<Job>> ParseAsync(string html)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(req => req.Content(html));

            var jobs = new List<Job>();

            if (document == null || document.Body == null)
            {
                return jobs;
            }

            var jobContainer = document.QuerySelector(".jobs-search__results-list");

            if(jobContainer != null)
            {
                var cardsList = jobContainer.QuerySelectorAll("li");

                foreach (var card in cardsList)
                {
                    var timeTag = card.QuerySelector(".job-search-card__listdate--new");
                    var job = new Job
                    {
                        Title = card.QuerySelector(".base-search-card__title")?.TextContent.Trim() ?? "N/A",
                        Company = card.QuerySelector(".base-search-card__subtitle")?.TextContent.Trim() ?? "N/A",
                        Location = card.QuerySelector(".job-search-card__location")?.TextContent.Trim() ?? "N/A",
                        Link = card.QuerySelector("a")?.GetAttribute("href") ?? "N/A",
                        Source = "LinkedIn",
                        PostedDate = timeTag?.GetAttribute("datetime")?.Trim() + " (" + timeTag?.TextContent.Trim() + ")",
                        ScrapedAt = DateTime.UtcNow
                    };
                    job.ExternalId = card.QuerySelector("div")?.GetAttribute("data-entity-urn") is string urn
                        ? ExtractExternalId(urn)
                        : "N/A";
                    jobs.Add(job);
                }
            }
            return jobs;
        }

        /// <summary>
        /// Extracts the external ID from URN string. It looks for the last ':' symbol and returns the cut version that contains the ID
        /// </summary>

        public string ExtractExternalId(string urn)
        {
            var index = urn.LastIndexOf(':');
            if (index != -1 && index + 1 < urn.Length)
            {
                return urn.Substring(index + 1);
            }
            return string.Empty;
        }
    }
}
