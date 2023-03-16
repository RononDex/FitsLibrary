using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.Deserialization.ContentValueDeserializer;
using FitsLibrary.DocumentParts;
using FitsLibrary.Extensions;

namespace FitsLibrary.Deserialization
{
    public class ContentDeserializer<T> : IContentDeserializer<T> where T : INumber<T>
    {
        private const int ChunkSize = 2880;

        public Task<(bool endOfStreamReached, Memory<T>? contentData)> DeserializeAsync(PipeReader dataStream, Header header)
        {
            if (header.NumberOfAxisInMainContent < 1)
            {
                // Return endOfStreamReached false, since this method is only called if endOfStreamReached was false
                // before calling this method, so since we did not read anything, it should still be false
                return Task.FromResult<(bool, Memory<T>?)>((false, null));
            }


            var numberOfBytesPerValue = Math.Abs((int)header.DataContentType / 8);
            var numberOfAxis = header.NumberOfAxisInMainContent;
            var axisSizes = Enumerable.Range(1, numberOfAxis)
                .Select(axisIndex => Convert.ToUInt64(header[$"NAXIS{axisIndex}"])).ToArray();
            var axisSizesSpan = new ReadOnlySpan<ulong>(axisSizes);
            var totalNumberOfValues = axisSizes.Aggregate((ulong)1, (x, y) => x * y);
            Memory<T> dataPointsMemory = new T[totalNumberOfValues];
            var dataPoints = dataPointsMemory.Span;
            var contentSizeInBytes = numberOfBytesPerValue * Convert.ToInt32(totalNumberOfValues);
            var totalContentSizeInBytes = Math.Ceiling(Convert.ToDouble(contentSizeInBytes) / Convert.ToDouble(ChunkSize)) * ChunkSize;
            Span<byte> currentValueBuffer = stackalloc byte[numberOfBytesPerValue];
            var endOfStreamReached = false;
            var valueParser = GetContentValueParser<T>(header.DataContentType);

            var bytesRead = 0;
            var currentValueIndex = 0;
            while (bytesRead < contentSizeInBytes)
            {
                var chunk = ReadContentDataStream(dataStream).GetAwaiter().GetResult();
                endOfStreamReached = chunk.IsCompleted;
                var blockSize = Math.Min(ChunkSize, contentSizeInBytes - bytesRead);
                bytesRead += blockSize;

                for (var i = 0; i < blockSize; i += numberOfBytesPerValue)
                {
                    chunk.Buffer.Slice(i, numberOfBytesPerValue).CopyTo(currentValueBuffer);

                    dataPoints[currentValueIndex++] = valueParser.ParseValue(currentValueBuffer);
                }

                dataStream.AdvanceTo(chunk.Buffer.GetPosition(blockSize), chunk.Buffer.End);
            }

            return Task.FromResult<(bool, Memory<T>?)>((endOfStreamReached, dataPointsMemory));
        }

        private static async Task<ReadResult> ReadContentDataStream(PipeReader dataStream)
        {
            return await dataStream.ReadAsync().ConfigureAwait(false);
        }

        private static IContentValueDeserializer<TData> GetContentValueParser<TData>(DataContentType dataContentType) where TData : INumber<TData>
        {
            try
            {
                switch (dataContentType)
                {
                    case DataContentType.DOUBLE:
                        return (IContentValueDeserializer<TData>)new ContentValueDeserializerDouble();
                    case DataContentType.FLOAT:
                        return (IContentValueDeserializer<TData>)new ContentValueDeserializerFloat();
                    case DataContentType.BYTE:
                        return (IContentValueDeserializer<TData>)new ContentValueDeserializerByte();
                    case DataContentType.INTEGER:
                        return (IContentValueDeserializer<TData>)new ContentValueDeserializerInt32();
                    case DataContentType.LONG:
                        return (IContentValueDeserializer<TData>)new ContentValueDeserializerInt64();
                    case DataContentType.SHORT:
                        return (IContentValueDeserializer<TData>)new ContentValueDeserializerInt16();
                    default:
                        throw new ArgumentException("Tried to deserialize unimplemented datatype");
                }
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException($"Tried to parse a fits document of type {dataContentType} as {typeof(TData).Name}");
            }
        }
    }
}
