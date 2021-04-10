// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;
// using FitsLibrary.DocumentParts;
// using FitsLibrary.DocumentParts.Objects;
// using FitsLibrary.Extensions;
//
// namespace FitsLibrary.Deserialization
// {
//     public class ContentDeserializer : IContentDeserializer
//     {
//         private const int ChunkSize = 2880;
//
//         public async Task<Content?> DeserializeAsync(Stream dataStream, Header header)
//         {
//             if (header.NumberOfAxisInMainContent == 0)
//             {
//                 return null;
//             }
//
//             var dataPoints = new List<DataPoint>();
//             var numberOfBytesPerValue = Math.Abs((int)header.DataContentType / 8);
//             var axisSizes = Enumerable.Range(1, header.NumberOfAxisInMainContent)
//                 .Select(axisIndex => Convert.ToUInt64(header[$"NAXIS{axisIndex}"])).ToArray();
//             var totalNumberOfValues = axisSizes.Aggregate((ulong)1, (x, y) => x * y);
//             var contentSizeInBytes = numberOfBytesPerValue * Convert.ToInt32(totalNumberOfValues);
//             var totalContentSizeInBytes = Math.Ceiling(Convert.ToDouble(contentSizeInBytes) / Convert.ToDouble(ChunkSize)) * ChunkSize;
//             var currentCoordinates = new ulong[header.NumberOfAxisInMainContent];
//
//             var contentData = await ReadContentDataStreamAsync(dataStream, totalContentSizeInBytes).ConfigureAwait(false);
//
//             for (int i = 0; i < contentSizeInBytes; i += numberOfBytesPerValue)
//             {
//                 var upperIndex = i + numberOfBytesPerValue;
//                 var currentValueBytes = contentData[i..upperIndex]
//                     .ConvertBigEndianToLittleEndianIfNecessary();
//
//                 var coordinates = new ulong[header.NumberOfAxisInMainContent];
//                 Array.Copy(currentCoordinates, coordinates, header.NumberOfAxisInMainContent);
//
//                 var value = ParseValue(header, currentValueBytes);
//
//                 dataPoints.Add(new DataPoint(coordinates, value));
//
//                 MoveToNextCoordinate(axisSizes, currentCoordinates);
//             }
//
//             return new Content(dataPoints);
//         }
//
//         private static void MoveToNextCoordinate(ulong[] axisSizes, ulong[] currentCoordinates)
//         {
//             var maxAxisReached = false;
//             for (var axis = 0; axis < axisSizes.Length && !maxAxisReached; axis++)
//             {
//                 if (axisSizes[axis] == currentCoordinates[axis] + 1)
//                 {
//                     maxAxisReached = false;
//                     currentCoordinates[axis] = 0;
//                 }
//                 else
//                 {
//                     currentCoordinates[axis]++;
//                     break;
//                 }
//             }
//         }
//
//         private static object ParseValue(Header header, byte[] currentValueBytes)
//         {
//             return header.DataContentType switch
//             {
//                 DataContentType.DOUBLE => BitConverter.ToDouble(currentValueBytes),
//                 DataContentType.FLOAT => BitConverter.ToSingle(currentValueBytes),
//                 DataContentType.BYTE => currentValueBytes.Single(),
//                 DataContentType.SHORT => BitConverter.ToInt16(currentValueBytes),
//                 DataContentType.INTEGER => BitConverter.ToInt32(currentValueBytes),
//                 DataContentType.LONG => BitConverter.ToInt64(currentValueBytes) as object,
//                 _ => throw new InvalidDataException("Invalid data type"),
//             };
//         }
//
//         private static async Task<byte[]> ReadContentDataStreamAsync(Stream dataStream, double totalContentSizeInBytes)
//         {
//             var contentData = new List<byte>();
//             var bytesRead = (long)0;
//
//             while (bytesRead < totalContentSizeInBytes)
//             {
//                 bytesRead += ChunkSize;
//                 var chunk = new byte[ChunkSize];
//                 _ = await dataStream.ReadAsync(chunk, 0, ChunkSize).ConfigureAwait(false);
//
//                 contentData.AddRange(chunk);
//             }
//
//             return contentData.ToArray();
//         }
//     }
// }
