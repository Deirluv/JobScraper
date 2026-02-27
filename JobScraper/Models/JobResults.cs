namespace JobScraper.Models
{
    public class JobResults
    {
        public List<Job> Results { get; set; } = new List<Job>();
        public int TotalFound => Results.Count;
        public TimeSpan ExecutionTime { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
