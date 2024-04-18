using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization.Image.ImageContentValueDeserializer;

public class ImageContentValueDeserializerFloat : IImageContentValueDeserializer<float>
{
    public float ParseValue(ReadOnlySpan<byte> currentValueBytes)
    {
        return BinaryPrimitives.ReadSingleBigEndian(currentValueBytes);
    }
}
