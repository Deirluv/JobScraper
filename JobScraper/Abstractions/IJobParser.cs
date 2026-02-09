using JobScraper.Models;

namespace JobScraper.Abstractions
{
    public interface IJobParser
    {
        public List<Job> Parse(string html);
    }
}
