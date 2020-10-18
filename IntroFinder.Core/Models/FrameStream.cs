using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IntroFinder.Core.Models
{
    public class FrameStream : Stream
    {
        public FrameStream(Action<int, byte[]> onImageDetected)
        {
            OnImageDetected = onImageDetected;
        }

        private int CurrentFrame { get; set; }
        private byte[] LastBuffer { get; set; }
        private Action<int, byte[]> OnImageDetected { get; }

        private static byte[] StartSiganture { get; } = {255, 216, 255, 224};
        private static byte[] EndSiganture { get; } = {255, 217};

        public override bool CanRead { get; } = false;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = true;
        public override long Length => CurrentFrame;
        public override long Position { get; set; }

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

        private static int SearchBytes(byte[] haystack, byte[] needle)
        {
            if (haystack == null)
                throw new ArgumentNullException(nameof(haystack));

            var len = needle.Length;
            var limit = haystack.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                    if (needle[k] != haystack[i + k])
                        break;

                if (k == len) return i;
            }

            return -1;
        }

        private static IEnumerable<int> SearchBytesRecursive(byte[] haystack, byte[] needle)
        {
            var initialIndex = SearchBytes(haystack, needle);

            if (initialIndex < 0)
                yield break;


            while (true)
            {
                var bytes = haystack[initialIndex..haystack.Length];
                var searchBytes = SearchBytes(bytes, needle);

                if (searchBytes < 0)
                    break;

                initialIndex += searchBytes + 1;


                yield return initialIndex - 1;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            buffer = buffer[..count];
            var starts = SearchBytesRecursive(buffer, StartSiganture).ToArray();
            var ends = SearchBytesRecursive(buffer, EndSiganture).ToArray();

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

                if (ends.Length == 0 && starts.Length == 0)
                {
                    return;
                }
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
            OnImageDetected?.Invoke(CurrentFrame, data);
            CurrentFrame++;
        }
    }
}