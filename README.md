# FitsLibrary
[![License](https://img.shields.io/badge/license-MPL2.0%20-green)](https://choosealicense.com/licenses/mpl-2.0/)

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

Data can be read using 
```csharp
fitsFile.Content
```
For example to get a the value at certain coordinates:
```csharp
fitsFile.Content.Data.Single(dp => dp.Coordinates[0] == xCoordinate && dp.Coordinates[1] == yCoordiante).Value as float;
```
Where `xCoordiante` and `yCoordinate` are the coordinates one wants to get the data from
