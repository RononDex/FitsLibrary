using System;
using System.IO;
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
            var testData = new byte[1568];
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
            var testData = new byte[3000];
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
            var testData = new byte[3000];
            testData = TestUtils.AddContentToArray(
                data: testData,
                index: 0,
                content: HeaderDeserializer.END_MARKER);
            var testStream = TestUtils.ByteArrayToStream(testData);
            var testee = new HeaderDeserializer();

            // Act
            var result = testee.Deserialize(testStream);

            // Assert
            result.Should().NotBe(null);
            result.Entries.Should().BeEmpty();
        }
    }
}
