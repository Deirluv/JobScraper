using JobScraper.Abstractions;
using JobScraper.Models;
using JobScraper.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

class PlaywrightExample
{
    public static async Task Main()
    {
        // Configure Serilog for logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        // Set up the host and dependency injection
        var host = Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IBrowserService, PlayWrightBrowserService>();
                services.AddSingleton<IJobParser, LinkedInParser>();
                services.AddSingleton<IDataStorage, FileStorage>();

                services.AddSingleton<ScraperEngine>(); 
            })
            .Build();

        var engine = host.Services.GetRequiredService<ScraperEngine>();
        try
        {
            Log.Information("Starting the scraping process...");
            await engine.RunAsync(new JobSettings
            {
                SearchQuery = GetRequiredInput("Enter the job keywords: "),
                Country = GetRequiredInput("Enter the country: "),
                MaxJobs = int.TryParse(GetRequiredInput("Enter the maximum number of jobs to scrape (default = 25 jobs): "), out int results) ? results : 25
            });
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unhandled exception occurred during scraping.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    static string GetRequiredInput(string message)
    {
        string? input = null;

        while (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine(message);
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Log.Warning("Input cannot be empty. Please try again.");
            }
        }

        return input;
    }
}