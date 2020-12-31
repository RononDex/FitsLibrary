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

# What doesn't work
 - Everything else

# Usage
Open a fits file using
```csharp
var reader = new FitsDocumentReader();
var document = await reader.ReadAsync("SampleFiles/FOCx38i0101t_c0f.fits");
```

The FitsDocument can take any I/O Stream and work with it.

## Accessing Header values
Header values can have different data types (string, integer, float, ...)

They can be read using
```csharp
fitsFile.Header.Entries.Single(h => h.Key == "TestHeaderKey").Value as string
```

