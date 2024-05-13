using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FitsLibrary.DocumentParts;
using NUnit.Framework;

namespace FitsLibrary.Tests.SampleFiles;

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net80, launchCount: 10, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class SampleFilesTests
{
    [Test]
    [Benchmark]
    public async Task OpenFitsFile_WithFOCFile_ReadsFileAsync()
    {
        Console.WriteLine("Reading sample file");
        var startTime = DateTime.Now;

        var reader = new FitsDocumentReader();
        var document = await reader.ReadAsync("SampleFiles/FOCx38i0101t_c0f.fits");
        var priamryHdu = (ImageHeaderDataUnit<float>)document.HeaderDataUnits[0];

        for (int x = 0; x < priamryHdu.Header.AxisSizes[0]; x++)
        {
            for (int y = 0; y < priamryHdu.Header.AxisSizes[1]; y++)
            {
                var valueAtXY = priamryHdu.Data.GetValueAt(x, y);
            }
        }

        var endTime = DateTime.Now;
        Console.WriteLine($"Sample file read in {(endTime - startTime).TotalSeconds}s");
    }

    [Test]
    [Benchmark]
    public async Task OpenFitsFile_WithSimpleWFPFile_ReadsAndWritesFileAsync()
    {
        Console.WriteLine("Reading sample file");
        var startTime = DateTime.Now;

        var reader = new FitsDocumentReader();
        var document = await reader.ReadAsync("SampleFiles/WFPC2ASSNu5780205bx.fits");
        var priamryHdu = (ImageHeaderDataUnit<float>)document.HeaderDataUnits[0];

        var endTime = DateTime.Now;
        Console.WriteLine($"Sample file read in {(endTime - startTime).TotalSeconds}s");

        startTime = DateTime.Now;

        var writer = new FitsDocumentWriter();
        // var path = Path.GetTempFileName();
        var path = "test.fits";
        await writer.WriteAsync(document, path);
        var hashOriginal = SHA256.HashData(File.OpenRead("SampleFiles/WFPC2ASSNu5780205bx.fits"));
    }

    [Benchmark]
    public async Task OpenFitsFile_WithAccessingContent_Benchmark()
    {
        var reader = new FitsDocumentReader<short>();
        var document = await reader.ReadAsync("/home/cobra/M_101_Light_L_120_secs_2023-05-26T23-00-11_001.fits");
        var priamryHdu = document.PrimaryHdu;

        for (int x = 0; x < priamryHdu.Header.AxisSizes[0]; x++)
        {
            for (int y = 0; y < priamryHdu.Header.AxisSizes[1]; y++)
            {
                var valueAtXY = priamryHdu.Data.GetValueAt(x, y);
            }
        }
    }
}
