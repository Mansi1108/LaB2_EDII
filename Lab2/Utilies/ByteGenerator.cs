using System;
using System.Collections.Generic;
using System.Text;

namespace CustomGenerics.Utilies
{
    class ByteGenerator
    {
        public static byte[] ConvertToBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        public static string ConvertToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
