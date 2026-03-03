namespace JobScraper.Abstractions
{
    public interface IBrowserService : IAsyncDisposable
    {
        public Task<string> FetchHtmlAsync(string url, int maxJobs);
        public Task ScrollPageAsync(int maxJobs);
    }
}
