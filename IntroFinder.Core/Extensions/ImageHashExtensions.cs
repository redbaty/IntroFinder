using System;
using System.Collections.Generic;
using System.Linq;
using IntroFinder.Core.Models;

namespace IntroFinder.Core.Extensions
{
    public static class ImageHashExtensions
    {
        public static IEnumerable<Sequence> CreateSequences(this IEnumerable<FrameHash> times, int frameSkip)
        {
            var oldFrame = -1;
            TimeSpan? initialTime = null;
            TimeSpan? endTime = null;

            foreach (var imageWithTime in times.OrderBy(i => i.Frame))
            {
                var maxFrame = oldFrame + frameSkip;

                if (imageWithTime.Frame > maxFrame && initialTime.HasValue)
                {
                    yield return new Sequence(initialTime.Value, endTime.Value);
                    initialTime = imageWithTime.Time;
                }
                
                initialTime ??= imageWithTime.Time;
                endTime = imageWithTime.Time;
                oldFrame = imageWithTime.Frame;
            }

            if (initialTime.HasValue)
                yield return new Sequence(initialTime.Value, endTime.Value);
        }
    }
}