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
            var numberOfBytesPerValue = (int)header.DataContentType / 8;
            var axisSizes = Enumerable.Range(1, header.NumberOfAxisInMainContet)
                .Select(axisIndex => (header[$"NAXIS{axisIndex}"] as long?)!.Value).ToArray();
            var totalNumberOfValues = axisSizes.Aggregate((long)1, (x, y) => x * y);
            var contentSizeInBytes = numberOfBytesPerValue * totalNumberOfValues;
            var totalContentSizeInBytes = Math.Ceiling(Convert.ToDouble(contentSizeInBytes) / Convert.ToDouble(ChunkSize)) * ChunkSize;
            var currentCoordinates = new long[header.NumberOfAxisInMainContet];
            var contentData = new List<byte>();

            var bytesRead = (long)0;
            while (bytesRead < totalContentSizeInBytes)
            {
                bytesRead += ChunkSize;
                var chunk = new byte[ChunkSize];
                _ = await dataStream.ReadAsync(chunk, 0, ChunkSize).ConfigureAwait(false);

                contentData.AddRange(chunk);
            }

            for (var i = 0; i < contentSizeInBytes; i += numberOfBytesPerValue)
            {
                var currentValueBytes = contentData
                    .Skip(i)
                    .Take(numberOfBytesPerValue)
                    .ToArray()
                    .ConvertBigEndianToLittleEndianIfNecessary();

                var maxAxisReached = false;
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
                var coordinates = currentCoordinates
                    .Select((value, index) => new KeyValuePair<uint, ulong>(
                                Convert.ToUInt32(index),
                                Convert.ToUInt64(value)))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                object value = header.DataContentType switch
                {
                    DataContentType.DOUBLE => (double)BitConverter.ToDouble(currentValueBytes.ToArray()),
                    DataContentType.FLOAT => (float)BitConverter.ToSingle(currentValueBytes.ToArray()),
                    DataContentType.BYTE => (byte)contentData[i],
                    DataContentType.SHORT => (short)BitConverter.ToInt16(currentValueBytes.ToArray()),
                    DataContentType.INTEGER => (int)BitConverter.ToInt32(currentValueBytes.ToArray()),
                    DataContentType.LONG => (long)BitConverter.ToInt64(currentValueBytes.ToArray()),
                    _ => throw new InvalidDataException("Invalid data type"),
                };

                dataPoints.Add(new DataPoint(coordinates, value));
            }

            return new Content(dataPoints);
        }
    }
}
