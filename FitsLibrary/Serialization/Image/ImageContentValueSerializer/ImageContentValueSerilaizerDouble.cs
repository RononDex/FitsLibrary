using System;
using System.Buffers.Binary;

namespace FitsLibrary.Serialization.Image.ImageContentValueSerializer;

internal class ImageContentValueSerializerDouble : IImageContentValueSerializer<double>
{
    public void WriteValue(double valueToWrite, Span<byte> target)
    {
        BinaryPrimitives.WriteDoubleBigEndian(target, valueToWrite);
    }
}
