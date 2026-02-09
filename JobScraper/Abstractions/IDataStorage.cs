using JobScraper.Models;

namespace JobScraper.Abstractions
{
    internal interface IDataStorage
    {
        public Task SaveAsync(List<Job> vacancies);
    }
}
