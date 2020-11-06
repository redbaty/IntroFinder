using IntroFinder.Core.Enums;

namespace IntroFinder.Core.Models
{
    public class MediaHashingOptions
    {
        internal static MediaHashingOptions Default { get; } = new MediaHashingOptions();

        public bool EnableHardwareAcceleration { get; set; }

        public bool DumpFiles { get; set; }

        public MediaHashingTypes HashingType { get; set; } = MediaHashingTypes.AverageHash;

        public string FFmpegBinaryPath { get; set; }

        public string FFprobeBinaryPath { get; set; }
    }
}