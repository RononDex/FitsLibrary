using System.IO;
using System.IO.Pipelines;
using System.Text;
using FitsLibrary.Deserialization;

namespace FitsLibrary.Tests
{
    public static class TestUtils
    {
        public static PipeReader ByteArrayToStream(byte[] data)
        {
            return PipeReader.Create(new MemoryStream(data), new StreamPipeReaderOptions(bufferSize: 2880));
        }

        public static PipeReader StringToStream(string data)
        {
            var bytes = Encoding.ASCII.GetBytes(data);
            return ByteArrayToStream(bytes);
        }

        public static byte[] AddHeaderEntry(byte[] data, int startIndex, string key, object? value, string? comment)
        {
            var combinedValue = (value ?? string.Empty).ToString()!.PadRight(70);
            if (key == "CONTINUE")
            {
                combinedValue = $"{key}   {value}";
                if (comment != null)
                {
                    combinedValue = $"{combinedValue} / {comment}";
                }
            }
            else if (comment != null)
            {
                combinedValue = $"{value} / {comment}".PadRight(70);
            }
            if (value is bool valueBool)
            {
                combinedValue = (valueBool ? "T" : "F").PadLeft(HeaderDeserializer.LogicalValuePosition);
                if (comment != null)
                {
                    combinedValue = $"{combinedValue} / {comment}".PadRight(70);
                }
            }

            var headerEntryBytes = Encoding.ASCII.GetBytes($"{combinedValue}");
            if (value != null && key != "CONTINUE")
            {
                headerEntryBytes = Encoding.ASCII.GetBytes($"{key.PadRight(8)}= {combinedValue}");
            }

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
            for (var i = startIndex; (i - startIndex) < content.Length; i++)
            {
                data[i] = content[i - startIndex];
            }

            return data;
        }
    }
}
