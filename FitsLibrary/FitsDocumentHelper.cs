using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.Deserialization;
using FitsLibrary.DocumentParts;

namespace FitsLibrary
{
    public static class FitsDocumentHelper
    {
        public static async Task<Header> ReadHeaderAsync(string filePath)
        {
            return await ReadHeaderAsync(File.OpenRead(filePath));
        }

        public static async Task<Header> ReadHeaderAsync(Stream inputStream)
        {
            var pipeReader = PipeReader.Create(
                    inputStream,
                    new StreamPipeReaderOptions(
                        bufferSize: FitsDocumentReader<int>.ChunkSize,
                        minimumReadSize: FitsDocumentReader<int>.ChunkSize))!;

            var headerDeserializer = new HeaderDeserializer();

            var headerResult = await headerDeserializer
                .DeserializeAsync(pipeReader)
                .ConfigureAwait(false);

            return headerResult.parsedHeader;
        }

        public static async Task<DataContentType> GetDocumentContentType(string filePath)
        {
            return await GetDocumentContentType(File.OpenRead(filePath));
        }

        public static async Task<DataContentType> GetDocumentContentType(Stream inputStream)
        {
            var header = await ReadHeaderAsync(inputStream);
            return header.DataContentType;
        }
    }
}
