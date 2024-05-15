using System;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using nom.tam.fits;
using nom.tam.util;
using NUnit.Framework;

namespace FitsLibrary.Tests.Benchmarking;

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net80, launchCount: 5, warmupCount: 5, iterationCount: 5)]
[MemoryDiagnoser]
public class FrameworkWriteComparisonsTests
{
    FitsDocument<short>? fitsLibraryDocument = null;
    Fits? cSharpLibraryDocument = null;

    [GlobalSetup]
    [SetUp]
    public async Task Setup()
    {
        fitsLibraryDocument = new FitsDocument<short>([4000, 3000, 3]);

        for (var x = 0; x < 4000; x++)
        {
            for (var y = 0; y < 3000; y++)
            {
                for (var z = 0; z < 3; z++)
                {
                    var randomValue = (short)Random.Shared.NextInt64(short.MinValue, short.MaxValue);
                    fitsLibraryDocument.PrimaryHdu.Data.SetValueAt(randomValue, x, y, z);
                }
            }
        }
    }

    [Benchmark]
    [Test]
    public async Task FitsLibraryAsyncWrite()
    {
        var tempFilePath = Path.GetTempFileName();
        var writer = new FitsDocumentWriter();
        await writer.WriteAsync(fitsLibraryDocument!, "/home/cobra/test.fits");
    }

    // [Benchmark]
    // public void CSharpFITWrite()
    // {
    //     var tempFilePath = Path.GetTempFileName();
    //     var file = new BufferedFile("bt5.fits", FileAccess.ReadWrite, FileShare.ReadWrite);
    //     cSharpLibraryDocument!.Write(file);
    // }
}
