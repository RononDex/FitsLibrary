using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.Extensions;

namespace FitsLibrary.Tests;

public class ContentStreamBuilder
{
    private readonly List<byte> data = new();

    public MemoryStream Build()
    {
        return new MemoryStream(data.ToArray());
    }

    public ContentStreamBuilder WithEmptyContent()
    {
        data.Clear();
        return this;
    }

    public ContentStreamBuilder WithDataBeingInitializedWith(object defaultData, ImageHeader header)
    {
        var numberOfAxis = (int?)header["NAXIS"];
        var axisSizes = Enumerable.Range(1, numberOfAxis!.Value)
            .Select(axisIndex => header[$"NAXIS{axisIndex}"] as long?);
        var totalNumberOfValues = axisSizes.Aggregate((long?)1, (x, y) => x!.Value * y!.Value);

        var valueInBytes = GetValueInBytes(defaultData, header);
        data.AddRange(Enumerable.Repeat(
            valueInBytes,
            Convert.ToInt32(totalNumberOfValues!.Value)).SelectMany(_ => _).ToArray());

        return this;
    }

    public ContentStreamBuilder WithDataAtCoordinates(object value, Dictionary<uint, ulong> coordinates, ImageHeader header)
    {
        var numberOfAxis = (int?)header["NAXIS"];
        var axisSizes = Enumerable.Range(1, numberOfAxis!.Value)
            .Select(axisIndex => Convert.ToUInt64(header[$"NAXIS{axisIndex}"] as long?));
        var sizePerValue = Convert.ToUInt64(Math.Abs((int)header.DataContentType)) / 8;
        var byteIndex = coordinates
            .Select(coordinate =>
                sizePerValue *
                coordinate.Value *
                axisSizes
                .Where((_, axisSizeIndex) => axisSizeIndex <= Convert.ToInt32(coordinate.Key) - 1)
                .Aggregate(Convert.ToUInt64(1), (x, y) => x * y))
            .Aggregate(Convert.ToUInt64(0), (x, y) => x + y);

        var valueInBytes = GetValueInBytes(value, header);

        for (var i = 0; i < valueInBytes.Length; i++)
        {
            data[Convert.ToInt32(byteIndex) + i] = valueInBytes[i];
        }

        return this;
    }

    public ContentStreamBuilder WithAdditionalDataAfterContent()
    {
        data.AddRange(Enumerable.Repeat((byte)0x01, 2880));
        return this;
    }

    private static byte[] GetValueInBytes(object value, ImageHeader header)
    {
        return header.DataContentType switch
        {
            DataContentType.BYTE => new[] { (byte)value },
            DataContentType.SHORT => BitConverter.GetBytes((short)value).ConvertLittleEndianToBigEndianIfNecessary(),
            DataContentType.INTEGER => BitConverter.GetBytes((int)value).ConvertLittleEndianToBigEndianIfNecessary(),
            DataContentType.LONG => BitConverter.GetBytes((long)value).ConvertLittleEndianToBigEndianIfNecessary(),
            DataContentType.FLOAT => BitConverter.GetBytes((float)value).ConvertLittleEndianToBigEndianIfNecessary(),
            DataContentType.DOUBLE => BitConverter.GetBytes((double)value).ConvertLittleEndianToBigEndianIfNecessary(),
            _ => throw new Exception("Unexpected Case"),
        };
    }
}

