using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.DocumentParts.Objects;

namespace FitsLibrary;

/// <summary>
/// Represents a .fits document for read and write access
/// </summary>
public class FitsDocument
{
    public IList<HeaderDataUnit> HeaderDataUnits { get; }

    /// <summary>
    /// Creates a new .fits document with a the given data
    /// </summary>
    /// <param name="hdus">A list of HeaderDataUnits contained in this document</param>
    public FitsDocument(IList<HeaderDataUnit> hdus)
    {
        this.HeaderDataUnits = hdus;
    }

    public FitsDocument(DataContentType primaryDocumentType, int[] axisSizes)
    {
        this.HeaderDataUnits = new List<HeaderDataUnit>();

        var primaryHeader = new Header([
                new HeaderEntry("SIMPLE", true),
                new HeaderEntry("BITPIX", (int)primaryDocumentType),
                new HeaderEntry("NAXIS", axisSizes.Length),
        ]);

        for (var i = 0; i < axisSizes.Length; i++)
        {
            primaryHeader.Entries.Add(new HeaderEntry($"NAXIS{i + 1}", axisSizes[i]));
        }

        this.HeaderDataUnits.Add(primaryDocumentType switch
        {
            DataContentType.BYTE => new ImageHeaderDataUnit<byte>(
                    HeaderDataUnitType.PRIMARY,
                    primaryHeader,
                    new ImageDataContent<byte>(axisSizes, new Memory<byte>(new byte[axisSizes.Aggregate(1, (x, y) => x * y)]))),
            DataContentType.FLOAT => new ImageHeaderDataUnit<float>(
                    HeaderDataUnitType.PRIMARY,
                    primaryHeader,
                    new ImageDataContent<float>(axisSizes, new Memory<float>(new float[axisSizes.Aggregate(1, (x, y) => x * y)]))),
            DataContentType.DOUBLE => new ImageHeaderDataUnit<double>(
                    HeaderDataUnitType.PRIMARY,
                    primaryHeader,
                    new ImageDataContent<double>(axisSizes, new Memory<double>(new double[axisSizes.Aggregate(1, (x, y) => x * y)]))),
            DataContentType.INT16 => new ImageHeaderDataUnit<short>(
                    HeaderDataUnitType.PRIMARY,
                    primaryHeader,
                    new ImageDataContent<short>(axisSizes, new Memory<short>(new short[axisSizes.Aggregate(1, (x, y) => x * y)]))),
            DataContentType.INT32 => new ImageHeaderDataUnit<int>(
                    HeaderDataUnitType.PRIMARY,
                    primaryHeader,
                    new ImageDataContent<int>(axisSizes, new Memory<int>(new int[axisSizes.Aggregate(1, (x, y) => x * y)]))),
            DataContentType.INT64 => new ImageHeaderDataUnit<long>(
                    HeaderDataUnitType.PRIMARY,
                    primaryHeader,
                    new ImageDataContent<long>(axisSizes, new Memory<long>(new long[axisSizes.Aggregate(1, (x, y) => x * y)]))),
            _ => throw new NotImplementedException()
        });
    }
}

public class FitsDocument<PrimaryDataType> : FitsDocument where PrimaryDataType : INumber<PrimaryDataType>
{
    public ImageHeaderDataUnit<PrimaryDataType> PrimaryHdu { get; }

    public FitsDocument(IList<HeaderDataUnit> hdus) : base(hdus)
    {
        this.PrimaryHdu = (ImageHeaderDataUnit<PrimaryDataType>)hdus[0];
    }

    public FitsDocument(int[] axisSizes) : base(GetDataContentTypeFromGenericParameter<PrimaryDataType>(), axisSizes)
    {
        this.PrimaryHdu = (ImageHeaderDataUnit<PrimaryDataType>)this.HeaderDataUnits[0];
    }

    private static DataContentType GetDataContentTypeFromGenericParameter<T>()
    {
        var genericType = typeof(T);
        if (genericType == typeof(byte))
            return DataContentType.BYTE;
        if (genericType == typeof(float))
            return DataContentType.FLOAT;
        if (genericType == typeof(double))
            return DataContentType.DOUBLE;
        if (genericType == typeof(short))
            return DataContentType.INT16;
        if (genericType == typeof(int))
            return DataContentType.INT32;
        if (genericType == typeof(long))
            return DataContentType.INT64;

        throw new NotImplementedException();
    }

}
