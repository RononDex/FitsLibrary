using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.Extensions;

namespace FitsLibrary.Deserialization
{
    public abstract class ContentDeserializer<TData> : IContentDeserializer<TData> where TData : INumber<TData>
    {
        private const int ChunkSize = 2880;

        public Task<(bool endOfStreamReached, Memory<TData>? contentData)> DeserializeAsync(PipeReader dataStream, Header header)
        {
            var numberOfBytesPerValue = Math.Abs((int)header.DataContentType / 8);
            var numberOfAxis = header.NumberOfAxisInMainContent;
            var axisSizes = Enumerable.Range(1, numberOfAxis)
                .Select(axisIndex => Convert.ToUInt64(header[$"NAXIS{axisIndex}"])).ToArray();
            var axisSizesSpan = new ReadOnlySpan<ulong>(axisSizes);
            var totalNumberOfValues = axisSizes.Aggregate((ulong)1, (x, y) => x * y);
            Memory<TData> dataPointsMemory = new TData[totalNumberOfValues];
            var dataPoints = dataPointsMemory.Span;
            var contentSizeInBytes = numberOfBytesPerValue * Convert.ToInt32(totalNumberOfValues);
            var totalContentSizeInBytes = Math.Ceiling(Convert.ToDouble(contentSizeInBytes) / Convert.ToDouble(ChunkSize)) * ChunkSize;
            Span<byte> currentValueBuffer = stackalloc byte[numberOfBytesPerValue];
            var endOfStreamReached = false;

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

                    dataPoints[currentValueIndex++] = ParseValue(currentValueBuffer);
                }

                dataStream.AdvanceTo(chunk.Buffer.GetPosition(blockSize), chunk.Buffer.End);
            }

            return Task.FromResult<(bool, Memory<TData>?)>((endOfStreamReached, dataPointsMemory));
        }

        protected abstract TData ParseValue(ReadOnlySpan<byte> currentValueBytes);

        private static async Task<ReadResult> ReadContentDataStream(PipeReader dataStream)
        {
            return await dataStream.ReadAsync().ConfigureAwait(false);
        }

        public static IContentDeserializer<TData> GetInstance(DataContentType dataContentType)
        {
            switch (dataContentType)
            {
                case DataContentType.DOUBLE:
                    return (IContentDeserializer<TData>)new ContentDeserializerDouble();
                case DataContentType.FLOAT:
                    return (IContentDeserializer<TData>)new ContentDeserializerFloat();
                case DataContentType.BYTE:
                    return (IContentDeserializer<TData>)new ContentDeserializerByte();
                case DataContentType.SHORT:
                    return (IContentDeserializer<TData>)new ContentDeserializerInt16();
                case DataContentType.INTEGER:
                    return (IContentDeserializer<TData>)new ContentDeserializerInt32();
                case DataContentType.LONG:
                    return (IContentDeserializer<TData>)new ContentDeserializerInt64();
                default:
                    throw new ArgumentException($"Invalid dataContentType {dataContentType}");

            }
        }
    }
}
