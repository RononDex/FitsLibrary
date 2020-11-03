# FitsLibrary
[![License](https://img.shields.io/badge/license-MPL%2.0%20-green)](https://choosealicense.com/licenses/mpl-2.0/)

**This project is currently a very early WIP, use at own risk!**
**There is no nuget package released until the first "release" version is ready**

FitsLibrary is a native C# / dotnet core implementation using up to date FITS specification for writing and reading astronomical FITS files.

# What currently works
 - Loading of header data, including "CONTINUE" keywords for values spanning over multiple entries

# What doesn't work
 - Everything else

# Usage
Open a fits file using
```csharp
var fitsFile = new FitsDocument(File.OpenRead("Path/To/FitsFile.fits"));
```

The FitsDocument can take any I/O Stream and work with it.
