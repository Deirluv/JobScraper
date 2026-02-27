namespace JobScraper.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Source { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location {  get; set; } = string.Empty;
        public string ApplicantsCount {  get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Responsibilities { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public string Industries { get; set; } = string.Empty;
        public string PostedDate { get; set; } = string.Empty; 
        public DateTime ScrapedAt { get; set; } = DateTime.UtcNow; // The date when the scraper found the vacancy
        public string Link { get; set; } = string.Empty;
    }
}
