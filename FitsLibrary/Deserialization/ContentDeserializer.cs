using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.Objects;
using FitsLibrary.Extensions;

namespace FitsLibrary.Deserialization
{
    public class ContentDeserializer : IContentDeserializer
    {
        private const int ChunkSize = 2880;

        public Task<Content?> DeserializeAsync(PipeReader dataStream, Header header)
        {
            if (header.NumberOfAxisInMainContent == 0)
            {
                return Task.FromResult<Content?>(null);
            }

            var numberOfBytesPerValue = Math.Abs((int)header.DataContentType / 8);
            var numberOfAxis = header.NumberOfAxisInMainContent;
            var axisSizes = Enumerable.Range(1, numberOfAxis)
                .Select(axisIndex => Convert.ToUInt64(header[$"NAXIS{axisIndex}"])).ToArray();
            var axisSizesSpan = new ReadOnlySpan<ulong>(axisSizes);
            var totalNumberOfValues = axisSizes.Aggregate((ulong)1, (x, y) => x * y);
            Memory<DataPoint> dataPointsMemory = new DataPoint[totalNumberOfValues];
            var dataPoints = dataPointsMemory.Span;
            var contentSizeInBytes = numberOfBytesPerValue * Convert.ToInt32(totalNumberOfValues);
            var totalContentSizeInBytes = Math.Ceiling(Convert.ToDouble(contentSizeInBytes) / Convert.ToDouble(ChunkSize)) * ChunkSize;
            var contentDataType = header.DataContentType;
            Span<uint> currentCoordinates = stackalloc uint[numberOfAxis];
            Span<byte> currentValueBuffer = stackalloc byte[numberOfBytesPerValue];

            var bytesRead = 0;
            var currentValueIndex = 0;
            while (bytesRead < contentSizeInBytes)
            {
                var chunk = ReadContentDataStream(dataStream).GetAwaiter().GetResult();
                var blockSize = Math.Min(ChunkSize, contentSizeInBytes - bytesRead);
                bytesRead += ChunkSize;

                for (var i = 0; i < blockSize; i += numberOfBytesPerValue)
                {
                    chunk.Buffer.Slice(i, numberOfBytesPerValue).CopyTo(currentValueBuffer);

                    var value = ParseValue(contentDataType, currentValueBuffer);

                    dataPoints[currentValueIndex++] = new DataPoint(currentCoordinates.ToArray(), value);

                    MoveToNextCoordinate(axisSizesSpan, currentCoordinates);
                }

                dataStream.AdvanceTo(chunk.Buffer.GetPosition(blockSize), chunk.Buffer.End);
            }

            return Task.FromResult((Content?)new Content(dataPointsMemory));
        }

        private static void MoveToNextCoordinate(ReadOnlySpan<ulong> axisSizes, Span<uint> currentCoordinates)
        {
            var maxAxisReached = false;
            for (var axis = 0; axis < axisSizes.Length && !maxAxisReached; axis++)
            {
                if (axisSizes[axis] == currentCoordinates[axis] + 1)
                {
                    maxAxisReached = false;
                    currentCoordinates[axis] = 0;
                }
                else
                {
                    currentCoordinates[axis]++;
                    break;
                }
            }
        }

        private static object ParseValue(DataContentType dataContentType, ReadOnlySpan<byte> currentValueBytes)
        {
            return dataContentType switch
            {
                DataContentType.DOUBLE => BinaryPrimitives.ReadDoubleBigEndian(currentValueBytes),
                DataContentType.FLOAT => BinaryPrimitives.ReadSingleBigEndian(currentValueBytes),
                DataContentType.BYTE => currentValueBytes[0],
                DataContentType.SHORT => BinaryPrimitives.ReadInt16BigEndian(currentValueBytes),
                DataContentType.INTEGER => BinaryPrimitives.ReadInt32BigEndian(currentValueBytes),
                DataContentType.LONG => BinaryPrimitives.ReadInt64BigEndian(currentValueBytes) as object,
                _ => throw new InvalidDataException("Invalid data type"),
            };
        }

        private static async Task<ReadResult> ReadContentDataStream(PipeReader dataStream)
        {
            return await dataStream.ReadAsync().ConfigureAwait(false);
        }
    }
}
