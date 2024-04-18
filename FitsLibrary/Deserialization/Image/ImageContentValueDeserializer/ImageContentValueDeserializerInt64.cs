using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization.Image.ImageContentValueDeserializer;

public class ImageContentValueDeserializerInt64 : IImageContentValueDeserializer<long>
{
    public long ParseValue(ReadOnlySpan<byte> currentValueBytes)
    {
        return BinaryPrimitives.ReadInt64BigEndian(currentValueBytes);
    }
}
