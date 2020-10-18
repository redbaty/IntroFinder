using System;

namespace Aif.Core.Exceptions
{
    public class FfmpegException : Exception
    {
        public FfmpegException(string message) : base(message)
        {
        }
    }
}