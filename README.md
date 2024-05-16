# FitsLibrary

[![License](https://img.shields.io/badge/license-MPL2.0%20-blue)](https://choosealicense.com/licenses/mpl-2.0/) ![master branch](https://github.com/RononDex/FitsLibrary/workflows/.NET/badge.svg?branch=master) [![Nuget](https://img.shields.io/nuget/v/FitsLibrary.svg)](https://www.nuget.org/packages/FitsLibrary/)

**This project is currently in pre-release, not all functionality is implemented yet. Use at own risk!**

FitsLibrary is a native C# / dotnet core implementation using the most up to date **FITS 4.0** specification for writing and reading astronomical FITS files.

The library focuses on being fast and easy to use.
It makes use of the newer C# generic maths features, which require net8.0.

# Table of Contents  
* [Current Development state](#devState)  
* [Usage](#usage)
  * [Accessing header](#readingHeader)
  * [Accessing data](#readingContent)
  * [Accessing raw data](#readingRawContent)
  * [Getting the datatype of priamry hdu](#readingDataType)
  * [Getting the datatype of priamry hdu](#readingDataType)

<a name="devState"/>
# Current Development state

## What currently works

-   Loading of header data
-   Validation of header content
-   Reading of N-Dimensional data arrays (PRIMARY and IMAGE hdu's)
-   Writing .fits files

## What doesn't work

-   Reading / writing of extensions of type TABLE and BINTABLE

<a name="usage"/>
# Usage

<a name="readingHeader"/>
## Reading header data

Every HDU comes with a header with meta-information:
```
var header = fitsFile.HeaderDataUnits[x].Header;
```
Which can then be acceessed like this:
```
var value = header["SomeValue"] as string;
```
Note that values are stored as type `object` but can be cast to whatever type the value has (like `string`, `long`, `float`,
...)

If you want to just read the primary header of a file, without opening the whole file you can do

```
var header = FitsDocumentHelper.ReadHeaderAsync("path/to/file");
```

You can assign values through
```
var exampleValue = 1.2f;
header.Entries.Add(new HeaderEntry("exampleKey", exampleValue));
// or with comment
header.Entries.Add(new HeaderEntry("exampleKey", exampleValue, "some Comment"));
```

<a name="readingContent"/>
## Reading content data

This library can open Fits files in a strong typed way (if you know the type of data inside the primary hdu beforehand) or a type unspecific way.

### Strong typed

Open a fits file using

```csharp
var reader = new FitsDocumentReader<float>();
var fitsFile = await reader.ReadAsync("SampleFiles/FOCx38i0101t_c0f.fits"); // or reader.ReadAsync(stream)
```
And then access the data in a strong typed way through

```csharp
for (int x = 0; x < fitsFile.PrimaryHdu.Header.AxisSizes[0]; x++) {
    for (int y = 0; y < fitsFile.PrimaryHdu.Header.AxisSizes[1]; y++) {
        var valueAtXY = fitsFile.PrimaryHdu.Data.GetValueAt(x, y);
    }
 }
```

### Untyped 

```csharp
var reader = new FitsDocumentReader();
var fitsFile = await reader.ReadAsync(FITS_FILE);
```

And then access the data in a strong typed way through

```
var primaryHdu = (ImageHeaderDataUnit<short>)fitsFile.HeaderDataUnits[0];
for (var x = 0; x < fitsFile.HeaderDataUnits[0].Header.AxisSizes[0]; x++)
{
    for (var y = 0; y < fitsFile.HeaderDataUnits[0].Header.AxisSizes[1]; y++)
    {
        var val = primaryHdu.Data.GetValueAt(x, y);
    }
}
```

<a name="readingRawContent"/>
## Accessing Raw data

By accessing `hdu.Data.RawData` (which is of type `Memory<T>`, use .Span to access data or much slower .ToArray())
This is used for fast access, does not differentiate between dimensions.
Index for value in 2 dimensional data for example is calculated like this:

```csharp
index = indexAxis1 + (axisSize1 * indexAxis2)
fitsFile.Content.Span[index];
```

<a name="readingDataType"/>
## Find the datatype of a Header Data Unit (HDU):

If you want to know the data type stored in the primary hdu of a fits file, without opening the whole file you can do:

```csharp
var dataType = await FitsDocumentHelper.GetDocumentContentType("SampleFiles/FOCx38i0101t_c0f.fits");
```

If you have already opened the file, you can do
```csharp
var reader = new FitsDocumentReader<float>();
var fitsFile = await reader.ReadAsync("SampleFiles/FOCx38i0101t_c0f.fits");
```
