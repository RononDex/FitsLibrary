
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.Extensions;

namespace FitsLibrary.Serialization
{
    public class HeaderSerializer : IHeaderSerializer
    {
        public const int HeaderEntryChunkSize = 80;
        public const int HeaderBlockSize = 2880;
        public const int LogicalValuePosition = 20;
        public const char ContinuedStringMarker = '&';
        private const string ContinueKeyWord = "CONTINUE";

        public Task SerializeAsync(Header header, PipeWriter writer)
        {
            var headerBlockBuilder = new StringBuilder();
            var headerEntryBuilder = new StringBuilder();
            foreach (var headerEntry in header.Entries)
            {
                _ = headerEntryBuilder.Clear();
                _ = headerEntryBuilder.Append(headerEntry.Key.PadRight(8));

                if (headerEntry.Value != null)
                {
                    _ = headerEntryBuilder.Append("= ");
                    var serializedValue = GetSerializedValue(headerEntry.Value);

                    // Handle strings and long strings
                    if (headerEntry.Value is string)
                    {
                        var subStrings = serializedValue.SplitInParts(68);
                    }
                    else
                    {
                        _ = headerEntryBuilder.Append(serializedValue);
                    }

                    if (!string.IsNullOrEmpty(headerEntry.Comment))
                    {
                        _ = headerEntryBuilder.AppendFormat(" / {0}", headerEntry.Comment);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(headerEntry.Comment))
                    {
                        _ = headerEntryBuilder.Append(headerEntry.Comment);
                    }
                }

                _ = headerBlockBuilder.Append(headerEntryBuilder.ToString().PadRight(HeaderEntryChunkSize));
            }


            _ = headerBlockBuilder.Append("END".PadRight(HeaderEntryChunkSize));
            var numberOfHeaderBlocks = headerBlockBuilder.Length / HeaderBlockSize;
            if (headerBlockBuilder.Length % HeaderBlockSize != 0)
            {
                numberOfHeaderBlocks += 1;
            }
            var length = numberOfHeaderBlocks * HeaderBlockSize;
            var writerMemory = writer.GetMemory(sizeHint: length);
            var bytes = Encoding.ASCII.GetBytes(headerBlockBuilder.ToString().PadRight(length), writerMemory.Span);
            writer.Advance(bytes);
            return writer.FlushAsync().AsTask();
        }

        private static string GetSerializedValue(object value)
        {
            return value switch
            {
                bool bValue => bValue ? "T" : "F",
                string sValue => sValue.Replace("'", "''"),
                _ => value.ToString()
            };
        }
    }
}
