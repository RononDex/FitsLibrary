using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization.Image.ImageContentValueDeserializer;

internal class ImageContentValueDeserializerInt32 : IImageContentValueDeserializer<int>
{
    public int ParseValue(ReadOnlySpan<byte> currentValueBytes)
    {
        return BinaryPrimitives.ReadInt32BigEndian(currentValueBytes);
    }
}
