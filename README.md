# FitsLibrary

[![License](https://img.shields.io/badge/license-MPL2.0%20-blue)](https://choosealicense.com/licenses/mpl-2.0/) ![master branch](https://github.com/RononDex/FitsLibrary/workflows/.NET/badge.svg?branch=master) [![Nuget](https://img.shields.io/nuget/v/FitsLibrary.svg)](https://www.nuget.org/packages/FitsLibrary/)

**This project is currently a very early WIP, use at own risk!**

FitsLibrary is a native C# / dotnet core implementation using the most up to date **FITS 4.0** specification for writing and reading astronomical FITS files.

The library focuses on being fast and easy to use.
It makes use of the newer C# generic maths features, which require net7.0.

# What currently works

-   Loading of header data
-   Validation of header content
-   Reading of N-Dimensional data arrays

# What doesn't work

-   Reading of extension headers
-   Writing .fits files

# Usage

Open a fits file using

```csharp
var reader = new FitsDocumentReader<float>();
var fitsFile = await reader.ReadAsync("SampleFiles/FOCx38i0101t_c0f.fits");
```

You could then iterate over a file with 2D using
```csharp
for (int x = 0; x < document.Header.AxisSizes[0]; x++) {
    for (int y = 0; y < document.Header.AxisSizes[1]; y++) {
        var valueAtXY = document.GetValueAt(x, y);
    }
 }
```

Note that the generic parameter (`float` in the above example) has to match the datatype in the fits file.

If you do not know the datatype of the fits file beforehand, you can use `FitsDocumentHelper` to get the type of the data inside the file:

```csharp
var dataType = await FitsDocumentHelper.GetDocumentContentType("SampleFiles/FOCx38i0101t_c0f.fits");
```

The FitsDocument can take any I/O Stream and work with it.

## Accessing Header values

Header values can have different data types (string, integer, float, ...)

They can be read using

```csharp
var headerValue = fitsFile.Header["TestHeaderKey"] as string
```

## Accessing Content data


Data can be accessed in different ways:
### By coordinates

To get the value at specific coordinates, do

```csharp
var valueAtXY = fitsFile.GetValueAt(x, y);
```

### RawData

By accesing `fitsFile.Content` (which is of type `Memory<T>`, use .Span to access data or much slower .ToArray())
This is used for fast access, does not differentiate between dimensions.
Index for value in 2 dimensional data for example is calculated like this:

```csharp
index = indexAxis1 + (axisSize1 * indexAxis2)
fitsFile.Content.Span[index];
```



There is a typed functions for all supported data types by the fits standard (byte, 16-bit integer, 32-bit integer,
64-bit integer, 32-bit float, 64-bit float)

**Be aware, these methods make no sanity checks for performance reasons.**
If you enter coordinates that do not exists or exceed their length expect random numbers to be returned!
Also, if you try to access data in the wrong datatybe, like for example as int32 while data is int64, expect exceptions. To check the datatype of the document
use `fitsFile.Header.DataContentType`.

The sizes of the axis can be determined using:

```csharp
var numberOfAxis = fitsFile.Header.NumberOfAxisInMainContent;
var axis1Size = fitsFile.Header["NAXIS1"] as long;
var axi2Size = fitsFile.Header["NAXIS2"] as long;
...
```
