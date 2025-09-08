using System;
using System.IO;
using System.IO.Compression;

namespace Exanite.Core.Utilities
{
    public static class CompressionUtility
    {
        public static byte[] BrotliCompress(ReadOnlySpan<byte> data)
        {
            using (var outputStream = new MemoryStream())
            using (var brotliStream = new BrotliStream(outputStream, CompressionMode.Compress))
            {
                brotliStream.Write(data);
                brotliStream.Flush();

                return outputStream.ToArray();
            }
        }

        public static byte[] BrotliDecompress(ReadOnlySpan<byte> data)
        {
            using (var inputStream = new MemoryStream())
            using (var outputStream = new MemoryStream())
            using (var brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress))
            {
                inputStream.Write(data);
                inputStream.Seek(0, SeekOrigin.Begin);

                brotliStream.CopyTo(outputStream);

                return outputStream.ToArray();
            }
        }
    }
}
