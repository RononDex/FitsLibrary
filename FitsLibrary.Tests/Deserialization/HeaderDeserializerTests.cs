using System;
using System.IO;
using System.Linq;
using FitsLibrary.Deserialization;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.Desersialization
{
    public partial class HeaderTests
    {
        [Test]
        public void Deserialize_WithEmptyByteStream_ThrowsException()
        {
            // Arrange
            var testData = new byte[0];
            var testStream = TestUtils.ByteArrayToStream(testData);
            var testee = new HeaderDeserializer();

            // Act
            Action act = () => testee.Deserialize(testStream);

            // Assert
            act.Should().Throw<InvalidDataException>();
        }

        [Test]
        public void Deserialize_WithByteStreamOfSizeLessThan2880_ThrowsException()
        {
            // Arrange
            var testData = new byte[2879];
            var testStream = TestUtils.ByteArrayToStream(testData);
            var testee = new HeaderDeserializer();

            // Act
            Action act = () => testee.Deserialize(testStream);

            // Assert
            act.Should().Throw<InvalidDataException>();
        }

        [Test]
        public void Deserialize_WithByteStreamOfValidLengthButNoEndMark_ThrowsException()
        {
            // Arrange
            var testData = new byte[2881];
            var testStream = TestUtils.ByteArrayToStream(testData);
            var testee = new HeaderDeserializer();

            // Act
            Action act = () => testee.Deserialize(testStream);

            // Assert
            act.Should().Throw<InvalidDataException>();
        }

        [Test]
        public void Deserialize_WithByteStreamOfValidLengthAndEndMarkAtBeginning_ReturnsEmptyHeader()
        {
            // Arrange
            var testData = new byte[2881];
            testData = TestUtils.AddContentToArray(
                data: testData,
                startIndex: 0,
                content: HeaderDeserializer.END_MARKER);
            var testStream = TestUtils.ByteArrayToStream(testData);
            var testee = new HeaderDeserializer();

            // Act
            var result = testee.Deserialize(testStream);

            // Assert
            result.Should().NotBe(null);
            result.Entries.Should().BeEmpty();
        }

        [Test]
        public void Deserialize_WithOneHeaderEntry_ReturnsHeaderWithOneEntry()
        {
            // Arrange
            var testData = new byte[2881];
            testData = TestUtils.AddHeaderEntry(
                data: testData,
                startIndex: 0,
                key: "TEST",
                value: "some Test Value",
                comment: "some test comment");
            testData = TestUtils.AddContentToArray(
                data: testData,
                startIndex: 80,
                content: HeaderDeserializer.END_MARKER);
            var testStream = TestUtils.ByteArrayToStream(testData);
            var testee = new HeaderDeserializer();

            // Act
            var result = testee.Deserialize(testStream);

            // Assert
            result.Should().NotBe(null);
            result.Entries.Should().HaveCount(1);
            result.Entries.First().Key.Should().Be("TEST");
            result.Entries.First().Value.Should().Be("some Test Value");
            result.Entries.First().Comment.Should().Be("some test comment");
        }

        // TODO Add more tests for header parsing
    }
}
