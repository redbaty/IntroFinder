using System;

namespace IntroFinder.Core.Models
{
    public class FrameHash
    {
        public FrameHash(int frame, string filePath, ulong hash)
        {
            Frame = frame;
            FilePath = filePath;
            Hash = hash;
        }

        public TimeSpan Time { get; set; }

        public int Frame { get; }

        public string FilePath { get; }

        public ulong Hash { get; }
    }
}