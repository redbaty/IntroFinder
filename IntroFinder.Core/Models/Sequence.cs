using System;
using System.Text.Json.Serialization;
using IntroFinder.Core.Converters;

namespace IntroFinder.Core.Models
{
    public class Sequence
    {
        [JsonConverter(typeof(TimespanStringConverter))]
        public TimeSpan Start { get; }

        [JsonConverter(typeof(TimespanStringConverter))]
        public TimeSpan End { get; }

        [JsonConverter(typeof(TimespanStringConverter))]
        public TimeSpan Duration => End - Start;

        public Sequence(TimeSpan start, TimeSpan end)
        {
            Start = start;
            End = end;
        }
    }
}