using System;
using System.Buffers.Binary;

namespace FitsLibrary.Serialization.Image.ImageContentValueSerializer;

internal class ImageContentValueSerializerShort : IImageContentValueSerializer<short>
{
    public void WriteValue(short valueToWrite, Span<byte> target)
    {
        BinaryPrimitives.WriteInt16BigEndian(target, valueToWrite);
    }
}
