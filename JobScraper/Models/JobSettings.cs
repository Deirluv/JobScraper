namespace JobScraper.Models
{
    public class JobSettings
    {
        public string SearchQuery { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int MaxPages { get; set; }
        public List<string> KeyWords { get; set; } = new List<string>();
    }
}
