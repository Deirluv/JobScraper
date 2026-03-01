using JobScraper.Abstractions;
using JobScraper.Models;
using Microsoft.Extensions.Logging;

namespace JobScraper.Services
{
    public class FileStorage : IDataStorage
    {
        private readonly string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private const string folderName = "ScrapedResults";

        private readonly ILogger<FileStorage> _logger;

        public FileStorage(ILogger<FileStorage> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Saves the provided list of jobs to a JSON file in the user's Documents. The file is named with a timestamp and optional search query and country information from the JobSettings.
        /// </summary>

        public async Task SaveAsync(List<Job> jobs, JobSettings jobInfo)
        {
            if(jobs == null || jobs.Count == 0)
            {
                _logger.LogWarning("No jobs to save. Skipping file storage.");
                return;
            }

            try
            {
                var targetDirectory = Path.Combine(filePath, folderName);

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                var fileNamePart = $"ScrapedJobs";

                if (jobInfo != null)
                {
                    var fixedSearchQuery = jobInfo.SearchQuery.Replace(" ", "_").Trim('_');
                    var fixedCountry = jobInfo.Country.Replace(" ", "_").Trim('_');

                    fileNamePart += $"_{fixedSearchQuery}_{fixedCountry}";
                }

                var fullFileName = $"{fileNamePart}_{DateTime.Now:yyyyMMdd_HHmmss}.json";

                var fullPath = Path.Combine(targetDirectory, fullFileName);

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                _logger.LogInformation("Serializing {Count} jobs to JSON format.", jobs.Count);
                var json = System.Text.Json.JsonSerializer.Serialize(jobs, options);
                _logger.LogInformation("Saving {Count} jobs to file: {FilePath}", jobs.Count, fullPath);
                await File.WriteAllTextAsync(fullPath, json);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
