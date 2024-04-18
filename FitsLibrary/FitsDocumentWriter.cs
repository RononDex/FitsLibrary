
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.Serialization;
using FitsLibrary.Validation;

namespace FitsLibrary;

public class FitsDocumentWriter : IFitsDocumentWriter
{
    private readonly IReadOnlyList<IValidator<Header>> headerValidators;
    private IHeaderSerializer headerSerializer;

    internal const int ChunkSize = 2880;

    public FitsDocumentWriter()
    {
        UseValidatorsForWriting();
        UseSerializersForWriting();
    }

    private void UseSerializersForWriting()
    {
        this.headerSerializer = new HeaderSerializer();
    }
    private void UseValidatorsForWriting() => throw new NotImplementedException();

    public Task WriteAsync(FitsDocument document, string filePath)
    {
        var fileStream = File.OpenWrite(filePath);
        return WriteAsync(document, fileStream);
    }
    public async Task WriteAsync(FitsDocument document, Stream writeToStream)
    {
        var pipeWriter = PipeWriter.Create(
                writeToStream,
                new StreamPipeWriterOptions(minimumBufferSize: ChunkSize));

        foreach (var hdu in document.HeaderDataUnits)
        {
            await headerSerializer.SerializeAsync(hdu.Header, pipeWriter);
        }
    }
}
