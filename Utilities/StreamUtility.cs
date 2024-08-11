using System.IO;

namespace Exanite.Core.Utilities
{
    public static class StreamUtility
    {
        public static byte[] ReadAsBytesAndDispose(this Stream stream)
        {
            using var input = stream;

            if (stream is MemoryStream inputMemoryStream)
            {
                return inputMemoryStream.ToArray();
            }

            using var memoryStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }

        public static string ReadAsStringAndDispose(this Stream stream)
        {
            using var input = stream;
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
