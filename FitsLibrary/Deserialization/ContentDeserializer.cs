using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task<Content?> DeserializeAsync(Stream dataStream, Header header)
        {
            if (header.NumberOfAxisInMainContet == 0)
            {
                return null;
            }

            var dataPoints = new List<DataPoint>();
            var numberOfBytesPerValue = Math.Abs((int)header.DataContentType / 8);
            var axisSizes = Enumerable.Range(1, header.NumberOfAxisInMainContet)
                .Select(axisIndex => Convert.ToUInt64(header[$"NAXIS{axisIndex}"])).ToArray();
            var totalNumberOfValues = axisSizes.Aggregate((ulong)1, (x, y) => x * y);
            var contentSizeInBytes = numberOfBytesPerValue * Convert.ToInt32(totalNumberOfValues);
            var totalContentSizeInBytes = Math.Ceiling(Convert.ToDouble(contentSizeInBytes) / Convert.ToDouble(ChunkSize)) * ChunkSize;
            var currentCoordinates = new ulong[header.NumberOfAxisInMainContet];
            var contentData = new List<byte>();

            var bytesRead = (long)0;

            while (bytesRead < totalContentSizeInBytes)
            {
                bytesRead += ChunkSize;
                var chunk = new byte[ChunkSize];
                _ = await dataStream.ReadAsync(chunk, 0, ChunkSize).ConfigureAwait(false);

                contentData.AddRange(chunk);
            }

            for (int i = 0; i < contentSizeInBytes; i += numberOfBytesPerValue)
            {
                var currentValueBytes = contentData
                    .Skip(i)
                    .Take(numberOfBytesPerValue)
                    .ToArray(numberOfBytesPerValue)
                    .ConvertBigEndianToLittleEndianIfNecessary();

                var maxAxisReached = false;
                var coordinates = currentCoordinates.ToArray(currentCoordinates.Length);

                var value = header.DataContentType switch
                {
                    DataContentType.DOUBLE => BitConverter.ToDouble(currentValueBytes.ToArray(numberOfBytesPerValue)),
                    DataContentType.FLOAT => BitConverter.ToSingle(currentValueBytes.ToArray(numberOfBytesPerValue)),
                    DataContentType.BYTE => contentData[i],
                    DataContentType.SHORT => BitConverter.ToInt16(currentValueBytes.ToArray(numberOfBytesPerValue)),
                    DataContentType.INTEGER => BitConverter.ToInt32(currentValueBytes.ToArray(numberOfBytesPerValue)),
                    DataContentType.LONG => BitConverter.ToInt64(currentValueBytes.ToArray(numberOfBytesPerValue)) as object,
                    _ => throw new InvalidDataException("Invalid data type"),
                };

                dataPoints.Add(new DataPoint(coordinates, value));

                for (var axis = 0; axis < header.NumberOfAxisInMainContet && !maxAxisReached; axis++)
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

            return new Content(dataPoints);
        }
    }
}
