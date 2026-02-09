namespace JobScraper.Abstractions
{
    public interface IBrowserParser
    {
        public Task<string> Parse(string url);
        public Task ScrollBackAsync();
    }
}
