namespace JobScraper.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string JobCompany { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string JobLocation {  get; set; } = string.Empty;
        public string ApplicantsCount {  get; set; } = string.Empty;
        public string JobLevel { get; set; } = string.Empty;
        public string JobResponsibilities { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public string Industries { get; set; } = string.Empty;
        public string PostedDate { get; set; } = string.Empty; 
        public DateTime ScrapedAt { get; set; } = DateTime.UtcNow; // The date when the scraper found the vacancy
        public string Link { get; set; } = string.Empty;
    }
}
