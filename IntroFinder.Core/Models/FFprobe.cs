using System.Text.Json.Serialization;

namespace IntroFinder.Core.Models
{
    public class Format
    {
        [JsonPropertyName("duration")]
        public string Duration { get; set; }
    }
}