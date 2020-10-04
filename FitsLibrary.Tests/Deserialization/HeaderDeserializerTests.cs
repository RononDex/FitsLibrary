using System;
using System.IO;
using System.Linq;
using FitsLibrary.Deserialization;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.Desersialization
{
    public class HeaderTests
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
                value: 1,
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
            result.Entries.First().Value.Should().Be(1);
            result.Entries.First().Comment.Should().Be("some test comment");
        }

        [Test]
        public void Deserialize_WithOneHeaderEntryHavingBooleanValue_ReturnsHeaderWithOneEntry()
        {
            // Arrange
            var testData = new byte[2881];
            testData = TestUtils.AddHeaderEntry(
                data: testData,
                startIndex: 0,
                key: "TEST",
                value: true,
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
            result.Entries.First().Value.Should().Be(true);
            result.Entries.First().Comment.Should().Be("some test comment");
        }

        [Test]
        public void Deserialize_WithAStringValue_ReturnsHeaderWithOneEntry()
        {
            // Arrange
            var testData = new byte[2881];
            testData = TestUtils.AddHeaderEntry(
                data: testData,
                startIndex: 0,
                key: "TEST",
                value: "'Some test value as string'",
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
            result.Entries.First().Value.Should().Be("Some test value as string");
            result.Entries.First().Comment.Should().Be("some test comment");
        }

        [Test]
        public void Deserialize_WithAFloatingPointValue_ReturnsHeaderWithOneEntry()
        {
            // Arrange
            var testData = new byte[2881];
            testData = TestUtils.AddHeaderEntry(
                data: testData,
                startIndex: 0,
                key: "TEST",
                value: 1.53,
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
            result.Entries.First().Value.Should().Be(1.53);
            result.Entries.First().Comment.Should().Be("some test comment");
        }

        [Test]
        public void Deserialize_WithOneHeaderEntryIntegerAndNoComment_ReturnsHeaderWithOneEntry()
        {
            // Arrange
            var testData = new byte[2881];
            testData = TestUtils.AddHeaderEntry(
                data: testData,
                startIndex: 0,
                key: "TEST",
                value: 1,
                comment: null);
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
            result.Entries.First().Value.Should().Be(1);
            result.Entries.First().Comment.Should().BeNull();
        }

        [Test]
        public void Deserialize_WithAStringValueButNoComment_ReturnsHeaderWithOneEntry()
        {
            // Arrange
            var testData = new byte[2881];
            testData = TestUtils.AddHeaderEntry(
                data: testData,
                startIndex: 0,
                key: "TEST",
                value: "'Some test value as string'",
                comment: null);
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
            result.Entries.First().Value.Should().Be("Some test value as string");
            result.Entries.First().Comment.Should().BeNull();
        }

        [Test]
        public void Deserialize_WithAFloatingPointValueButNoComment_ReturnsHeaderWithOneEntry()
        {
            // Arrange
            var testData = new byte[2881];
            testData = TestUtils.AddHeaderEntry(
                data: testData,
                startIndex: 0,
                key: "TEST",
                value: 1.53,
                comment: null);
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
            result.Entries.First().Value.Should().Be(1.53);
            result.Entries.First().Comment.Should().BeNull();
        }

        // TODO Add more tests for header parsing (error cases)
    }
}
