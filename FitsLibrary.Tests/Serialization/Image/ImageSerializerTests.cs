
using System.Buffers.Binary;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.Serialization.Image;
using NUnit.Framework;

namespace FitsLibrary.Tests.Serialization.Image;

public class ImageSerializerTests
{
    [Test]
    public async Task Serialize_With1ByteEntryOnly_WritesBlockAndHasValueInStream()
    {
        using var memory = new MemoryStream();
        var testee = new ImageSerializer<byte>();
        var content = new ImageDataContent<byte>([1], new System.Memory<byte>([1]));
        var header = new HeaderBuilder().WithContentDataType(DataContentType.BYTE).Build();

        await testee.SerializeAsync(PipeWriter.Create(memory), content, header);
        var actual = memory.ToArray();

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0], Is.EqualTo((byte)1));
        Assert.That(actual[1..].All(b => b == 0), Is.True);
    }

    [Test]
    public async Task Serialize_With2DimensionalData_WritesBlockAndHasValueInStream()
    {
        using var memory = new MemoryStream();
        var testee = new ImageSerializer<byte>();
        var content = new ImageDataContent<byte>([2, 2], new System.Memory<byte>([1, 2, 3, 4]));
        var header = new HeaderBuilder().WithContentDataType(DataContentType.BYTE).Build();

        await testee.SerializeAsync(PipeWriter.Create(memory), content, header);
        var actual = memory.ToArray();

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0], Is.EqualTo((byte)1));
        Assert.That(actual[1], Is.EqualTo((byte)2));
        Assert.That(actual[2], Is.EqualTo((byte)3));
        Assert.That(actual[3], Is.EqualTo((byte)4));
        Assert.That(actual[4..].All(b => b == 0), Is.True);
    }

    [Test]
    public async Task Serialize_With3DimensionalData_WritesBlockAndHasValueInStream()
    {
        using var memory = new MemoryStream();
        var testee = new ImageSerializer<byte>();
        var content = new ImageDataContent<byte>([2, 2, 2], new System.Memory<byte>([1, 2, 3, 4, 5, 6, 7, 8]));
        var header = new HeaderBuilder().WithContentDataType(DataContentType.BYTE).Build();

        await testee.SerializeAsync(PipeWriter.Create(memory), content, header);
        var actual = memory.ToArray();

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(actual[0], Is.EqualTo((byte)1));
        Assert.That(actual[1], Is.EqualTo((byte)2));
        Assert.That(actual[2], Is.EqualTo((byte)3));
        Assert.That(actual[3], Is.EqualTo((byte)4));
        Assert.That(actual[4], Is.EqualTo((byte)5));
        Assert.That(actual[5], Is.EqualTo((byte)6));
        Assert.That(actual[6], Is.EqualTo((byte)7));
        Assert.That(actual[7], Is.EqualTo((byte)8));
        Assert.That(actual[8..].All(b => b == 0), Is.True);
    }

    [Test]
    public async Task Serialize_WithInt32Data_CorrectlySerializesDataStream()
    {
        using var memory = new MemoryStream();
        var testee = new ImageSerializer<int>();
        var content = new ImageDataContent<int>([2], new System.Memory<int>([1, 2]));
        var header = new HeaderBuilder().WithContentDataType(DataContentType.INT32).Build();

        await testee.SerializeAsync(PipeWriter.Create(memory), content, header);
        var actual = memory.ToArray();

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(BinaryPrimitives.ReadInt32BigEndian(actual[0..4]), Is.EqualTo(1));
        Assert.That(BinaryPrimitives.ReadInt32BigEndian(actual[4..8]), Is.EqualTo(2));
        Assert.That(actual[8..].All(b => b == 0), Is.True);
    }

    [Test]
    public async Task Serialize_WithInt16Data_CorrectlySerializesDataStream()
    {
        using var memory = new MemoryStream();
        var testee = new ImageSerializer<short>();
        var content = new ImageDataContent<short>([2], new System.Memory<short>([1, 2]));
        var header = new HeaderBuilder().WithContentDataType(DataContentType.INT16).Build();

        await testee.SerializeAsync(PipeWriter.Create(memory), content, header);
        var actual = memory.ToArray();

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(BinaryPrimitives.ReadInt16BigEndian(actual[0..2]), Is.EqualTo(1));
        Assert.That(BinaryPrimitives.ReadInt16BigEndian(actual[2..4]), Is.EqualTo(2));
        Assert.That(actual[4..].All(b => b == 0), Is.True);
    }

    [Test]
    public async Task Serialize_WithInt64Data_CorrectlySerializesDataStream()
    {
        using var memory = new MemoryStream();
        var testee = new ImageSerializer<long>();
        var content = new ImageDataContent<long>([2], new System.Memory<long>([1, 2]));
        var header = new HeaderBuilder().WithContentDataType(DataContentType.INT64).Build();

        await testee.SerializeAsync(PipeWriter.Create(memory), content, header);
        var actual = memory.ToArray();

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(BinaryPrimitives.ReadInt64BigEndian(actual[0..8]), Is.EqualTo(1));
        Assert.That(BinaryPrimitives.ReadInt64BigEndian(actual[8..16]), Is.EqualTo(2));
        Assert.That(actual[16..].All(b => b == 0), Is.True);
    }

    [Test]
    public async Task Serialize_WithFloatData_CorrectlySerializesDataStream()
    {
        using var memory = new MemoryStream();
        var testee = new ImageSerializer<float>();
        var content = new ImageDataContent<float>([2], new System.Memory<float>([1.25f, 2.25f]));
        var header = new HeaderBuilder().WithContentDataType(DataContentType.FLOAT).Build();

        await testee.SerializeAsync(PipeWriter.Create(memory), content, header);
        var actual = memory.ToArray();

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(BinaryPrimitives.ReadSingleBigEndian(actual[0..4]), Is.EqualTo(1.25f));
        Assert.That(BinaryPrimitives.ReadSingleBigEndian(actual[4..8]), Is.EqualTo(2.25f));
        Assert.That(actual[8..].All(b => b == 0), Is.True);
    }

    [Test]
    public async Task Serialize_WithDoubleData_CorrectlySerializesDataStream()
    {
        using var memory = new MemoryStream();
        var testee = new ImageSerializer<double>();
        var content = new ImageDataContent<double>([2], new System.Memory<double>([1.25, 2.25]));
        var header = new HeaderBuilder().WithContentDataType(DataContentType.DOUBLE).Build();

        await testee.SerializeAsync(PipeWriter.Create(memory), content, header);
        var actual = memory.ToArray();

        Assert.That(actual, Has.Length.EqualTo(2880));
        Assert.That(BinaryPrimitives.ReadDoubleBigEndian(actual[0..8]), Is.EqualTo(1.25f));
        Assert.That(BinaryPrimitives.ReadDoubleBigEndian(actual[8..16]), Is.EqualTo(2.25f));
        Assert.That(actual[16..].All(b => b == 0), Is.True);
    }
}
