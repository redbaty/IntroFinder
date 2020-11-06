using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommandLine;
using FrameExtractor;
using IntroFinder.Core;
using IntroFinder.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace IntroFinder.Console
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);
            await parserResult.WithParsedAsync(Run);
        }

        private static async Task Run(Options options)
        {
            var startNew = Stopwatch.StartNew();
            Log.Logger = new LoggerConfiguration().Enrich
                .FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <s:{SourceContext}>{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .MinimumLevel.Override("FrameExtractor.FrameExtractionService", LogEventLevel.Information)
                .CreateLogger();

            var directory = new DirectoryInfo(options.Directory);

            if (!directory.Exists)
            {
                Log.Logger.Error("The directory {directory} does not exist.", options.Directory);
                return;
            }

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<FrameExtractionService>();
            serviceCollection.AddSingleton<MediaHashingService>();
            serviceCollection.AddSingleton<CommonFrameFinderService>();
            serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var frameOptions = new FrameFinderOptions
            {
                TimeLimit = options.TimeLimit, EnableHardwareAcceleration = options.HardwareAcceleration,
                MinimumIntroTime = options.MinimumIntroTime,
                SequenceTolerableSeconds = options.SequenceTolerableSeconds, BatchSize = options.BatchSize,
                Recursive = options.Recursive,
                FFmpegBinaryPath = options.FFmpegPath,
                FFprobeBinaryPath = options.FFprobePath
            };

            var commonFrameFinderService = serviceProvider.GetService<CommonFrameFinderService>();
            var medias = await commonFrameFinderService.FindCommonFrames(directory, frameOptions).ToListAsync();
            var outputFile = Path.Combine(directory.FullName, "intro_index.json");

            Log.Logger.Information("Index has been written at {outputFile}", outputFile);

            var serializedOutput = JsonSerializer.Serialize(medias, new JsonSerializerOptions {WriteIndented = true});
            await File.WriteAllTextAsync(outputFile, serializedOutput);
            startNew.Stop();
            Log.Logger.Information("Process took {@processTime}", startNew.Elapsed);
        }
    }
}