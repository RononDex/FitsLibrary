using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.DocumentParts.Objects;

namespace FitsLibrary.DocumentParts;

public class Header
{
    private readonly IList<HeaderEntry> _entries;

    /// <summary>
    /// Initializes a header with the given entries
    /// </summary>
    /// <param name="entries">A list of entries used to initialize the
    /// header</param>
    public Header(IList<HeaderEntry> entries)
    {
        _entries = entries;
    }

    /// <summary>
    /// Initializes an empty header
    /// </summary>
    internal Header()
    {
        _entries = new List<HeaderEntry>();
    }
    /// <summary>
    /// A list of entries contained within the header
    /// </summary>
    public IList<HeaderEntry> Entries => _entries;

    /// <summary>
    /// Gets the value for the given header entry.
    /// Returns only first found entry if there are multiple with the same key
    /// </summary>
    /// <param name="key">The key for which to search in the header</param>
    public object? this[string key] =>
        _entries.FirstOrDefault(entry => string.Equals(entry.Key, key, StringComparison.Ordinal))?.Value;

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

    /// <summary>
    /// Returns an array containing the length of each axis in the documents content
    /// </summary>
    public int[] AxisSizes =>
        _cachedAxisSizes ??= Enumerable
            .Range(0, this.NumberOfAxisInMainContent)
            .Select(i => Convert.ToInt32(this[$"NAXIS{i + 1}"], CultureInfo.InvariantCulture))
            .ToArray();
}
