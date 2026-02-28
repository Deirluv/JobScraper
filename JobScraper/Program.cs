using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using Serilog;
using System.Threading.Tasks;

class PlaywrightExample
{
    public static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
    }
}