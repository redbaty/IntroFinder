using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FrameExtractor;
using IntroFinder.Core.Extensions;
using IntroFinder.Core.Models;
using SixLabors.ImageSharp;

namespace IntroFinder.Core
{
    public class MediaHashingService
    {
        public MediaHashingService(FrameExtractionService frameExtractionService)
        {
            FrameExtractionService = frameExtractionService;
        }

        private FrameExtractionService FrameExtractionService { get; }
        
        private IAsyncEnumerable<Frame> GetFrames(Media media,
            MediaHashingOptions mediaHashingOptions,
            TimeSpan? timeLimit
        )
        {
            return FrameExtractionService.GetFrames(media.FilePath, new FrameExtractionOptions
            {
                TimeLimit = timeLimit,
                FrameSize = new FrameSize{Height = 64, Width = 64},
                FrameFormat = FrameFormat.Jpg,
                EnableHardwareAcceleration = mediaHashingOptions.EnableHardwareAcceleration
            }, fps => media.Fps = fps);
        }

        public async Task<Media> GetMedia(string filePath,
            TimeSpan? timeLimit = null,
            MediaHashingOptions mediaHashingOptions = null)
        {
            mediaHashingOptions ??= MediaHashingOptions.Default;
            var media = new Media
            {
                FilePath = filePath,
                Frames = new List<FrameHash>()
            };

            var hashAlgorithm = mediaHashingOptions.GetHashAlgorithm();
            await foreach (var frame in GetFrames(media, mediaHashingOptions, timeLimit))
            {
                using var image = Image.Load(frame.Data);
                var hash = hashAlgorithm.Hash(image);
                media.Frames.Add(new FrameHash(frame.Position,
                    filePath,
                    hash));

                if (mediaHashingOptions.DumpFiles)
                {
                    var fileName = Path.Combine(Path.GetDirectoryName(filePath)!, "frames",
                        $"frame-{frame.Position}.png");
                    var directory = new DirectoryInfo(Path.GetDirectoryName(fileName)!);
                    if (!directory.Exists)
                        directory.Create();
                    await File.WriteAllBytesAsync(fileName, frame.Data);
                }
            }

            foreach (var frameHash in media.Frames) frameHash.Time = TimeSpan.FromSeconds(frameHash.Frame / media.Fps);

            return media;
        }
    }
}