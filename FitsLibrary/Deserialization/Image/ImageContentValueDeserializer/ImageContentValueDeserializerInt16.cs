using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization.Image.ImageContentValueDeserializer;

internal class ImageContentValueDeserializerInt16 : IImageContentValueDeserializer<short>
{
    public short ParseValue(ReadOnlySpan<byte> currentValueBytes)
    {
        return BinaryPrimitives.ReadInt16BigEndian(currentValueBytes);
    }
}
