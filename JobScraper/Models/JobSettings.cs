namespace JobScraper.Models
{
    public class JobSettings
    {
        public string SearchQuery { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int MaxJobs { get; set; } = 25;
    }
}
