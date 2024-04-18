using System;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;
using FluentAssertions;
using NUnit.Framework;

namespace FitsLibrary.Tests.DocumentParts;

public class ImageHeaderDataUnitTests
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
        Memory<int> contentData = new int[100];
        for (var i = 0; i < contentData.Length; i++)
        {
            contentData.Span[i] = i;
        }

        var testee = new ImageHeaderDataUnit<int>(HeaderDataUnitType.PRIMARY, header, new ImageDataContent<int>([10, 10], contentData));

        testee.Data.GetValueAt(0, 0).Should().Be(0);
        testee.Data.GetValueAt(0, 2).Should().Be(20);
        testee.Data.GetValueAt(4, 5).Should().Be(54);
        testee.Data.GetValueAt(6, 9).Should().Be(96);
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
        Memory<short> contentData = new short[100];
        for (var i = 0; i < contentData.Length; i++)
        {
            contentData.Span[i] = (short)i;
        }

        var testee = new ImageHeaderDataUnit<short>(HeaderDataUnitType.PRIMARY, header, new ImageDataContent<short>([10, 10], contentData));

        testee.Data.GetValueAt(0, 0).Should().Be(0);
        testee.Data.GetValueAt(0, 2).Should().Be(20);
        testee.Data.GetValueAt(4, 5).Should().Be(54);
        testee.Data.GetValueAt(6, 9).Should().Be(96);
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
        Memory<long> contentData = new long[100];
        for (var i = 0; i < contentData.Length; i++)
        {
            contentData.Span[i] = (long)i;
        }

        var testee = new ImageHeaderDataUnit<long>(HeaderDataUnitType.PRIMARY, header, new ImageDataContent<long>([10, 10], contentData));

        testee.Data.GetValueAt(0, 0).Should().Be(0);
        testee.Data.GetValueAt(0, 2).Should().Be(20);
        testee.Data.GetValueAt(4, 5).Should().Be(54);
        testee.Data.GetValueAt(6, 9).Should().Be(96);
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
        Memory<byte> contentData = new byte[100];
        for (var i = 0; i < contentData.Length; i++)
        {
            contentData.Span[i] = (byte)i;
        }

        var testee = new ImageHeaderDataUnit<byte>(HeaderDataUnitType.PRIMARY, header, new ImageDataContent<byte>([10, 10], contentData));

        testee.Data.GetValueAt(0, 0).Should().Be(0);
        testee.Data.GetValueAt(0, 2).Should().Be(20);
        testee.Data.GetValueAt(4, 5).Should().Be(54);
        testee.Data.GetValueAt(6, 9).Should().Be(96);
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
        Memory<float> contentData = new float[100];
        for (var i = 0; i < contentData.Length; i++)
        {
            contentData.Span[i] = (float)i;
        }

        var testee = new ImageHeaderDataUnit<float>(HeaderDataUnitType.PRIMARY, header, new ImageDataContent<float>([10, 10], contentData));

        testee.Data.GetValueAt(0, 0).Should().Be(0);
        testee.Data.GetValueAt(0, 2).Should().Be(20);
        testee.Data.GetValueAt(4, 5).Should().Be(54);
        testee.Data.GetValueAt(6, 9).Should().Be(96);
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
        Memory<double> contentData = new double[100];
        for (var i = 0; i < contentData.Length; i++)
        {
            contentData.Span[i] = (double)i;
        }

        var testee = new ImageHeaderDataUnit<double>(HeaderDataUnitType.PRIMARY, header, new ImageDataContent<double>([10, 10], contentData));

        testee.Data.GetValueAt(0, 0).Should().Be(0);
        testee.Data.GetValueAt(0, 2).Should().Be(20);
        testee.Data.GetValueAt(4, 5).Should().Be(54);
        testee.Data.GetValueAt(6, 9).Should().Be(96);
    }
}
