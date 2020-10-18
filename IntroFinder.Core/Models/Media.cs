using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IntroFinder.Core.Models
{
    public class Media
    {
        public string FilePath { get; set; }

        [JsonIgnore]
        public List<FrameHash> Frames { get; set; }

        public double Fps { get; set; }

        public Sequence Intro { get; set; }
    }
}