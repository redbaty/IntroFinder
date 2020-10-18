using System;

namespace IntroFinder.Core.Exceptions
{
    public class FfmpegException : Exception
    {
        public FfmpegException(string message) : base(message)
        {
        }
    }
}