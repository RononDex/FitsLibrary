using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.SampleFiles
{
    [SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net70, launchCount: 5, warmupCount: 5, iterationCount: 5)]
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

            for (int x = 0; x < document.Header.AxisSizes[0]; x++)
            {
                for (int y = 0; y < document.Header.AxisSizes[1]; y++)
                {
                    var valueAtXY = document.GetValueAt(x, y);
                }
            }

            var endTime = DateTime.Now;
            Console.WriteLine($"Sample file read in {(endTime - startTime).TotalSeconds}s");
        }

        [Test]
        public async Task OpenFitsFile_WithWrongGenericType_ThrowsInvalidArgumentException()
        {
            var reader = new FitsDocumentReader<int>();

            Func<Task> act = () => reader.ReadAsync("SampleFiles/FOCx38i0101t_c0f.fits");

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        [Benchmark]
        public async Task OpenFitsFile_WithAccessingContent_IsAbleToAccessData()
        {
            var reader = new FitsDocumentReader<float>();
            var document = await reader.ReadAsync("SampleFiles/FOCx38i0101t_c0f.fits");

            for (int x = 0; x < document.Header.AxisSizes[0]; x++)
            {
                for (int y = 0; y < document.Header.AxisSizes[1]; y++)
                {
                    var valueAtXY = document.GetValueAt(x, y);
                }
            }
        }

        // Extensions are not yet implemented
        // [Test]
        // public async Task OpenFitsFile_WithExtensions_LoadsExtensions()
        // {
        //     var reader = new FitsDocumentReader<float>();
        //
        //     var document = await reader.ReadAsync("SampleFiles/FOCx38i0101t_c0f.fits");
        //
        //     document.Extensions.Should().NotBeNull();
        //     document.Extensions.Should().HaveCount(1);
        // }
    }
}
