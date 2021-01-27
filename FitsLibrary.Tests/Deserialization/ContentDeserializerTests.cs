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
            deserilaizedContent!.Data.All(val => (int)val.Value == 123).Should().BeTrue();
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

                switch (header.DataContentType)
                {
                    case DataContentType.BYTE:
                        data.AddRange(
                                Enumerable.Repeat(
                                    (byte)defaultData,
                                    Convert.ToInt32(totalNumberOfValues!.Value)));
                        break;
                    case DataContentType.SHORT:
                        data.AddRange(
                                Enumerable.Repeat(
                                    BitConverter.GetBytes((short)defaultData).ConvertLittleEndianToBigEndianIfNecessary(),
                                    Convert.ToInt32(totalNumberOfValues!.Value))
                                .SelectMany(arr => arr).ToArray());
                        break;
                    case DataContentType.INTEGER:
                        data.AddRange(
                                Enumerable.Repeat(
                                    BitConverter.GetBytes((int)defaultData).ConvertLittleEndianToBigEndianIfNecessary(),
                                    Convert.ToInt32(totalNumberOfValues!.Value))
                                .SelectMany(arr => arr).ToArray());
                        break;
                    case DataContentType.LONG:
                        data.AddRange(
                                Enumerable.Repeat(
                                    BitConverter.GetBytes((long)defaultData).ConvertLittleEndianToBigEndianIfNecessary(),
                                    Convert.ToInt32(totalNumberOfValues!.Value))
                                .SelectMany(arr => arr).ToArray());
                        break;
                    case DataContentType.FLOAT:
                        data.AddRange(
                                Enumerable.Repeat(
                                    BitConverter.GetBytes((float)defaultData).ConvertLittleEndianToBigEndianIfNecessary(),
                                    Convert.ToInt32(totalNumberOfValues!.Value))
                                .SelectMany(arr => arr).ToArray());
                        break;
                    case DataContentType.DOUBLE:
                        data.AddRange(
                                Enumerable.Repeat(
                                    BitConverter.GetBytes((double)defaultData).ConvertLittleEndianToBigEndianIfNecessary(),
                                    Convert.ToInt32(totalNumberOfValues!.Value))
                                .SelectMany(arr => arr).ToArray());
                        break;

                    default:
                        throw new Exception("Unexpected Case");
                }

                return this;
            }
        }
    }
}
