using System;
using System.IO;
using System.Text;
using FitsLibrary.Deserialization;

namespace FitsLibrary.Tests
{
    public static class TestUtils
    {
        public static Stream ByteArrayToStream(byte[] data)
        {
            return new MemoryStream(data);
        }

        public static Stream StringToStream(string data)
        {
            var bytes = Encoding.ASCII.GetBytes(data);
            return ByteArrayToStream(bytes);
        }

        public static byte[] AddHeaderEntry(byte[] data, int startIndex, string key, object value, string comment)
        {
            var combinedValue = value.ToString().PadRight(70);
            if (key == "CONTINUE")
            {
                combinedValue = $"{key} {value}";
                if (comment != null)
                {
                    combinedValue = $"{combinedValue} / {comment}";
                }
            }
            else if (comment != null)
            {
                combinedValue = $"{value} / {comment}".PadRight(70);
            }
            if (value is bool)
            {
                combinedValue = (((bool)value) ? "T" : "F").PadLeft(HeaderDeserializer.LogicalValuePosition);
            }

            var headerEntryBytes = Encoding.ASCII.GetBytes($"{key.PadRight(8)}= {combinedValue}");

            return AddContentToArray(
                data,
                startIndex,
                headerEntryBytes);
        }

        public static byte[] AddHeaderEntry(byte[] data, int startIndex, string key)
        {
            return AddContentToArray(
                data,
                startIndex,
                Encoding.ASCII.GetBytes(key.PadRight(80)));
        }

        public static byte[] AddContentToArray(byte[] data, int startIndex, byte[] content)
        {
            for (var i = startIndex; (i-startIndex)<content.Length; i++)
            {
                data[i] = content[i-startIndex];
            }

            return data;
        }
    }
}
