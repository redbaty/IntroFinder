using System;
using System.IO;
using IntroFinder.Core.Decoders;

namespace IntroFinder.Core.Models
{
    internal class FrameStream : Stream
    {
        public FrameStream(IFrameBufferDecoder decoder)
        {
            Decoder = decoder;
        }


        public override bool CanRead { get; } = false;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = true;
        public override long Length { get; } = 0;
        public override long Position { get; set; }
        private IFrameBufferDecoder Decoder { get; }


        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            Decoder.Write(buffer[..count]);
        }
    }
}