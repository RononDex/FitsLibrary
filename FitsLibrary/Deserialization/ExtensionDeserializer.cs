using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public class ExtensionDeserializer : IExtensionDeserializer
    {
        private readonly ContentDeserializer contentDeserializer;
        private readonly HeaderDeserializer headerDeserializer;

        public ExtensionDeserializer(
            HeaderDeserializer headerDeserializer,
            ContentDeserializer contentDeserializer)
        {
            this.headerDeserializer = headerDeserializer;
            this.contentDeserializer = contentDeserializer;
        }

        public async Task<Extension> DeserializeAsync(PipeReader dataStream)
        {
            // TODO: Validate parsedHeader for needed fields etc
            var parsedExtensionHeader = await headerDeserializer.DeserializeAsync(dataStream);
            var parsedExtensionContent = await contentDeserializer.DeserializeAsync(dataStream, parsedExtensionHeader);

            return new Extension(parsedExtensionHeader, parsedExtensionContent);
        }
    }
}
