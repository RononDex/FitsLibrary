using System;
using System.Collections.Generic;
using System.IO;
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

            var deserilaizedContent = await deserializer.DeserializeAsync(dataStream, header);

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

            var deserilaizedContent = await deserializer.DeserializeAsync(dataStream, header);

            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Data.Should().HaveCount(10);
            Enumerable.Range(0, 10)
                .All(i =>
                        deserilaizedContent!.Data.Any(d => d.Coordinates[0] == Convert.ToUInt64(i)))
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
                .Build();

            var deserilaizedContent = await deserializer.DeserializeAsync(dataStream, header);

            deserilaizedContent.Should().NotBeNull();
            deserilaizedContent!.Data.Should().HaveCount(10);
            Enumerable.Range(0, 10)
                .All(i =>
                        deserilaizedContent!.Data.Any(d => d.Coordinates[0] == Convert.ToUInt64(i)))
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
                var sizePerValue = Convert.ToUInt64(Math.Abs((int)header.DataContentType));
                var byteIndex = coordinates
                    .Select(coordinate =>
                        sizePerValue *
                        coordinate.Value *
                        axisSizes
                        .Where((_, axisSizeIndex) => axisSizeIndex <= coordinate.Key - 1)
                        .Aggregate((ulong)1, (x, y) => x * y))
                    .Aggregate((ulong)0, (x, y) => x + y);

                var valueInBytes = GetValueInBytes(value, header);

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
