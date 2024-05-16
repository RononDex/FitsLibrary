using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.Serialization;
using FitsLibrary.Serialization.Header;
using FitsLibrary.Serialization.Image;
using FitsLibrary.Validation;
using FitsLibrary.Validation.Header;

namespace FitsLibrary;

public class FitsDocumentWriter : IFitsDocumentWriter
{
    internal const int ChunkSize = 2880;

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

        var headerSerializer = new HeaderSerializer();

        foreach (var validator in ValidationLists.FitsDocumentValidators)
        {
            var result = await validator.ValidateAsync(document);
            if (!result.ValidationSuccessful)
            {
                throw new InvalidDataException($"Tried to write invalid FitsDocument: {result.ValidationFailureMessage}");
            }
        }

        foreach (var hdu in document.HeaderDataUnits)
        {
            var type = hdu.Type;
            IHduSerializer? hduSerializer = type switch
            {
                HeaderDataUnitType.IMAGE => hdu.Header.DataContentType switch
                {
                    DataContentType.BYTE => new ImageHduSerializer<byte>(headerSerializer, new HeaderValidator(ValidationLists.ImageExtensionHeaderValidators)),
                    DataContentType.FLOAT => new ImageHduSerializer<float>(headerSerializer, new HeaderValidator(ValidationLists.ImageExtensionHeaderValidators)),
                    DataContentType.INT16 => new ImageHduSerializer<short>(headerSerializer, new HeaderValidator(ValidationLists.ImageExtensionHeaderValidators)),
                    DataContentType.INT32 => new ImageHduSerializer<int>(headerSerializer, new HeaderValidator(ValidationLists.ImageExtensionHeaderValidators)),
                    DataContentType.INT64 => new ImageHduSerializer<long>(headerSerializer, new HeaderValidator(ValidationLists.ImageExtensionHeaderValidators)),
                    DataContentType.DOUBLE => new ImageHduSerializer<double>(headerSerializer, new HeaderValidator(ValidationLists.ImageExtensionHeaderValidators)),
                    _ => throw new NotImplementedException(),
                },
                HeaderDataUnitType.PRIMARY => hdu.Header.DataContentType switch
                {
                    DataContentType.BYTE => new ImageHduSerializer<byte>(headerSerializer, new HeaderValidator(ValidationLists.PrimaryBlockHeaderValidators)),
                    DataContentType.FLOAT => new ImageHduSerializer<float>(headerSerializer, new HeaderValidator(ValidationLists.PrimaryBlockHeaderValidators)),
                    DataContentType.INT16 => new ImageHduSerializer<short>(headerSerializer, new HeaderValidator(ValidationLists.PrimaryBlockHeaderValidators)),
                    DataContentType.INT32 => new ImageHduSerializer<int>(headerSerializer, new HeaderValidator(ValidationLists.PrimaryBlockHeaderValidators)),
                    DataContentType.INT64 => new ImageHduSerializer<long>(headerSerializer, new HeaderValidator(ValidationLists.PrimaryBlockHeaderValidators)),
                    DataContentType.DOUBLE => new ImageHduSerializer<double>(headerSerializer, new HeaderValidator(ValidationLists.PrimaryBlockHeaderValidators)),
                    _ => throw new NotImplementedException(),
                },
                _ => null,
            };

            if (hduSerializer == null)
            {
                Console.WriteLine($"Unknown hdu type {type} found, stopping writing of file");
                break;
            }

            await hduSerializer.SerializeAsync(pipeWriter, hdu);
        }

        pipeWriter.Complete();
    }
}
