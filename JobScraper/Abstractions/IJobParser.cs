using JobScraper.Models;

namespace JobScraper.Abstractions
{
    public interface IJobParser
    {
        public string JobSource { get; }

        public Task<List<Job>> ParseAsync(string html);
    }
}
