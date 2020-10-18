using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IntroFinder.Core.Models
{
    public partial class FFprobeOutput
    {
        [JsonPropertyName("format")]
        public Format Format { get; set; }
    }

    public partial class Format
    {
        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("nb_streams")]
        public long NbStreams { get; set; }

        [JsonPropertyName("nb_programs")]
        public long NbPrograms { get; set; }

        [JsonPropertyName("format_name")]
        public string FormatName { get; set; }

        [JsonPropertyName("format_long_name")]
        public string FormatLongName { get; set; }

        [JsonPropertyName("start_time")]
        public string StartTime { get; set; }

        [JsonPropertyName("duration")]
        public string Duration { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("bit_rate")]
        public string BitRate { get; set; }

        [JsonPropertyName("probe_score")]
        public long ProbeScore { get; set; }

        [JsonPropertyName("tags")]
        public FormatTags Tags { get; set; }
    }

    public partial class FormatTags
    {
        [JsonPropertyName("encoder")]
        public string Encoder { get; set; }

        [JsonPropertyName("creation_time")]
        public DateTimeOffset CreationTime { get; set; }
    }
}