using System;
using System.IO;

namespace ForieroEngine.Extensions.Zip
{
    public static class ZipExtensions
    {
        public static byte[] GetAllBytes(this MemoryStream stream)
        {
            byte[] result = new byte[stream.Length];
            Array.Copy(stream.ToArray(), result, (int)stream.Length);
            return result;
        }

        public static byte[] RemoveZeros(this byte[] bytes)
        {
            var i = bytes.Length - 1;
            while (bytes[i] == 0)
            {
                --i;
            }

            var temp = new byte[i + 1];
            Array.Copy(bytes, temp, i + 1);
            return temp;
        }
    }
}