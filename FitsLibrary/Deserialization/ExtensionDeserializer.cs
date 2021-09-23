using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public class ExtensionDeserializer : IExtensionDeserializer
    {
        private readonly IContentDeserializer contentDeserializer;
        private readonly IHeaderDeserializer headerDeserializer;

        public ExtensionDeserializer(
            IHeaderDeserializer headerDeserializer,
            IContentDeserializer contentDeserializer)
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
