using System;
using FitsLibrary.DocumentParts;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests
{
    public class FitsDocumentTests
    {
        [Test]
        public void GetInt32ValueAt_WithSpecificCoordinates_ReturnsCorrectValue()
        {
            var header = new HeaderBuilder()
                .WithNumberOfAxis(2)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .WithAxisOfSize(dimensionIndex: 2, size: 10)
                .WithContentDataType(DataContentType.INTEGER)
                .Build();
            Memory<object> contentData = new object[100];
            for (var i = 0; i < contentData.Length; i++)
            {
                contentData.Span[i] = i;
            }

            var testee = new FitsDocument(header, contentData);

            testee.GetInt32ValueAt(0, 0).Should().Be(0);
            testee.GetInt32ValueAt(0, 2).Should().Be(20);
            testee.GetInt32ValueAt(4, 5).Should().Be(54);
            testee.GetInt32ValueAt(6, 9).Should().Be(96);
        }

        [Test]
        public void GetInt16ValueAt_WithSpecificCoordinates_ReturnsCorrectValue()
        {
            var header = new HeaderBuilder()
                .WithNumberOfAxis(2)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .WithAxisOfSize(dimensionIndex: 2, size: 10)
                .WithContentDataType(DataContentType.SHORT)
                .Build();
            Memory<object> contentData = new object[100];
            for (var i = 0; i < contentData.Length; i++)
            {
                contentData.Span[i] = (short)i;
            }

            var testee = new FitsDocument(header, contentData);

            testee.GetInt16ValueAt(0, 0).Should().Be(0);
            testee.GetInt16ValueAt(0, 2).Should().Be(20);
            testee.GetInt16ValueAt(4, 5).Should().Be(54);
            testee.GetInt16ValueAt(6, 9).Should().Be(96);
        }

        [Test]
        public void GetInt64ValueAt_WithSpecificCoordinates_ReturnsCorrectValue()
        {
            var header = new HeaderBuilder()
                .WithNumberOfAxis(2)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .WithAxisOfSize(dimensionIndex: 2, size: 10)
                .WithContentDataType(DataContentType.LONG)
                .Build();
            Memory<object> contentData = new object[100];
            for (var i = 0; i < contentData.Length; i++)
            {
                contentData.Span[i] = (long)i;
            }

            var testee = new FitsDocument(header, contentData);

            testee.GetInt64ValueAt(0, 0).Should().Be(0);
            testee.GetInt64ValueAt(0, 2).Should().Be(20);
            testee.GetInt64ValueAt(4, 5).Should().Be(54);
            testee.GetInt64ValueAt(6, 9).Should().Be(96);
        }

        [Test]
        public void GetByteValueAt_WithSpecificCoordinates_ReturnsCorrectValue()
        {
            var header = new HeaderBuilder()
                .WithNumberOfAxis(2)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .WithAxisOfSize(dimensionIndex: 2, size: 10)
                .WithContentDataType(DataContentType.BYTE)
                .Build();
            Memory<object> contentData = new object[100];
            for (var i = 0; i < contentData.Length; i++)
            {
                contentData.Span[i] = (byte)i;
            }

            var testee = new FitsDocument(header, contentData);

            testee.GetByteValueAt(0, 0).Should().Be(0);
            testee.GetByteValueAt(0, 2).Should().Be(20);
            testee.GetByteValueAt(4, 5).Should().Be(54);
            testee.GetByteValueAt(6, 9).Should().Be(96);
        }

        [Test]
        public void GetFloat32ValueAt_WithSpecificCoordinates_ReturnsCorrectValue()
        {
            var header = new HeaderBuilder()
                .WithNumberOfAxis(2)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .WithAxisOfSize(dimensionIndex: 2, size: 10)
                .WithContentDataType(DataContentType.FLOAT)
                .Build();
            Memory<object> contentData = new object[100];
            for (var i = 0; i < contentData.Length; i++)
            {
                contentData.Span[i] = (float)i;
            }

            var testee = new FitsDocument(header, contentData);

            testee.GetFloat32ValueAt(0, 0).Should().Be(0);
            testee.GetFloat32ValueAt(0, 2).Should().Be(20);
            testee.GetFloat32ValueAt(4, 5).Should().Be(54);
            testee.GetFloat32ValueAt(6, 9).Should().Be(96);
        }

        [Test]
        public void GetFloat64ValueAt_WithSpecificCoordinates_ReturnsCorrectValue()
        {
            var header = new HeaderBuilder()
                .WithNumberOfAxis(2)
                .WithAxisOfSize(dimensionIndex: 1, size: 10)
                .WithAxisOfSize(dimensionIndex: 2, size: 10)
                .WithContentDataType(DataContentType.DOUBLE)
                .Build();
            Memory<object> contentData = new object[100];
            for (var i = 0; i < contentData.Length; i++)
            {
                contentData.Span[i] = (double)i;
            }

            var testee = new FitsDocument(header, contentData);

            testee.GetFloat64ValueAt(0, 0).Should().Be(0);
            testee.GetFloat64ValueAt(0, 2).Should().Be(20);
            testee.GetFloat64ValueAt(4, 5).Should().Be(54);
            testee.GetFloat64ValueAt(6, 9).Should().Be(96);
        }
    }
}
