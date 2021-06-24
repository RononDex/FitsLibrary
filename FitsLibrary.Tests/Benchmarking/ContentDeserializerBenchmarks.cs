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
    public class ContentDeserializerBenchmarks
    {
        private PipeReader TestEmptyStream;
        private Header EmptyHeader;
        private Header HeaderWith1Axis;
        private Header HeaderWith2Axis;
        private PipeReader TestWith10IntValues;
        private PipeReader TestWith1MillionFloatValues;
        private ContentDeserializer ContentDeserializer;

        [IterationSetup]
        public void Setup()
        {
            TestEmptyStream = PipeReader.Create(new ContentStreamBuilder()
                .WithEmptyContent()
                .Build());
            EmptyHeader = new HeaderBuilder()
                .WithNumberOfAxis(0)
                .Build();

            HeaderWith1Axis = new HeaderBuilder()
                .WithContentDataType(DataContentType.INTEGER)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            TestWith10IntValues = PipeReader.Create(new ContentStreamBuilder()
                .WithDataBeingInitializedWith(123, HeaderWith1Axis)
                .Build());

            HeaderWith2Axis = new HeaderBuilder()
                .WithContentDataType(DataContentType.FLOAT)
                .WithNumberOfAxis(2)
                .WithAxisOfSize(dimensionIndex: 1, size: 1024)
                .WithAxisOfSize(dimensionIndex: 2, size: 1024)
                .Build();
            TestWith1MillionFloatValues = PipeReader.Create(new ContentStreamBuilder()
                .WithDataBeingInitializedWith(123.45f, HeaderWith2Axis)
                .Build());

            ContentDeserializer = new ContentDeserializer();
        }

        [Benchmark]
        public Task<Memory<object>?> WithEmptyContentStream() => ContentDeserializer.DeserializeAsync(TestEmptyStream, EmptyHeader);

        [Benchmark]
        public Task<Memory<object>?> With10IntValues() => ContentDeserializer.DeserializeAsync(TestWith10IntValues, HeaderWith1Axis);

        [Benchmark]
        public Task<Memory<object>?> With1MillionFloatValues() => ContentDeserializer.DeserializeAsync(TestWith1MillionFloatValues, HeaderWith2Axis);
    }
}
