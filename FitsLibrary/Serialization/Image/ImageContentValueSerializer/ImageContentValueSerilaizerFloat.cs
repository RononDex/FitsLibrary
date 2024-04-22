using System;
using System.Buffers.Binary;

namespace FitsLibrary.Serialization.Image.ImageContentValueSerializer;

internal class ImageContentValueSerializerFloat : IImageContentValueSerializer<float>
{
    public void WriteValue(float valueToWrite, Span<byte> target)
    {
        BinaryPrimitives.WriteSingleBigEndian(target, valueToWrite);
    }
}
