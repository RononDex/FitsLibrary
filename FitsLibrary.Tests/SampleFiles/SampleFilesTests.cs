using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using NUnit.Framework;

namespace FitsLibrary.Tests.SampleFiles
{
    [SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net70, launchCount: 5, warmupCount: 5, targetCount: 5)]
    [MemoryDiagnoser]
    public class SampleFilesTests
    {
        [Test]
        [Benchmark]
        public async Task OpenFitsFile_WithFOCFile_ReadsFileAsync()
        {
            Console.WriteLine("Reading sample file");
            var startTime = DateTime.Now;

            var reader = new FitsDocumentReader<float>();
            var document = await reader.ReadAsync("SampleFiles/FOCx38i0101t_c0f.fits");

            var endTime = DateTime.Now;
            Console.WriteLine($"Sample file read in {(endTime - startTime).TotalSeconds}s");
        }
    }
}
