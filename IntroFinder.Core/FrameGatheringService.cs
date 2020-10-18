using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aif.Core.Exceptions;
using Aif.Core.Models;
using CliWrap;
using CoenM.ImageHash.HashAlgorithms;
using Microsoft.Extensions.Logging;
using Image = SixLabors.ImageSharp.Image;

namespace Aif.Core
{
    public class FrameGatheringService
    {
        private static readonly AverageHash HashAlgorithm = new AverageHash();

        public FrameGatheringService(ILogger<FrameGatheringService> logger)
        {
            Logger = logger;
        }

        private ILogger<FrameGatheringService> Logger { get; }

        public async Task<Media> GetMedia(string filePath,
            TimeSpan? timeLimit = null,
            bool enableHardwareAcceleration = false,
            bool dumpFiles = false)
        {
            var argumentsList = new List<string>();

            if (timeLimit.HasValue) argumentsList.Add($"-t {timeLimit.Value:hh\\:mm\\:ss\\.fff}");

            if (enableHardwareAcceleration) argumentsList.Add("-hwaccel auto");

            argumentsList.Add("-i -");
            argumentsList.Add("-an -s 640x360 -f image2pipe");
            argumentsList.Add("-");

            var arguments = argumentsList.Aggregate((x, y) => $"{x} {y}");
            var frameHashes = new List<FrameHash>();

            await using var input = File.OpenRead(filePath);
            var standardErrorOutput = new StringBuilder();
            await using var standardOutput = new FrameStream((frame, data) =>
            {
                using var image = Image.Load(data);
                var hash = HashAlgorithm.Hash(image);
                frameHashes.Add(new FrameHash(frame,
                    filePath,
                    hash));

                if (dumpFiles)
                {
                    var fileName = Path.Combine(Path.GetDirectoryName(filePath)!, "frames",
                        $"frame-{frame}.png");
                    var directory = new DirectoryInfo(Path.GetDirectoryName(fileName)!);
                    if (!directory.Exists)
                        directory.Create();
                    File.WriteAllBytes(fileName, data);
                }
            });

            Logger.LogDebug("Starting FFmpeg. {@Arguments}", new
            {
                InputFilePath = filePath,
                Arguments = arguments
            });

            var result = await Cli.Wrap("ffmpeg")
                .WithStandardInputPipe(PipeSource.FromStream(input))
                .WithStandardOutputPipe(PipeTarget.ToStream(standardOutput))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(standardErrorOutput))
                .WithArguments(arguments)
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

            Logger.LogDebug("FFmpeg finished. {@Data}", result);

            if (result.ExitCode != 0) throw new FfmpegException(standardErrorOutput.ToString());

            var fps = GetFps(standardErrorOutput);

            foreach (var frameHash in frameHashes)
            {
                var second = frameHash.Frame / fps;
                var time = TimeSpan.FromSeconds(second);
                frameHash.Time = time;
            }

            var media = new Media(filePath, fps, frameHashes);

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