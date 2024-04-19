using System;
using System.Buffers;
using System.Globalization;
using System.IO.Pipelines;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.Deserialization.Image.ImageContentValueDeserializer;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.Extensions;

namespace FitsLibrary.Deserialization.Image;

internal class ImageContentDeserializer<T> : IContentDeserializer where T : INumber<T>
{
    private const int ChunkSize = 2880;

    public Task<(bool endOfStreamReached, ImageDataContent<T> data)> DeserializeAsync(PipeReader dataStream, Header header)
    {
        var headerBoxed = new ImageHeader(header.Entries);
        if (headerBoxed.NumberOfAxisInMainContent < 1)
        {
            // Return endOfStreamReached false, since this method is only called if endOfStreamReached was false
            // before calling this method, so since we did not read anything, it should still be false
            return Task.FromResult((endOfStreamReached: false, data: new ImageDataContent<T>(new int[1] { 0 }, Memory<T>.Empty)));
        }

        var numberOfBytesPerValue = Math.Abs((int)headerBoxed.DataContentType / 8);
        var numberOfAxis = headerBoxed.NumberOfAxisInMainContent;
        var axisSizes = Enumerable.Range(1, numberOfAxis)
            .Select(axisIndex => Convert.ToInt32(header[$"NAXIS{axisIndex}"], CultureInfo.InvariantCulture)).ToArray();
        var axisSizesSpan = new ReadOnlySpan<int>(axisSizes);
        var totalNumberOfValues = axisSizes.Aggregate((ulong)1, (x, y) => x * (ulong)y);
        Memory<T> dataPointsMemory = new T[totalNumberOfValues];
        var dataPoints = dataPointsMemory.Span;
        var contentSizeInBytes = numberOfBytesPerValue * Convert.ToInt32(totalNumberOfValues);
        var totalContentSizeInBytes = Math.Ceiling(Convert.ToDouble(contentSizeInBytes) / Convert.ToDouble(ChunkSize)) * ChunkSize;
        Span<byte> currentValueBuffer = stackalloc byte[numberOfBytesPerValue];
        var endOfStreamReached = false;
        var valueParser = GetContentValueParser<T>(headerBoxed.DataContentType);

        var bytesRead = 0;
        var currentValueIndex = 0;
        while (bytesRead <= contentSizeInBytes)
        {
            var chunk = ReadContentDataStream(dataStream).GetAwaiter().GetResult();
            endOfStreamReached = chunk.IsCompleted;
            var blockSize = Math.Min(ChunkSize, contentSizeInBytes - bytesRead);
            if (chunk.Buffer.Length > blockSize)
            {
                // If we dont read the whole buffer, we still have not reached
                // the end of the stream
                endOfStreamReached = false;
            }
            if (blockSize == 0)
            {
                break;
            }

            bytesRead += blockSize;

            for (var i = 0; i < blockSize; i += numberOfBytesPerValue)
            {
                chunk.Buffer.Slice(i, numberOfBytesPerValue).CopyTo(currentValueBuffer);

                dataPoints[currentValueIndex++] = valueParser.ParseValue(currentValueBuffer);
            }

            dataStream.AdvanceTo(chunk.Buffer.GetPosition(blockSize), chunk.Buffer.End);
        }

        return Task.FromResult((endOfStreamReached, data: new ImageDataContent<T>(axisSizes, dataPointsMemory)));
    }

    private static IImageContentValueDeserializer<TData> GetContentValueParser<TData>(DataContentType dataContentType) where TData : INumber<TData>
    {
        try
        {
            return dataContentType switch
            {
                DataContentType.DOUBLE => (IImageContentValueDeserializer<TData>)new ImageContentValueDeserializerDouble(),
                DataContentType.FLOAT => (IImageContentValueDeserializer<TData>)new ImageContentValueDeserializerFloat(),
                DataContentType.BYTE => (IImageContentValueDeserializer<TData>)new ImageContentValueDeserializerByte(),
                DataContentType.INTEGER => (IImageContentValueDeserializer<TData>)new ImageContentValueDeserializerInt32(),
                DataContentType.LONG => (IImageContentValueDeserializer<TData>)new ImageContentValueDeserializerInt64(),
                DataContentType.SHORT => (IImageContentValueDeserializer<TData>)new ImageContentValueDeserializerInt16(),
                _ => throw new ArgumentException("Tried to deserialize unimplemented datatype"),
            };
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException($"Tried to parse a fits document of type {dataContentType} as {typeof(TData).Name}");
        }
    }

    private static async Task<ReadResult> ReadContentDataStream(PipeReader dataStream)
    {
        return await dataStream.ReadAsync().ConfigureAwait(false);
    }

    async Task<(bool endOfStreamReached, DataContent data)> IContentDeserializer.DeserializeAsync(PipeReader dataStream, Header header)
    {
        var (endOfStreamReached, data) = await DeserializeAsync(dataStream, header);
        return (endOfStreamReached, data);
    }
}
