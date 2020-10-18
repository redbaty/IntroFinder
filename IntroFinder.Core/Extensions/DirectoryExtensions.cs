using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CliWrap;
using IntroFinder.Core.Models;

namespace IntroFinder.Core.Extensions
{
    internal static class DirectoryExtensions
    {
        public static async IAsyncEnumerable<List<FileInfo>> Batch(this IAsyncEnumerable<FileInfo> fileEnumerable, int batchSize)
        {
            var currentBatchSize = 0;
            var currentBatch = new List<FileInfo>();

            await foreach (var fileInfo in fileEnumerable)
            {
                currentBatch.Add(fileInfo);
                currentBatchSize++;

                if (currentBatchSize == batchSize)
                {
                    yield return currentBatch;
                    currentBatch.Clear();
                }
            }
        }
        
        public static async IAsyncEnumerable<FileInfo> GetVideoFiles(this DirectoryInfo directory, FrameFinderOptions frameFinderOptions)
        {
            foreach (var file in directory.EnumerateFiles())
            {
                if (await IsValidVideoFile(file, frameFinderOptions))
                {
                    yield return file;
                }
            }
        }

        private static async Task<bool> IsValidVideoFile(FileSystemInfo file, FrameFinderOptions frameFinderOptions)
        {
           var standardOutput = new StringBuilder();
            var standardErrorOutput = new StringBuilder();
            
            var result = await Cli.Wrap("ffprobe")
                .WithArguments($"-print_format json -show_format -show_streams \"{file.FullName}\"")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(standardOutput))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(standardErrorOutput))
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

            if (result.ExitCode == 0)
            {
                var json = standardOutput.ToString();
                var parsed = JsonSerializer.Deserialize<FFprobeOutput>(json);

                if (parsed.Format.Duration == null)
                {
                    return false;
                }
                
                var parsedDuration = double.Parse(parsed.Format.Duration, CultureInfo.InvariantCulture);
                var duration = TimeSpan.FromSeconds(parsedDuration);
                return duration > frameFinderOptions.MinimumIntroTime;
            }

            return false;
        }
    }
}