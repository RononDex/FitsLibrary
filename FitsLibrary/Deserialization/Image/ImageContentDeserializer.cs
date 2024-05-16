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
        if (header.NumberOfAxisInMainContent < 1)
        {
            // Return endOfStreamReached false, since this method is only called if endOfStreamReached was false
            // before calling this method, so since we did not read anything, it should still be false
            return Task.FromResult((endOfStreamReached: false, data: new ImageDataContent<T>(new int[] { 0 }, Memory<T>.Empty)));
        }

        var numberOfBytesPerValue = Math.Abs((int)header.DataContentType / 8);
        var numberOfAxis = header.NumberOfAxisInMainContent;
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
        var valueParser = GetContentValueParser<T>(header.DataContentType);
        ReadOnlySpan<byte> readerSpan = currentValueBuffer;

        var bytesRead = 0;
        var currentValueIndex = 0;

        // Read one block further to check if its the end of the file (but don't advance the readers position)
        while (bytesRead <= totalContentSizeInBytes)
        {
            var chunk = ReadContentDataStream(dataStream).GetAwaiter().GetResult();
            endOfStreamReached = chunk.IsCompleted;
            var blockSize = Math.Min(ChunkSize, contentSizeInBytes - bytesRead);
            if (chunk.Buffer.Length > ChunkSize)
            {
                // If we dont read the whole buffer, we still have not reached
                // the end of the stream
                endOfStreamReached = false;
            }
            if (blockSize <= 0)
            {
                break;
            }

            bytesRead += ChunkSize;

            for (var i = 0; i < blockSize; i += numberOfBytesPerValue)
            {
                // If the first span already has the value we are looking for
                // we do not need to copy its value, saving a lot of performance
                if (chunk.Buffer.FirstSpan.Length >= i + numberOfBytesPerValue)
                {
                    readerSpan = chunk.Buffer.FirstSpan.Slice(i, numberOfBytesPerValue);
                }
                else
                {
                    chunk.Buffer.Slice(i, numberOfBytesPerValue).CopyTo(currentValueBuffer);
                    readerSpan = currentValueBuffer;
                }

                dataPoints[currentValueIndex++] = valueParser.ParseValue(readerSpan);
            }

            dataStream.AdvanceTo(chunk.Buffer.GetPosition(ChunkSize), chunk.Buffer.End);
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
                DataContentType.INT32 => (IImageContentValueDeserializer<TData>)new ImageContentValueDeserializerInt32(),
                DataContentType.INT64 => (IImageContentValueDeserializer<TData>)new ImageContentValueDeserializerInt64(),
                DataContentType.INT16 => (IImageContentValueDeserializer<TData>)new ImageContentValueDeserializerInt16(),
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
