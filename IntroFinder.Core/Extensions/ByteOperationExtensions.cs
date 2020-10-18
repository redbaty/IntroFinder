using System;
using System.Collections.Generic;

namespace IntroFinder.Core.Extensions
{
    internal static class ByteOperationExtensions
    {
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

        public static IEnumerable<int> SearchBytesRecursive(this byte[] haystack, byte[] needle)
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
    }
}