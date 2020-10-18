using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Aif.Core.Models
{
    public class Media
    {
        public string FilePath { get; }

        [JsonIgnore]
        public List<FrameHash> Frames { get; }

        public double Fps { get; }

        public Sequence Intro { get; set; }

        public Media(string filePath, double fps, List<FrameHash> frames)
        {
            FilePath = filePath;
            Fps = fps;
            Frames = frames;
        }
    }
}