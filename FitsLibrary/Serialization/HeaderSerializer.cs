
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
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

        public Task SerializeAsync(Header header, PipeWriter writer)
        {
            var headerBlockBuilder = new StringBuilder();
            var headerEntryBuilder = new StringBuilder();
            foreach (var headerEntry in header.Entries)
            {
                headerEntryBuilder.Clear();
                headerEntryBuilder.Append(headerEntry.Key.PadRight(8));

                if (headerEntry.Value != null)
                {
                    headerEntryBuilder.Append("= ");
                    headerEntryBuilder.Append(headerEntry.Value);
                }
                else
                {
                    headerEntryBuilder.Append("  ");
                }

                headerBlockBuilder.Append(headerEntryBuilder.ToString().PadRight(HeaderEntryChunkSize));
            }


            headerBlockBuilder.Append("END".PadRight(HeaderEntryChunkSize));
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
    }
}
