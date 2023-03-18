using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using FitsLibrary.Deserialization;
using FitsLibrary.DocumentParts;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.Desersialization
{
    public class ContentDeserializerTests
    {
        [Test]
        public async Task DeserializeAsync_WithNoContent_ReturnsNullAsync()
        {
            var deserializer = new ContentDeserializer<float>();
            var header = new HeaderBuilder()
                .WithNumberOfAxis(0)
                .WithContentDataType(DataContentType.FLOAT)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithEmptyContent()
                .Build();

            var deserilaizedContent = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);

            deserilaizedContent.endOfStreamReached.Should().BeFalse();
            deserilaizedContent.contentData.Should().BeNull();
        }

        [Test]
        public async Task DeserializeAsync_WithOneAxisAndSomeValues_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer<int>();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.INTEGER)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith(123, header)
                .Build();

            var deserilaizedContentResult = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);
            var deserilaizedContent = deserilaizedContentResult.contentData;

            deserilaizedContentResult.endOfStreamReached.Should().BeTrue();
            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Value.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Value.ToArray().All(d => (int)d == 123).Should().BeTrue();
        }
        [Test]
        public async Task DeserializeAsync_WithHavingAdditionalData_ReturnsEndOfStreamReachedFalse()
        {
            var deserializer = new ContentDeserializer<int>();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.INTEGER)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith(123, header)
                .WithAdditionalDataAfterContent()
                .Build();

            var deserilaizedContentResult = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);
            var deserilaizedContent = deserilaizedContentResult.contentData;

            deserilaizedContentResult.endOfStreamReached.Should().BeFalse();
            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Value.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Value.ToArray().All(d => (int)d == 123).Should().BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithDataOfTypeShort_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer<short>();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.SHORT)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith((short)123, header)
                .Build();

            var deserilaizedContentResult = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);
            var deserilaizedContent = deserilaizedContentResult.contentData;

            deserilaizedContentResult.endOfStreamReached.Should().BeTrue();
            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Value.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Value.ToArray().All(d => (short)d == 123).Should().BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithDataOfTypeByte_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer<byte>();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.BYTE)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith((byte)123, header)
                .Build();

            var deserilaizedContentResult = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);
            var deserilaizedContent = deserilaizedContentResult.contentData;

            deserilaizedContentResult.endOfStreamReached.Should().BeTrue();
            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Value.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Value.ToArray().All(d => (byte)d == 123).Should().BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithDataOfTypeDouble_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer<double>();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.DOUBLE)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith((double)123, header)
                .Build();

            var deserilaizedContentResult = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);
            var deserilaizedContent = deserilaizedContentResult.contentData;

            deserilaizedContentResult.endOfStreamReached.Should().BeTrue();
            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Value.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Value.ToArray().All(d => (double)d == 123).Should().BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithDataOfTypeFloat_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer<float>();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.FLOAT)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith((float)123, header)
                .Build();

            var deserilaizedContentResult = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);
            var deserilaizedContent = deserilaizedContentResult.contentData;

            deserilaizedContentResult.endOfStreamReached.Should().BeTrue();
            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Value.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Value.ToArray().All(d => (float)d == 123).Should().BeTrue();

        }

        [Test]
        public async Task DeserializeAsync_WithDataOfTypeLong_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer<long>();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.LONG)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith((long)123, header)
                .Build();

            var deserilaizedContentResult = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);
            var deserilaizedContent = deserilaizedContentResult.contentData;

            deserilaizedContentResult.endOfStreamReached.Should().BeTrue();
            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Value.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Value.ToArray().All(d => (long)d == 123).Should().BeTrue();

        }

        [Test]
        public async Task DeserializeAsync_WithOneAxisAndSomeValuesAndOneValueDifferent_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer<int>();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.INTEGER)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith(123, header)
                .WithDataAtCoordinates(
                        10,
                        new Dictionary<uint, ulong> { { 0, 4 } },
                        header)
                .Build();

            var deserilaizedContentResult = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);
            var deserilaizedContent = deserilaizedContentResult.contentData;

            deserilaizedContentResult.endOfStreamReached.Should().BeTrue();
            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Value.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Value.Span[4].Should().Be(10);
            deserilaizedContent!.Value.ToArray().Select((d, i) => (i, d)).Where(d => d.i != 4).All(d => (int)d.d == 123).Should().BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithTwoAxisAndDefaultValues_ReturnsAllValuesAsync()
        {
            var deserializer = new ContentDeserializer<int>();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.INTEGER)
                .WithNumberOfAxis(2)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .WithAxisOfSize(dimensionIndex: 2, size: 20)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith(123, header)
                .Build();

            var deserilaizedContentResult = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);
            var deserilaizedContent = deserilaizedContentResult.contentData;

            deserilaizedContentResult.endOfStreamReached.Should().BeTrue();
            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Value.ToArray().Should().HaveCount(10 * 20);
            deserilaizedContent!.Value.ToArray().All(d => (int)d == 123).Should().BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithTwoAxisAndSomeValuesAndOneValueDifferent_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer<int>();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.INTEGER)
                .WithNumberOfAxis(2)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .WithAxisOfSize(dimensionIndex: 2, size: 20)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith(123, header)
                .WithDataAtCoordinates(
                        10,
                        new Dictionary<uint, ulong>
                        {
                            { 0, 5 },
                            { 1, 2 },
                        },
                        header)
                .Build();

            var deserilaizedContentResult = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);
            var deserilaizedContent = deserilaizedContentResult.contentData;

            deserilaizedContentResult.endOfStreamReached.Should().BeTrue();
            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Value
                .ToArray()
                .Should()
                .HaveCount(10 * 20);
            deserilaizedContent!.Value
                .ToArray()
                .Select((d, i) => (i, d))
                .Single(d => d.i == 5 + (2 * 10))
                .d
                .Should()
                .Be(10);
            deserilaizedContent!.Value
                .ToArray()
                .Select((d, i) => (i, d))
                .Where(d => d.i != 5 + (2 * 10))
                .All(d => (int)d.d == 123)
                .Should()
                .BeTrue();
        }
    }
}
