using System.IO;
using System.Threading.Tasks;

namespace FitsLibrary
{
    public class FitsDocumentReader : IFitsDocumentReader
    {
        public async Task<FitsDocument> ReadAsync(Stream inputStream)
        {
            var header = await new Deserialization.HeaderDeserializer()
                .DeserializeAsync(inputStream)
                .ConfigureAwait(false);

            return new FitsDocument(
                header: header);
        }

        public Task<FitsDocument> ReadAsync(string filePath)
        {
            return ReadAsync(File.OpenRead(filePath));
        }
    }
}
