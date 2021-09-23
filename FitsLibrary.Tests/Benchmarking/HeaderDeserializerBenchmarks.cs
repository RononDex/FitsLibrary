using System;
using System.IO.Pipelines;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FitsLibrary.Deserialization;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Tests.Benchmarking
{
    [SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.NetCoreApp50, launchCount: 5, warmupCount: 5, targetCount: 5)]
    [MemoryDiagnoser]
    public class HeaderDeserializerBenchmarks
    {
        private PipeReader headerDataWithOneEntry = null!;
        private readonly HeaderDeserializer headerDeserializer = new();

        [IterationSetup]
        public void Setup()
        {
            var testData = new byte[2881];
            testData = TestUtils.AddHeaderEntry(
                data: testData,
                startIndex: 0,
                key: "TEST",
                value: 1,
                comment: "some test comment");
            testData = TestUtils.AddContentToArray(
                data: testData,
                startIndex: 80,
                content: HeaderDeserializer.END_MARKER);
            headerDataWithOneEntry = TestUtils.ByteArrayToStream(testData);

            Console.WriteLine("Initialized!");
        }

        [Benchmark]
        public Task<(bool, Header?)> ParseHeader_WithOneEntry() => headerDeserializer.DeserializeAsync(headerDataWithOneEntry);
    }
}
