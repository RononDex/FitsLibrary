
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.Serialization;
using FitsLibrary.Validation;

namespace FitsLibrary
{

    public class FitsDocumentWriter<T> : IFitsDocumentWriter<T> where T : INumber<T>
    {
        private IReadOnlyList<IValidator<Header>> headerValidators;
        private IHeaderSerializer headerSerializer;

        internal const int ChunkSize = 2880;

        public FitsDocumentWriter()
        {
            UseValidatorsForWriting();
            UseSerializersForWriting();
        }

        private void UseSerializersForWriting()
        {
            headerSerializer = new HeaderSerializer();
        }
        private void UseValidatorsForWriting() => throw new NotImplementedException();

        public async Task WriteAsync(FitsDocument<T> document, string filePath)
        {
            var fileStream = File.OpenWrite(filePath);
        }
        public async Task WriteAsync(FitsDocument<T> document, Stream writeToStream)
        {
            var pipeWriter = PipeWriter.Create(
                    writeToStream,
                    new StreamPipeWriterOptions(minimumBufferSize: ChunkSize));

            await headerSerializer.SerializeAsync(document.Header, pipeWriter);
        }
    }
}
