using System;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public class ExtensionDeserializer : IExtensionDeserializer
    {
        private readonly IHeaderDeserializer headerDeserializer;

        public ExtensionDeserializer(IHeaderDeserializer headerDeserializer)
        {
            this.headerDeserializer = headerDeserializer;
        }

        public async Task<(bool, Extension)> DeserializeAsync(PipeReader dataStream)
        {
            // TODO: Validate parsedHeader for needed fields etc
            var parsedExtensionHeaderResult = await headerDeserializer.DeserializeAsync(dataStream);
            (bool endOfStreamReached, Memory<object>? contentData)? parsedExtensionContentResult = null;

            if (!parsedExtensionHeaderResult.endOfStreamReached)
            {
                parsedExtensionContentResult = await contentDeserializer.DeserializeAsync(dataStream, parsedExtensionHeaderResult.parsedHeader);
            }

            return (parsedExtensionHeaderResult.endOfStreamReached
                    || (parsedExtensionContentResult?.endOfStreamReached == true),
                    new Extension(parsedExtensionHeaderResult.parsedHeader, parsedExtensionContentResult?.contentData));
        }
    }
}
