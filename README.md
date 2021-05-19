# FitsLibrary
[![License](https://img.shields.io/badge/license-MPL2.0%20-blue)](https://choosealicense.com/licenses/mpl-2.0/) ![master branch](https://github.com/RononDex/FitsLibrary/workflows/.NET/badge.svg?branch=master) [![Nuget](https://img.shields.io/nuget/v/FitsLibrary.svg)](https://www.nuget.org/packages/FitsLibrary/) 

**This project is currently a very early WIP, use at own risk!**
**There is no nuget package released until the first "release" version is ready**

FitsLibrary is a native C# / dotnet core implementation using the most up to date **FITS 4.0** specification for writing and reading astronomical FITS files.

The library focuses on being fast and easy to use.
**This library has support for IoC (Inversion of Control), everything is implemented through interfaces** 

# What currently works
 - Loading of header data, including "CONTINUE" keywords for values spanning over multiple entries
 - Validation of header content
 - Reading of N-Dimensional data arrays

# What doesn't work
 - Extension Headers
 - Writing .fits files

# Usage
Open a fits file using
```csharp
var reader = new FitsDocumentReader();
var fitsFile = await reader.ReadAsync("SampleFiles/FOCx38i0101t_c0f.fits");
```

The FitsDocument can take any I/O Stream and work with it.

## Accessing Header values
Header values can have different data types (string, integer, float, ...)

They can be read using
```csharp
fitsFile.Header.Entries.Single(h => h.Key == "TestHeaderKey").Value as string
```

## Accessing Content data
Since the type of data can change (for example, int or float) per file, the only way to model the content of a .fits
file was to use `object` as dataype for the data. You have the responsibility, to cast the data to the correct type
before use (helper methods for this are planned).

Data can be accessed in different ways:

### RawData
By accesing `fitsFile.Content` (which is of type `Memory<T>`, use .Span to access data or much slower .ToArray())
This is used for fast access, does not differntiate between dimensions. 
Index for value in 2 dimensional data for example is calculated like this:
```csharp
index = indexAxis1 + (axisSize1 * indexAxis2)
fitsFile.Content.Span[index];
```

### By coordinates
To get the value at specific coordinates, do
```csharp
var value = fitsFile.GetFloat32ValueAt(x, y);
```
There is a typed functions for all supported data types by the fits standard (byte, 16-bit integer, 32-bit integer,
64-bit integer, 32-bit float, 64-bit float)

Be aware though, that this method for performance reasons makes no sansity checks.
If you enter coordinates that do not exists expect random numbers to be returned!
Also, if you try to access data as in32 while data is int64, expect exceptions. To check the datatype of the document
use `fitsFile.Header.DataContentType`.

