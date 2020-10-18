using System;
using CommandLine;

namespace IntroFinder.Console
{
    public class Options
    {
        [Option("hwaccel", Required = false, Default = false,
            HelpText =
                "Sets hwaccel to auto while calling FFmpeg. This might offset the load to the GPU, but is known to cause some trouble.")]
        public bool HardwareAcceleration { get; set; }

        [Option("tseconds", Required = false, Default = 10,
            HelpText =
                "Sets the number of seconds that will be tolerated while determining an intro sequence.")]
        public int SequenceTolerableSeconds { get; set; }

        [Option("tmin", Required = false,
            HelpText =
                "(Default: 1 minute and 10 seconds) Sets the minimum time required for a sequence to be considered an intro.")]
        public TimeSpan MinimumIntroTime { get; set; } = new TimeSpan(0, 0, 1, 10);

        [Option("tlimit", Required = false,
            HelpText =
                "(Default: 5 seconds) Sets the maximum duration extracted from the video files. This heavily impacts performance.")]
        public TimeSpan TimeLimit { get; set; } = new TimeSpan(0, 0, 5, 0);

        [Value(0, MetaName = nameof(Directory), Required = true,
            HelpText = "The directory containing the video files.")]
        public string Directory { get; set; }
    }
}