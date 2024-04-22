using System.IO.Pipelines;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FitsLibrary.Deserialization.Image;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;

namespace FitsLibrary.Tests.Benchmarking;

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net80, launchCount: 5, warmupCount: 5, iterationCount: 5)]
[MemoryDiagnoser]
public class ContentDeserializerBenchmarks
{
    private PipeReader TestEmptyStream = null!;
    private Header EmptyHeader = null!;
    private Header HeaderWith1Axis = null!;
    private Header HeaderWith2Axis = null!;
    private PipeReader TestWith10IntValues = null!;
    private PipeReader TestWith1MillionFloatValues = null!;
    private ImageContentDeserializer<float> ContentDeserializer = null!;

    [IterationSetup]
    public void Setup()
    {
        this.TestEmptyStream = PipeReader.Create(new ContentStreamBuilder()
            .WithEmptyContent()
            .Build());
        this.EmptyHeader = new HeaderBuilder()
            .WithNumberOfAxis(0)
            .Build();

        this.HeaderWith1Axis = new HeaderBuilder()
            .WithContentDataType(DataContentType.FLOAT)
            .WithNumberOfAxis(1)
            .WithAxisOfSize(dimensionIndex: 1, size: 10)
            .Build();
        this.TestWith10IntValues = PipeReader.Create(new ContentStreamBuilder()
            .WithDataBeingInitializedWith(123, this.HeaderWith1Axis)
            .Build());

        this.HeaderWith2Axis = new HeaderBuilder()
            .WithContentDataType(DataContentType.FLOAT)
            .WithNumberOfAxis(2)
            .WithAxisOfSize(dimensionIndex: 1, size: 1024)
            .WithAxisOfSize(dimensionIndex: 2, size: 1024)
            .Build();
        this.TestWith1MillionFloatValues = PipeReader.Create(new ContentStreamBuilder()
            .WithDataBeingInitializedWith(123.45f, this.HeaderWith2Axis)
            .Build());

        this.ContentDeserializer = new ImageContentDeserializer<float>();
    }

    [Benchmark]
    public Task<(bool, ImageDataContent<float>?)> WithEmptyContentStream() => this.ContentDeserializer.DeserializeAsync(TestEmptyStream, EmptyHeader);

    [Benchmark]
    public Task<(bool, ImageDataContent<float>?)> With10IntValues() => this.ContentDeserializer.DeserializeAsync(TestWith10IntValues, HeaderWith1Axis);

    [Benchmark]
    public Task<(bool, ImageDataContent<float>?)> With1MillionFloatValues() => this.ContentDeserializer.DeserializeAsync(TestWith1MillionFloatValues, HeaderWith2Axis);
}
