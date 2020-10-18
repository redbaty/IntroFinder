using System;

namespace Aif.Core.Models
{
    public class FrameFinderOptions
    {
        internal static FrameFinderOptions Default { get; } = new FrameFinderOptions();

        public TimeSpan DefaultIntroTime { get; set; } = new TimeSpan(0, 0, 1, 10);

        public TimeSpan TimeLimit { get; set; } = new TimeSpan(0, 0, 5, 0);

        public int SequenceTorableSeconds { get; set; } = 10;

        public bool EnableHardwareAcceleration { get; set; }
    }
}