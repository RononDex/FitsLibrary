using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using FitsLibrary.Deserialization;
using FitsLibrary.DocumentParts;
using FitsLibrary.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.Desersialization
{
    public class ContentDeserializerTests
    {
        [Test]
        public async Task DeserializeAsync_WithNoContent_ReturnsNullAsync()
        {
            var deserializer = new ContentDeserializer();
            var header = new HeaderBuilder()
                .WithNumberOfAxis(0)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithEmptyContent()
                .Build();

            var deserilaizedContent = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);

            deserilaizedContent.Should().BeNull();
        }

        [Test]
        public async Task DeserializeAsync_WithOneAxisAndSomeValues_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.INTEGER)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith(123, header)
                .Build();

            var deserilaizedContent = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);

            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Data.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Data.ToArray().All(d => (int)d.Value == 123).Should().BeTrue();
            Enumerable.Range(0, 10)
                .All(i => deserilaizedContent!.Data.ToArray().Any(d => d.Coordinates[0] == Convert.ToUInt64(i)))
                .Should()
                .BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithDataOfTypeShort_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.SHORT)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith((short)123, header)
                .Build();

            var deserilaizedContent = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);

            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Data.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Data.ToArray().All(d => (short)d.Value == 123).Should().BeTrue();
            Enumerable.Range(0, 10)
                .All(i => deserilaizedContent!.Data.ToArray().Any(d => d.Coordinates[0] == Convert.ToUInt64(i)))
                .Should()
                .BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithDataOfTypeByte_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.BYTE)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith((byte)123, header)
                .Build();

            var deserilaizedContent = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);

            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Data.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Data.ToArray().All(d => (byte)d.Value == 123).Should().BeTrue();
            Enumerable.Range(0, 10)
                .All(i => deserilaizedContent!.Data.ToArray().Any(d => d.Coordinates[0] == Convert.ToUInt64(i)))
                .Should()
                .BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithDataOfTypeDouble_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.DOUBLE)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith((double)123, header)
                .Build();

            var deserilaizedContent = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);

            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Data.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Data.ToArray().All(d => (double)d.Value == 123).Should().BeTrue();
            Enumerable.Range(0, 10)
                .All(i => deserilaizedContent!.Data.ToArray().Any(d => d.Coordinates[0] == Convert.ToUInt64(i)))
                .Should()
                .BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithDataOfTypeFloat_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.FLOAT)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith((float)123, header)
                .Build();

            var deserilaizedContent = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);

            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Data.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Data.ToArray().All(d => (float)d.Value == 123).Should().BeTrue();
            Enumerable.Range(0, 10)
                .All(i => deserilaizedContent!.Data.ToArray().Any(d => d.Coordinates[0] == Convert.ToUInt64(i)))
                .Should()
                .BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithDataOfTypeLong_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.LONG)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith((long)123, header)
                .Build();

            var deserilaizedContent = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);

            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Data.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Data.ToArray().All(d => (long)d.Value == 123).Should().BeTrue();
            Enumerable.Range(0, 10)
                .All(i => deserilaizedContent!.Data.ToArray().Any(d => d.Coordinates[0] == Convert.ToUInt64(i)))
                .Should()
                .BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithOneAxisAndSomeValuesAndOneValueDifferent_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.INTEGER)
                .WithNumberOfAxis(1)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith(123, header)
                .WithDataAtCoordinates(
                        10,
                        new Dictionary<uint, ulong> { { 0, 5 } },
                        header)
                .Build();

            var deserilaizedContent = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);

            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Data.ToArray().Should().HaveCount(10);
            deserilaizedContent!.Data.ToArray().Single(d => d.Coordinates[0] == 5).Value.Should().Equals(10);
            deserilaizedContent!.Data.ToArray().Where(d => d.Coordinates[0] != 5).All(d => (int)d.Value == 123).Should().BeTrue();
            Enumerable.Range(0, 10)
                .All(i => deserilaizedContent!.Data.ToArray().Any(d => d.Coordinates[0] == Convert.ToUInt64(i)))
                .Should()
                .BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithTwoAxisAndDefaultValues_ReturnsAllValuesAsync()
        {
            var deserializer = new ContentDeserializer();
            var header = new HeaderBuilder()
                .WithContentDataType(DataContentType.INTEGER)
                .WithNumberOfAxis(2)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .WithAxisOfSize(dimensionIndex: 2, size: 20)
                .Build();
            var dataStream = new ContentStreamBuilder()
                .WithDataBeingInitializedWith(123, header)
                .Build();

            var deserilaizedContent = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);

            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Data.ToArray().Should().HaveCount(10 * 20);
            deserilaizedContent!.Data.ToArray().All(d => (int)d.Value == 123).Should().BeTrue();
            Enumerable.Range(0, 10)
                .All(i => deserilaizedContent!.Data.ToArray().Any(d => d.Coordinates[0] == Convert.ToUInt64(i)))
                .Should()
                .BeTrue();
            Enumerable.Range(0, 20)
                .All(i => deserilaizedContent!.Data.ToArray().Any(d => d.Coordinates[1] == Convert.ToUInt64(i)))
                .Should()
                .BeTrue();
        }

        [Test]
        public async Task DeserializeAsync_WithTwoAxisAndSomeValuesAndOneValueDifferent_ReturnsTheValuesAsync()
        {
            var deserializer = new ContentDeserializer();
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

            var deserilaizedContent = await deserializer.DeserializeAsync(PipeReader.Create(dataStream), header);

            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Data.ToArray().Should().HaveCount(10 * 20);
            deserilaizedContent!.Data.ToArray().Single(d => d.Coordinates[0] == 5 && d.Coordinates[1] == 2).Value.Should().Equals(10);
            deserilaizedContent!.Data.ToArray().Where(d => d.Coordinates[0] != 5 && d.Coordinates[1] != 2).All(d => (int)d.Value == 123).Should().BeTrue();
            Enumerable.Range(0, 10)
                .All(i => deserilaizedContent!.Data.ToArray().Any(d => d.Coordinates[0] == Convert.ToUInt64(i)))
                .Should()
                .BeTrue();
            Enumerable.Range(0, 20)
                .All(i => deserilaizedContent!.Data.ToArray().Any(d => d.Coordinates[1] == Convert.ToUInt64(i)))
                .Should()
                .BeTrue();
        }

        private class ContentStreamBuilder
        {
            private readonly List<byte> data = new();

            public MemoryStream Build()
            {
                return new MemoryStream(data.ToArray());
            }

            public ContentStreamBuilder WithEmptyContent()
            {
                data.Clear();
                return this;
            }

            public ContentStreamBuilder WithDataBeingInitializedWith(object defaultData, Header header)
            {
                var numberOfAxis = (int?)header["NAXIS"];
                var axisSizes = Enumerable.Range(1, numberOfAxis!.Value)
                    .Select(axisIndex => header[$"NAXIS{axisIndex}"] as long?);
                var totalNumberOfValues = axisSizes.Aggregate((long?)1, (x, y) => x!.Value * y!.Value);

                var valueInBytes = GetValueInBytes(defaultData, header);
                data.AddRange(Enumerable.Repeat(
                    valueInBytes,
                    Convert.ToInt32(totalNumberOfValues!.Value)).SelectMany(_ => _).ToArray());

                return this;
            }

            public ContentStreamBuilder WithDataAtCoordinates(object value, Dictionary<uint, ulong> coordinates, Header header)
            {
                var numberOfAxis = (int?)header["NAXIS"];
                var axisSizes = Enumerable.Range(1, numberOfAxis!.Value)
                    .Select(axisIndex => Convert.ToUInt64(header[$"NAXIS{axisIndex}"] as long?));
                var sizePerValue = Convert.ToUInt64(Math.Abs((int)header.DataContentType)) / 8;
                var byteIndex = coordinates
                    .Select(coordinate =>
                        sizePerValue *
                        coordinate.Value *
                        axisSizes
                        .Where((_, axisSizeIndex) => axisSizeIndex <= Convert.ToInt32(coordinate.Key) - 1)
                        .Aggregate(Convert.ToUInt64(1), (x, y) => x * y))
                    .Aggregate(Convert.ToUInt64(0), (x, y) => x + y);

                var valueInBytes = GetValueInBytes(value, header);

                for (var i = 0; i < valueInBytes.Length; i++)
                {
                    data[Convert.ToInt32(byteIndex) + i] = valueInBytes[i];
                }

                return this;
            }

            private static byte[] GetValueInBytes(object value, Header header)
            {
                return header.DataContentType switch
                {
                    DataContentType.BYTE => new[] { (byte)value },
                    DataContentType.SHORT => BitConverter.GetBytes((short)value).ConvertLittleEndianToBigEndianIfNecessary(),
                    DataContentType.INTEGER => BitConverter.GetBytes((int)value).ConvertLittleEndianToBigEndianIfNecessary(),
                    DataContentType.LONG => BitConverter.GetBytes((long)value).ConvertLittleEndianToBigEndianIfNecessary(),
                    DataContentType.FLOAT => BitConverter.GetBytes((float)value).ConvertLittleEndianToBigEndianIfNecessary(),
                    DataContentType.DOUBLE => BitConverter.GetBytes((double)value).ConvertLittleEndianToBigEndianIfNecessary(),
                    _ => throw new Exception("Unexpected Case"),
                };
            }
        }
    }
}
