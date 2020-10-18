using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CliWrap;

namespace IntroFinder.Core.Extensions
{
    internal static class DirectoryExtensions
    {
        public static async IAsyncEnumerable<FileInfo> GetVideoFiles(this DirectoryInfo directory)
        {
            foreach (var file in directory.EnumerateFiles())
            {
                if (await IsValidVideoFile(file))
                {
                    yield return file;
                }
            }
        }

        private static async Task<bool> IsValidVideoFile(FileSystemInfo file)
        {
            var result = await Cli.Wrap("ffprobe")
                .WithArguments($"\"{file.FullName}\"")
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

            return result.ExitCode == 0;
        }
    }
}