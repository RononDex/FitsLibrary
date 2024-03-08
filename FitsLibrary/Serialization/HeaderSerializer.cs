
using System.IO.Pipelines;
using System.Text;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Serialization
{
    public class HeaderSerializer : IHeaderSerializer
    {
        public const int HeaderEntryChunkSize = 80;
        public const int HeaderBlockSize = 2880;
        public const int LogicalValuePosition = 20;
        public const char ContinuedStringMarker = '&';
        private const string ContinueKeyWord = "CONTINUE";

        public void SerializeAsync(Header header, PipeWriter writer)
        {
            var stringBuilder = new StringBuilder();
            foreach (var headerEntry in header.Entries)
            {
                stringBuilder.Clear();
                stringBuilder.Append(headerEntry.Key.PadRight(8));

                if (headerEntry.Value != null)
                {
                    stringBuilder.Append("= ");
                    stringBuilder.Append(headerEntry.Value);
                }
                else
                {
                    stringBuilder.Append("  ");
                }
            }
        }
    }
}
