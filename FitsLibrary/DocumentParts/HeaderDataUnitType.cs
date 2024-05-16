namespace FitsLibrary.DocumentParts;

public enum HeaderDataUnitType
{
    /// <summary>
    /// Marks the primary HDU (the first one of the fits file)
    /// </summary>
    PRIMARY = 0,
    /// <summary>
    /// Marks the HDU to contain image or n-dimensional data
    /// </summary>
    IMAGE = 1,
    /// <summary>
    /// Marks the HDU to contain table data
    /// </summary>
    TABLE = 2,
}
