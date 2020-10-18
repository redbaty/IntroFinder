using System.Threading.Channels;
using IntroFinder.Core.Models;

namespace IntroFinder.Core.Decoders
{
    internal interface IFrameBufferDecoder
    {
        ChannelWriter<Frame> ChannelWriter { get; }
        void Write(byte[] buffer);
    }
}