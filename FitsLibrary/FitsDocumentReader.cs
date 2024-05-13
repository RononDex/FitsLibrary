using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipelines;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.Deserialization;
using FitsLibrary.Deserialization.Head;
using FitsLibrary.Deserialization.Image;
using FitsLibrary.Deserialization.Primary;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.Validation.Header;

namespace FitsLibrary;

public class FitsDocumentReader : IFitsDocumentReader
{
    internal const int ChunkSize = 2880;

    public Task<FitsDocument> ReadAsync(string filePath)
    {
        return ReadAsync(File.OpenRead(filePath));
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
            (var endOfStreamReachedHeader, var header) = await new HeaderDeserializer()
                .DeserializeAsync(pipeReader)
                .ConfigureAwait(false);

            if (endOfStreamReachedHeader && header == null)
                break;

            var type = first ? HeaderDataUnitType.PRIMARY : ParseHduType(header["XTENSION"] as string);
            IHduDeserializer? hduDeserializer = type switch
            {
                HeaderDataUnitType.PRIMARY => ExtractDataContentType(header) switch
                {
                    DataContentType.BYTE => new PrimaryHduDeserializer<byte>(ValidationLists.PrimaryBlockHeaderValidators),
                    DataContentType.INT16 => new PrimaryHduDeserializer<short>(ValidationLists.PrimaryBlockHeaderValidators),
                    DataContentType.INT32 => new PrimaryHduDeserializer<int>(ValidationLists.PrimaryBlockHeaderValidators),
                    DataContentType.INT64 => new PrimaryHduDeserializer<long>(ValidationLists.PrimaryBlockHeaderValidators),
                    DataContentType.FLOAT => new PrimaryHduDeserializer<float>(ValidationLists.PrimaryBlockHeaderValidators),
                    DataContentType.DOUBLE => new PrimaryHduDeserializer<double>(ValidationLists.PrimaryBlockHeaderValidators),
                    _ => throw new NotImplementedException(),
                },
                HeaderDataUnitType.IMAGE => ExtractDataContentType(header) switch
                {
                    DataContentType.BYTE => new ImageHduDeserializer<byte>(ValidationLists.ImageExtensionHeaderValidators),
                    DataContentType.INT16 => new ImageHduDeserializer<short>(ValidationLists.ImageExtensionHeaderValidators),
                    DataContentType.INT32 => new ImageHduDeserializer<int>(ValidationLists.ImageExtensionHeaderValidators),
                    DataContentType.INT64 => new ImageHduDeserializer<long>(ValidationLists.ImageExtensionHeaderValidators),
                    DataContentType.FLOAT => new ImageHduDeserializer<float>(ValidationLists.ImageExtensionHeaderValidators),
                    DataContentType.DOUBLE => new ImageHduDeserializer<double>(ValidationLists.ImageExtensionHeaderValidators),
                    _ => throw new NotImplementedException(),
                },
                _ => null,
            };

            if (hduDeserializer == null)
            {
                Console.WriteLine($"Unknown xtension type {header["XTENSION"]} found, stopping parsing of file");
                break;
            }
            (var endOfStreamReachedHdu, var hdu) = await hduDeserializer
                .DeserializeAsync(pipeReader, header)
                .ConfigureAwait(false);

            hdus.Add(hdu);
            endOfStreamReached = endOfStreamReachedHdu;
            first = false;
        }
        pipeReader.Complete();
        return new FitsDocument(hdus);
    }

    private static HeaderDataUnitType ParseHduType(string extensionType) => extensionType.Trim() switch
    {
        "IMAGE" => HeaderDataUnitType.IMAGE,
        "TABLE" => HeaderDataUnitType.TABLE,
        _ => throw new InvalidDataException($"Unknown extension type {extensionType} encountered")
    };

    private static DataContentType ExtractDataContentType(Header header) => (DataContentType)Convert.ToInt32(header["BITPIX"]!, CultureInfo.InvariantCulture);
}

public class FitsDocumentReader<PrimaryDataType> : FitsDocumentReader where PrimaryDataType : INumber<PrimaryDataType>
{
    public new async Task<FitsDocument<PrimaryDataType>> ReadAsync(Stream inputStream)
    {
        var doc = await base.ReadAsync(inputStream).ConfigureAwait(false);
        return new FitsDocument<PrimaryDataType>(doc.HeaderDataUnits);
    }

    public new Task<FitsDocument<PrimaryDataType>> ReadAsync(string filePath)
    {
        return ReadAsync(File.OpenRead(filePath));
    }
}
