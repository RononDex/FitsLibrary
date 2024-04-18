using System.Collections.Generic;
using System.Numerics;
using FitsLibrary.DocumentParts;

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
}

public class FitsDocument<PrimaryDataType> : FitsDocument where PrimaryDataType : INumber<PrimaryDataType>
{
    public ImageHeaderDataUnit<PrimaryDataType> PrimaryHdu { get; }
    public FitsDocument(IList<HeaderDataUnit> hdus) : base(hdus)
    {
        this.PrimaryHdu = (ImageHeaderDataUnit<PrimaryDataType>)hdus[0];
    }
}
