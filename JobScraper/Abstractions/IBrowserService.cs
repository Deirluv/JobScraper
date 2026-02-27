namespace JobScraper.Abstractions
{
    public interface IBrowserService : IAsyncDisposable
    {
        public Task<string> FetchHtmlAsync(string url);
        public Task ScrollPageAsync();
    }
}
