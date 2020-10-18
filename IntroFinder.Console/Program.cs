using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IntroFinder.Core;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace IntroFinder.Console
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            if (args.Length <= 0)
            {
                Log.Logger.Error("Please provide a directory.");
                return;
            }

            var rawDirectory = args[0];
            var directory = new DirectoryInfo(rawDirectory);

            if (!directory.Exists)
            {
                Log.Logger.Error("The directory {directory} does not exist.", rawDirectory);
                return;
            }

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<FrameGatheringService>();
            serviceCollection.AddSingleton<CommonFrameFinderService>();
            serviceCollection.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var commonFrameFinderService = serviceProvider.GetService<CommonFrameFinderService>();
            var medias = await commonFrameFinderService.FindCommonFrames(directory).ToListAsync();
            var x = JsonSerializer.Serialize(medias);
        }
    }
}