using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization.Image.ImageContentValueDeserializer;

internal class ImageContentValueDeserializerDouble : IImageContentValueDeserializer<double>
{
    public double ParseValue(ReadOnlySpan<byte> currentValueBytes)
    {
        return BinaryPrimitives.ReadDoubleBigEndian(currentValueBytes);
    }
}
