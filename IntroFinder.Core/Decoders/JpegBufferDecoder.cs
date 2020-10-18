using System.Linq;
using System.Threading.Channels;
using IntroFinder.Core.Extensions;
using IntroFinder.Core.Models;

namespace IntroFinder.Core.Decoders
{
    internal class JpegBufferDecoder : IFrameBufferDecoder
    {
        public JpegBufferDecoder(ChannelWriter<Frame> channelWriter)
        {
            ChannelWriter = channelWriter;
        }

        private int CurrentFrame { get; set; }
        private byte[] LastBuffer { get; set; }
        private static byte[] StartSignature { get; } = {255, 216, 255, 224};
        private static byte[] EndSignature { get; } = {255, 217};

        public ChannelWriter<Frame> ChannelWriter { get; }

        public void Write(byte[] buffer)
        {
            var starts = buffer.SearchBytesRecursive(StartSignature).ToArray();
            var ends = buffer.SearchBytesRecursive(EndSignature).ToArray();

            if (LastBuffer != null)
            {
                var minIndex = new[] {starts.DefaultIfEmpty().First(), ends.DefaultIfEmpty().First()}.Min();
                var concat = LastBuffer.Concat(buffer[..minIndex]).ToArray();
                CreateImage(concat);
                LastBuffer = null;

                if (starts.Length > ends.Length)
                    starts = starts[1..];
                else if (ends.Length > starts.Length)
                    ends = ends[1..];

                if (ends.Length == 0 && starts.Length == 0) return;
            }

            var max = starts.Length > ends.Length ? ends.Length : starts.Length;

            for (var index = 0; index < max; index++)
            {
                var start = starts[index];
                var end = ends[index];
                CreateImage(buffer[start..end]);
            }

            if (ends.Length != starts.Length)
            {
                var maxIndex = starts.Length > ends.Length ? starts.Last() : ends.Last();
                LastBuffer = buffer[maxIndex..];
            }
        }

        private void CreateImage(byte[] data)
        {
            ChannelWriter.TryWrite(new Frame(data, CurrentFrame));
            CurrentFrame++;
        }
    }
}