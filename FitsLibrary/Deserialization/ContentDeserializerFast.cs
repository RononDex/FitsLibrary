using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.Objects;
using FitsLibrary.Extensions;

namespace FitsLibrary.Deserialization
{
    public class ContentDeserializerFast : IContentDeserializer
    {

        private const int ChunkSize = 2880;

        public Task<Content?> DeserializeAsync(Stream dataStream, Header header)
        {
            if (header.NumberOfAxisInMainContent == 0)
            {
                return null;
            }

            var dataPoints = new List<DataPoint>();
            var numberOfBytesPerValue = Math.Abs((int)header.DataContentType / 8);
            var numberOfAxis = header.NumberOfAxisInMainContent;
            var axisSizes = Enumerable.Range(1, numberOfAxis)
                .Select(axisIndex => Convert.ToUInt64(header[$"NAXIS{axisIndex}"])).ToArray();
            var axisSizesSpan = new ReadOnlySpan<ulong>(axisSizes);
            var totalNumberOfValues = axisSizes.Aggregate((ulong)1, (x, y) => x * y);
            var contentSizeInBytes = numberOfBytesPerValue * Convert.ToInt32(totalNumberOfValues);
            var totalContentSizeInBytes = Convert.ToUInt64(Math.Ceiling(Convert.ToDouble(contentSizeInBytes) / Convert.ToDouble(ChunkSize)) * ChunkSize);
            var contentDataType = header.DataContentType;
            var currentCoordinates = new ulong[numberOfAxis];
            var currentCoordinatesSpan = new Span<ulong>(currentCoordinates);
            ReadOnlySpan<byte> currentValueBytes;

            var contentData = ReadContentDataStream(dataStream, totalContentSizeInBytes);

            for (int i = 0; i < contentSizeInBytes; i += numberOfBytesPerValue)
            {
                currentValueBytes = contentData.Slice(i, numberOfBytesPerValue);

                var value = ParseValue(contentDataType, currentValueBytes);

                dataPoints.Add(new DataPoint(currentCoordinates, value));

                MoveToNextCoordinate(axisSizesSpan, currentCoordinatesSpan);
            }

            return Task.FromResult((Content?)new Content(dataPoints));
        }

        private static void MoveToNextCoordinate(ReadOnlySpan<ulong> axisSizes, Span<ulong> currentCoordinates)
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
                DataContentType.LONG => BinaryPrimitives.ReadInt64BigEndian(currentValueBytes),
                _ => throw new InvalidDataException("Invalid data type"),
            };
        }

        private static ReadOnlySpan<byte> ReadContentDataStream(Stream dataStream, ulong totalContentSizeInBytes)
        {
            var contentData = new List<byte>();
            var bytesRead = (ulong)0;

            while (bytesRead < totalContentSizeInBytes)
            {
                bytesRead += ChunkSize;
                var chunk = new byte[ChunkSize];
                _ = dataStream.Read(chunk, 0, ChunkSize);

                contentData.AddRange(chunk);
            }

            return new ReadOnlySpan<byte>(contentData.ToArray());
        }
    }
}