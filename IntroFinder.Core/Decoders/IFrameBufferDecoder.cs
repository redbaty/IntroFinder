using System.Threading.Channels;
using System.Threading.Tasks;
using IntroFinder.Core.Models;

namespace IntroFinder.Core.Decoders
{
    internal interface IFrameBufferDecoder
    {
        ChannelWriter<Frame> ChannelWriter { get; }
        
        Task WriteAsync(byte[] buffer);
    }
}