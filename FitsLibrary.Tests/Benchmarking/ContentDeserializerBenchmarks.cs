using System.IO.Pipelines;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FitsLibrary.Deserialization.Image;
using FitsLibrary.DocumentParts.ImageData;

namespace FitsLibrary.Tests.Benchmarking;

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net80, launchCount: 5, warmupCount: 5, iterationCount: 5)]
[MemoryDiagnoser]
public class ContentDeserializerBenchmarks
{
    private PipeReader TestEmptyStream = null!;
    private ImageHeader EmptyHeader = null!;
    private ImageHeader HeaderWith1Axis = null!;
    private ImageHeader HeaderWith2Axis = null!;
    private PipeReader TestWith10IntValues = null!;
    private PipeReader TestWith1MillionFloatValues = null!;
    private ImageContentDeserializer<float> ContentDeserializer = null!;

    [IterationSetup]
    public void Setup()
    {
        TestEmptyStream = PipeReader.Create(new ContentStreamBuilder()
            .WithEmptyContent()
            .Build());
        EmptyHeader = new ImageHeader(new HeaderBuilder()
            .WithNumberOfAxis(0)
            .Build().Entries);

        HeaderWith1Axis = new ImageHeader(new HeaderBuilder()
            .WithContentDataType(DataContentType.FLOAT)
            .WithNumberOfAxis(1)
            .WithAxisOfSize(dimensionIndex: 1, size: 10)
            .Build().Entries);
        TestWith10IntValues = PipeReader.Create(new ContentStreamBuilder()
            .WithDataBeingInitializedWith(123, HeaderWith1Axis)
            .Build());

        HeaderWith2Axis = new ImageHeader(new HeaderBuilder()
            .WithContentDataType(DataContentType.FLOAT)
            .WithNumberOfAxis(2)
            .WithAxisOfSize(dimensionIndex: 1, size: 1024)
            .WithAxisOfSize(dimensionIndex: 2, size: 1024)
            .Build().Entries);
        TestWith1MillionFloatValues = PipeReader.Create(new ContentStreamBuilder()
            .WithDataBeingInitializedWith(123.45f, HeaderWith2Axis)
            .Build());

        ContentDeserializer = new ImageContentDeserializer<float>();
    }

    [Benchmark]
    public Task<(bool, ImageDataContent<float>?)> WithEmptyContentStream() => ContentDeserializer.DeserializeAsync(TestEmptyStream, EmptyHeader);

    [Benchmark]
    public Task<(bool, ImageDataContent<float>?)> With10IntValues() => ContentDeserializer.DeserializeAsync(TestWith10IntValues, HeaderWith1Axis);

    [Benchmark]
    public Task<(bool, ImageDataContent<float>?)> With1MillionFloatValues() => ContentDeserializer.DeserializeAsync(TestWith1MillionFloatValues, HeaderWith2Axis);
}
