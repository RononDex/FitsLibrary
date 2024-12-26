using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FitsLibrary.Deserialization.Head;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.Desersialization;

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net70, launchCount: 5, warmupCount: 5, iterationCount: 5)]
[MemoryDiagnoser]
public class HeaderDeserializerTests
{
    [Test]
    public void Deserialize_WithEmptyByteStream_ThrowsException()
    {
        // Arrange
        var testData = Array.Empty<byte>();
        var testStream = TestUtils.ByteArrayToStream(testData);
        var testee = new HeaderDeserializer();

        // Act
        Func<Task> act = () => testee.DeserializeAsync(testStream);

        // Assert
        act.Should().ThrowAsync<InvalidDataException>();
    }

    [Test]
    public void Deserialize_WithByteStreamOfSizeLessThan2880_ThrowsException()
    {
        // Arrange
        var testData = new byte[2879];
        var testStream = TestUtils.ByteArrayToStream(testData);
        var testee = new HeaderDeserializer();

        // Act
        Func<Task> act = () => testee.DeserializeAsync(testStream);

        // Assert
        act.Should().ThrowAsync<InvalidDataException>();
    }

    [Test]
    public void Deserialize_WithByteStreamOfValidLengthButNoEndMark_ThrowsException()
    {
        // Arrange
        var testData = new byte[2881];
        var testStream = TestUtils.ByteArrayToStream(testData);
        var testee = new HeaderDeserializer();

        // Act
        Func<Task> act = () => testee.DeserializeAsync(testStream);

        // Assert
        act.Should().ThrowAsync<InvalidDataException>();
    }

    [Test]
    public async Task Deserialize_WithByteStreamOfValidLengthAndEndMarkAtBeginning_ReturnsEmptyHeaderAsync()
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
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().BeEmpty();
    }

    [Test]
    public async Task Deserialize_WithOneHeaderEntry_ReturnsHeaderWithOneEntryAsync()
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
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().HaveCount(1);
        parsedHeader!.Entries.First().Key.Should().Be("TEST");
        parsedHeader!.Entries.First().Value.Should().Be(1);
        parsedHeader!.Entries.First().Comment.Should().Be("some test comment");
    }

    [Test]
    public async Task Deserialize_WithOneHeaderEntryHavingBooleanValue_ReturnsHeaderWithOneEntryAsync()
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
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().HaveCount(1);
        parsedHeader!.Entries.First().Key.Should().Be("TEST");
        parsedHeader!.Entries.First().Value.Should().Be(true);
        parsedHeader!.Entries.First().Comment.Should().Be("some test comment");
    }

    [Test]
    public async Task Deserialize_WithAStringValue_ReturnsHeaderWithOneEntryAsync()
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
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().HaveCount(1);
        parsedHeader!.Entries.First().Key.Should().Be("TEST");
        parsedHeader!.Entries.First().Value.Should().Be("Some test value as string");
        parsedHeader!.Entries.First().Comment.Should().Be("some test comment");
    }

    [Test]
    public async Task Deserialize_WithAFloatingPointValue_ReturnsHeaderWithOneEntryAsync()
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
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().HaveCount(1);
        parsedHeader!.Entries.First().Key.Should().Be("TEST");
        parsedHeader!.Entries.First().Value.Should().Be(1.53);
        parsedHeader!.Entries.First().Comment.Should().Be("some test comment");
    }

    [Test]
    public async Task Deserialize_WithOneHeaderEntryIntegerAndNoComment_ReturnsHeaderWithOneEntryAsync()
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
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().HaveCount(1);
        parsedHeader!.Entries.First().Key.Should().Be("TEST");
        parsedHeader!.Entries.First().Value.Should().Be(1);
        parsedHeader!.Entries.First().Comment.Should().BeNull();
    }

    [Test]
    public async Task Deserialize_WithAStringValueButNoComment_ReturnsHeaderWithOneEntryAsync()
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
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().HaveCount(1);
        parsedHeader!.Entries.First().Key.Should().Be("TEST");
        parsedHeader!.Entries.First().Value.Should().Be("Some test value as string");
        parsedHeader!.Entries.First().Comment.Should().BeNull();
    }

    [Test]
    public async Task Deserialize_WithAFloatingPointValueButNoComment_ReturnsHeaderWithOneEntryAsync()
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
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().HaveCount(1);
        parsedHeader!.Entries.First().Key.Should().Be("TEST");
        parsedHeader!.Entries.First().Value.Should().Be(1.53);
        parsedHeader!.Entries.First().Comment.Should().BeNull();
    }

    [Test]
    public async Task Deserilaize_WithMultiKeywordValue_ReturnsConcattedValueAsync()
    {
        // Arrange
        var testData = new byte[2881];
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 0,
            key: "TEST",
            value: "'Some very looooong test value&'",
            comment: null);
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 80,
            key: "CONTINUE",
            value: "' AND some moooore from previous value'",
            comment: null);
        testData = TestUtils.AddContentToArray(
            data: testData,
            startIndex: 160,
            content: HeaderDeserializer.END_MARKER);
        var testStream = TestUtils.ByteArrayToStream(testData);
        var testee = new HeaderDeserializer();

        // Act
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().HaveCount(1);
        parsedHeader!.Entries.First().Key.Should().Be("TEST");
        parsedHeader!.Entries.First().Value.Should().Be("Some very looooong test value AND some moooore from previous value");
        parsedHeader!.Entries.First().Comment.Should().BeNull();
    }

    [Test]
    public async Task Deserilaize_WithMultiKeywordComment_ReturnsConcattedValueAsync()
    {
        // Arrange
        var testData = new byte[2881];
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 0,
            key: "TEST",
            value: "'&'",
            comment: "Test some very looong comment");
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 80,
            key: "CONTINUE",
            value: "'&'",
            comment: " continuation of some very long comment");
        testData = TestUtils.AddContentToArray(
            data: testData,
            startIndex: 160,
            content: HeaderDeserializer.END_MARKER);
        var testStream = TestUtils.ByteArrayToStream(testData);
        var testee = new HeaderDeserializer();

        // Act
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().HaveCount(1);
        parsedHeader!.Entries.First().Key.Should().Be("TEST");
        parsedHeader!.Entries.First().Value.Should().Be(string.Empty);
        parsedHeader!.Entries.First().Comment.Should().Be("Test some very looong comment continuation of some very long comment");
    }

    [Test]
    public async Task Deserilaize_WithMultiKeywordValueOf3HeaderChunks_ReturnsCombinedValueAsync()
    {
        // Arrange
        var testData = new byte[2881];
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 0,
            key: "TEST",
            value: "'Some test value &'",
            comment: "Test some very looong comment");
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 80,
            key: "CONTINUE",
            value: "'Spanning over 3 values &'",
            comment: " continuation of some very long comment");
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 160,
            key: "CONTINUE",
            value: "'TEST TEST TESTING TEST'",
            comment: null);
        testData = TestUtils.AddContentToArray(
            data: testData,
            startIndex: 240,
            content: HeaderDeserializer.END_MARKER);
        var testStream = TestUtils.ByteArrayToStream(testData);
        var testee = new HeaderDeserializer();

        // Act
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().HaveCount(1);
        parsedHeader!.Entries.First().Key.Should().Be("TEST");
        parsedHeader!.Entries.First().Value.Should().Be("Some test value Spanning over 3 values TEST TEST TESTING TEST");
        parsedHeader!.Entries.First().Comment.Should().Be("Test some very looong comment continuation of some very long comment");
    }

    [Test]
    public void Deserilaize_WithMultiKeywordValueButMissingContinuation_ThrowsException()
    {
        // Arrange
        var testData = new byte[2881];
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 0,
            key: "TEST",
            value: "'Some test value &'",
            comment: "Test some very looong comment");
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 80,
            key: "TEST2",
            value: "'Spanning over 3 values'",
            comment: " continuation of some very long comment");
        testData = TestUtils.AddContentToArray(
            data: testData,
            startIndex: 160,
            content: HeaderDeserializer.END_MARKER);
        var testStream = TestUtils.ByteArrayToStream(testData);
        var testee = new HeaderDeserializer();

        // Act
        Func<Task> act = () => testee.DeserializeAsync(testStream);

        // Assert
        act.Should().ThrowAsync<InvalidDataException>();
    }

    [Test]
    public async Task Deserilaize_WithHistoryValues_ParsesHistoryValues()
    {
        // Arrange
        var testData = new byte[2881];
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 0,
            key: "HISTORY",
            value: "Some History entry",
            comment: null);
        testData = TestUtils.AddContentToArray(
            data: testData,
            startIndex: 80,
            content: HeaderDeserializer.END_MARKER);
        var testStream = TestUtils.ByteArrayToStream(testData);
        var testee = new HeaderDeserializer();

        // Act
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.First().Key.Should().Be("HISTORY");
        parsedHeader!.Entries.First().Value.Should().Be("Some History entry");
    }

    [Test]
    public async Task Deserilaize_WithCommentValues_ParsesHistoryValues()
    {
        // Arrange
        var testData = new byte[2881];
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 0,
            key: "COMMENT",
            value: "Some comment entry",
            comment: null);
        testData = TestUtils.AddContentToArray(
            data: testData,
            startIndex: 80,
            content: HeaderDeserializer.END_MARKER);
        var testStream = TestUtils.ByteArrayToStream(testData);
        var testee = new HeaderDeserializer();

        // Act
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.First().Key.Should().Be("COMMENT");
        parsedHeader!.Entries.First().Value.Should().Be("Some comment entry");
    }

    [Test]
    public async Task Deserialize_WithAFloatingPointValueAsString_ReturnsHeaderWithOneEntryAsync()
    {
        // Arrange
        var testData = new byte[2881];
        testData = TestUtils.AddHeaderEntry(
            data: testData,
            startIndex: 0,
            key: "TEST",
            value: "1.53",
            comment: "some test comment");
        testData = TestUtils.AddContentToArray(
            data: testData,
            startIndex: 80,
            content: HeaderDeserializer.END_MARKER);
        var testStream = TestUtils.ByteArrayToStream(testData);
        var testee = new HeaderDeserializer();

        // Act
        var result = await testee.DeserializeAsync(testStream);
        var parsedHeader = result.header;

        // Assert
        result.Should().NotBeNull();
        parsedHeader.Should().NotBeNull();
        parsedHeader!.Entries.Should().HaveCount(1);
        parsedHeader!.Entries.First().Key.Should().Be("TEST");
        parsedHeader!.Entries.First().Value.Should().Be(1.53);
        parsedHeader!.Entries.First().Comment.Should().Be("some test comment");
    }

    // TODO Add mote tests for header parsing (error cases)
}
