using JobScraper.Models;

namespace JobScraper.Abstractions
{
    public interface IJobParser
    {
        public Task<List<Job>> ParseAsync(string html);
    }
}
