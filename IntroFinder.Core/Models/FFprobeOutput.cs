using System.Text.Json.Serialization;

namespace IntroFinder.Core.Models
{
    internal class FFprobeOutput
    {
        [JsonPropertyName("format")]
        public Format Format { get; set; }
    }
}