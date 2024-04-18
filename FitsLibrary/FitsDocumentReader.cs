using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.Deserialization;
using FitsLibrary.Deserialization.Head;
using FitsLibrary.Deserialization.Image;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.Validation;
using FitsLibrary.Validation.Header;

namespace FitsLibrary;

public class FitsDocumentReader : IFitsDocumentReader
{
    private IReadOnlyList<IValidator<Header>> headerValidators;

    internal const int ChunkSize = 2880;

    public FitsDocumentReader()
    {
        UseValidatorsForReading();
    }

    /// <summary>
    /// Used for Unit Testing
    /// </summary>
    /// <param name="headerDeserializer"></param>
    /// <param name="headerValidators"></param>
    /// <param name="contentDeserializer"></param>
    internal FitsDocumentReader(
            List<IValidator<Header>> headerValidators)
    {
        this.headerValidators = headerValidators;
    }

    private void UseValidatorsForReading()
    {
        headerValidators = new List<IValidator<Header>>
        {
            new KeywordsMustBeUniqueValidator(),
            new MandatoryHeaderEntriesValidator(),
        };
    }

    public Task<FitsDocument> ReadAsync(string filePath)
    {
        return ReadAsync(File.OpenRead(filePath));
    }

    private async Task<(bool endOfStreamReached, Header parsedHeader)> ReadHeaderAsync(PipeReader pipeReader)
    {
        (var endOfStreamReachedHeader, var header) = await new HeaderDeserializer()
            .DeserializeAsync(pipeReader)
            .ConfigureAwait(false);

        var validatorTasks = new List<Task<ValidationResult>>();
        foreach (var headerValidator in headerValidators)
        {
            validatorTasks.Add(headerValidator.ValidateAsync(header));
        }

        var validationResults = await Task
            .WhenAll(validatorTasks)
            .ConfigureAwait(continueOnCapturedContext: false);

        foreach (var validationResult in validationResults)
        {
            if (!validationResult.ValidationSuccessful)
            {
                throw new InvalidDataException($"Validation failed for the header of the fits file: {validationResult.ValidationFailureMessage}");
            }
        }

        return (endOfStreamReachedHeader, header);
    }

    public virtual async Task<FitsDocument> ReadAsync(Stream inputStream)
    {
        var pipeReader = PipeReader.Create(
                inputStream,
                new StreamPipeReaderOptions(
                    bufferSize: ChunkSize,
                    minimumReadSize: ChunkSize))!;

        var hdus = new List<HeaderDataUnit>();

        var endOfStreamReached = false;
        var first = true;
        while (!endOfStreamReached)
        {
            var headerResult = await ReadHeaderAsync(pipeReader).ConfigureAwait(false);
            var type = first ? HeaderDataUnitType.PRIMARY : ParseHduType(headerResult.parsedHeader["XTENSION"] as string);
            IHduDeserializer hduDeserializer = type switch
            {
                HeaderDataUnitType.PRIMARY => headerResult.parsedHeader.DataContentType switch
                {
                    DataContentType.BYTE => new PrimaryHduDeserializer<byte>(),
                    DataContentType.SHORT => new PrimaryHduDeserializer<short>(),
                    DataContentType.INTEGER => new PrimaryHduDeserializer<int>(),
                    DataContentType.LONG => new PrimaryHduDeserializer<long>(),
                    DataContentType.FLOAT => new PrimaryHduDeserializer<float>(),
                    DataContentType.DOUBLE => new PrimaryHduDeserializer<double>(),
                },
                HeaderDataUnitType.IMAGE => headerResult.parsedHeader.DataContentType switch
                {
                    DataContentType.BYTE => new ImageHduDeserializer<byte>(),
                    DataContentType.SHORT => new ImageHduDeserializer<short>(),
                    DataContentType.INTEGER => new ImageHduDeserializer<int>(),
                    DataContentType.LONG => new ImageHduDeserializer<long>(),
                    DataContentType.FLOAT => new ImageHduDeserializer<float>(),
                    DataContentType.DOUBLE => new ImageHduDeserializer<double>(),
                },
                _ => throw new NotImplementedException()
            };

            (var endOfStreamReachedHdu, var hdu) = await hduDeserializer
                .DeserializeAsync(pipeReader, headerResult.parsedHeader)
                .ConfigureAwait(false);

            hdus.Add(hdu);
            endOfStreamReached = endOfStreamReachedHdu;
            first = false;
        }
        return new FitsDocument(hdus);
    }

    private static HeaderDataUnitType ParseHduType(string extensionType) => extensionType.Trim() switch
    {
        "IMAGE" => HeaderDataUnitType.IMAGE,
        "TABLE" => HeaderDataUnitType.TABLE,
        _ => throw new InvalidDataException($"Unknown extension type {extensionType} encountered")
    };
}

public class FitsDocumentReader<PrimaryDataType> : FitsDocumentReader where PrimaryDataType : INumber<PrimaryDataType>
{
    public override async Task<FitsDocument> ReadAsync(Stream inputStream)
    {
        var doc = await base.ReadAsync(inputStream).ConfigureAwait(false);
        return new FitsDocument<PrimaryDataType>(doc.HeaderDataUnits);
    }
}
