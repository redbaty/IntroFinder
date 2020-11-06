using System;

namespace IntroFinder.Core.Models
{
    public class FrameFinderOptions
    {
        internal static FrameFinderOptions Default { get; } = new FrameFinderOptions();

        public TimeSpan MinimumIntroTime { get; set; } = new TimeSpan(0, 0, 1, 10);

        public TimeSpan TimeLimit { get; set; } = new TimeSpan(0, 0, 5, 0);

        public int SequenceTolerableSeconds { get; set; } = 10;

        public int? BatchSize { get; set; }

        public bool Recursive { get; set; }

        public bool EnableHardwareAcceleration
        {
            get => MediaHashingOptions.EnableHardwareAcceleration;
            set => MediaHashingOptions.EnableHardwareAcceleration = value;
        }

        public string FFmpegBinaryPath
        {
            get => MediaHashingOptions.FFmpegBinaryPath;
            set => MediaHashingOptions.FFmpegBinaryPath = value;
        }

        public string FFprobeBinaryPath
        {
            get => MediaHashingOptions.FFprobeBinaryPath;
            set => MediaHashingOptions.FFprobeBinaryPath = value;
        }

        public MediaHashingOptions MediaHashingOptions { get; set; } = new MediaHashingOptions();
    }
}