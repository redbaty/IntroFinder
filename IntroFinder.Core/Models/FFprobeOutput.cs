using System.Text.Json.Serialization;

namespace IntroFinder.Core.Models
{
    public class FFprobeOutput
    {
        [JsonPropertyName("format")]
        public Format Format { get; set; }
    }
}