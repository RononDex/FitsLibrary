using System;
using System.Buffers.Binary;

namespace FitsLibrary.Serialization.Image.ImageContentValueSerializer;

internal class ImageContentValueSerializerLong : IImageContentValueSerializer<long>
{
    public void WriteValue(long valueToWrite, Span<byte> target)
    {
        BinaryPrimitives.WriteInt64BigEndian(target, valueToWrite);
    }
}
