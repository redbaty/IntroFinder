using System;
using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using IntroFinder.Core.Enums;
using IntroFinder.Core.Models;

namespace IntroFinder.Core.Extensions
{
    internal static class MediaHashingOptionsExtensions
    {
        public static IImageHash GetHashAlgorithm(this MediaHashingOptions options) =>
            options.HashingType switch
            {
                MediaHashingTypes.AverageHash => new AverageHash(),
                MediaHashingTypes.PerceptualHash => new PerceptualHash(),
                _ => throw new ArgumentOutOfRangeException(nameof(options))
            };
    }
}