using FrameExtractor;
using IntroFinder.Core.Enums;

namespace IntroFinder.Core.Models
{
    public class MediaHashingOptions : FFmpegOptions
    {
        internal static MediaHashingOptions Default { get; } = new();

        public bool EnableHardwareAcceleration { get; set; }

        public bool DumpFiles { get; set; }

        public MediaHashingTypes HashingType { get; set; } = MediaHashingTypes.AverageHash;
    }
}