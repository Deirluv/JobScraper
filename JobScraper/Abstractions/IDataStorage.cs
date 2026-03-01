using JobScraper.Models;

namespace JobScraper.Abstractions
{
    public interface IDataStorage
    {
        public Task SaveAsync(List<Job> jobs, JobSettings jobInfo);
    }
}
