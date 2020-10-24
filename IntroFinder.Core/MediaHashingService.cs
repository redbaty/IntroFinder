using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using CliWrap;
using IntroFinder.Core.Decoders;
using IntroFinder.Core.Exceptions;
using IntroFinder.Core.Extensions;
using IntroFinder.Core.Models;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace IntroFinder.Core
{
    public class MediaHashingService
    {
        public MediaHashingService(ILogger<MediaHashingService> logger)
        {
            Logger = logger;
        }

        private ILogger<MediaHashingService> Logger { get; }

        private async IAsyncEnumerable<Frame> GetFrames(Media media,
            MediaHashingOptions mediaHashingOptions,
            TimeSpan? timeLimit
        )
        {
            var argumentsList = new List<string>();

            if (timeLimit.HasValue) argumentsList.Add($"-t {timeLimit.Value:hh\\:mm\\:ss\\.fff}");

            if (mediaHashingOptions.EnableHardwareAcceleration) argumentsList.Add("-hwaccel auto");

            argumentsList.Add("-i -");
            argumentsList.Add("-an -s 640x360 -f image2pipe");
            argumentsList.Add("-");

            var arguments = argumentsList.Aggregate((x, y) => $"{x} {y}");
            
            var channel = Channel.CreateUnbounded<Frame>();
            await using var standardInput = File.OpenRead(media.FilePath);
            await using var standardOutput = new FrameStream(new JpegBufferDecoder(channel));
            var standardErrorOutput = new StringBuilder();
            
            Logger.LogDebug("Starting FFmpeg. {@Arguments}", new
            {
                InputFilePath = media.FilePath,
                Arguments = arguments
            });

            var taskResult = Cli.Wrap("ffmpeg")
                .WithStandardInputPipe(PipeSource.FromStream(standardInput))
                .WithStandardOutputPipe(PipeTarget.ToStream(standardOutput))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(standardErrorOutput))
                .WithArguments(arguments)
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync()
                .Task.ContinueWith(t =>
                {
                    channel.Writer.Complete();
                    return t.Result;
                });

            await foreach (var frame in channel.Reader.ReadAllAsync())
            {
                yield return frame;
            }

            var result = await taskResult;
            Logger.LogDebug("FFmpeg finished. {@Data}", result);
            if (result.ExitCode != 0) throw new FfmpegException(standardErrorOutput.ToString());
            var fps = GetFps(standardErrorOutput);
            media.Fps = fps;
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

        private static double GetFps(StringBuilder standardErrorOutput)
        {
            var fpsMatch = Regex.Match(standardErrorOutput.ToString(), "\\d*\\.?\\d* fps");
            var fpsDigits = new string(fpsMatch.Value.Where(i => char.IsDigit(i) || i == '.').ToArray());
            var fps = double.Parse(fpsDigits, CultureInfo.InvariantCulture);
            return fps;
        }
    }
}