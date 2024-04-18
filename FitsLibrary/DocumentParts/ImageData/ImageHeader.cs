
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FitsLibrary.DocumentParts.Objects;

namespace FitsLibrary.DocumentParts.ImageData;

/// <summary>
/// Contains a few convenience properties related to image headers
/// </summary>
public class ImageHeader : Header
{
    private DataContentType? _cachedDataContentType;

    /// <summary>
    /// Returns the type of the data (integer, float, etc)
    /// </summary>
    public DataContentType DataContentType =>
        _cachedDataContentType ??= (DataContentType)Convert.ToInt32(this["BITPIX"]!, CultureInfo.InvariantCulture);

    private int? _cachedNumberOfAxisInMainContent;

    /// <summary>
    /// Returns the number of axis inside the primary data array
    /// </summary>
    public int NumberOfAxisInMainContent =>
        _cachedNumberOfAxisInMainContent ??= Convert.ToInt32(this["NAXIS"]!, CultureInfo.InvariantCulture);

    private int[]? _cachedAxisSizes;

    public ImageHeader(IList<HeaderEntry> entries) : base(entries)
    {
    }

    /// <summary>
    /// Returns an array containing the length of each axis in the documents content
    /// </summary>
    public int[] AxisSizes =>
        _cachedAxisSizes ??= Enumerable
            .Range(0, this.NumberOfAxisInMainContent)
            .Select(i => Convert.ToInt32(this[$"NAXIS{i + 1}"], CultureInfo.InvariantCulture))
            .ToArray();
}
