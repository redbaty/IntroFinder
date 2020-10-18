using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IntroFinder.Core.Extensions;
using IntroFinder.Core.Models;
using Microsoft.Extensions.Logging;

namespace IntroFinder.Core
{
    public class CommonFrameFinderService
    {
        public CommonFrameFinderService(MediaHashingService mediaHashingService,
            ILogger<CommonFrameFinderService> logger)
        {
            MediaHashingService = mediaHashingService;
            Logger = logger;
        }

        private MediaHashingService MediaHashingService { get; }

        private ILogger<CommonFrameFinderService> Logger { get; }

        public async IAsyncEnumerable<Media> FindCommonFrames(DirectoryInfo directory,
            FrameFinderOptions options = null)
        {
            options ??= FrameFinderOptions.Default;
            var tasks = await directory.GetVideoFiles()
                .Select(videoFile =>
                    MediaHashingService.GetMedia(videoFile.FullName, options.TimeLimit, options.MediaHashingOptions))
                .ToListAsync();

            var results = await Task.WhenAll(tasks);
            var forEachFile = results.SelectMany(i => i.Frames)
                .GroupBy(i => new {i.Hash, i.FilePath})
                .Select(i => i.First())
                .GroupBy(i => i.Hash)
                .Where(i => i.Count() > 1)
                .SelectMany(i => i)
                .GroupBy(i => i.FilePath);

            foreach (var imageHashes in forEachFile)
            {
                var media = results.Single(i => i.FilePath == imageHashes.Key);
                var introSequence = imageHashes
                    .CreateSequences((int) (media.Fps * options.SequenceTolerableSeconds))
                    .SingleOrDefault(i => i.Duration >= options.MinimumIntroTime);
                media.Intro = introSequence;

                if (introSequence == null)
                {
                    Logger.LogError("Could not find intro for file {file}!", imageHashes.Key);
                    continue;
                }

                Logger.LogInformation(
                    "File '{file}' intro starts in {@introStart} ends {@introEnds} with delta {@introDuration}",
                    imageHashes.Key,
                    introSequence.Start,
                    introSequence.End,
                    introSequence.Duration);

                yield return media;
            }
        }
    }
}