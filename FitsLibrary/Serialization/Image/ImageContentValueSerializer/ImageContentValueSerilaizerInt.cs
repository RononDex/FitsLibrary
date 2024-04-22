using System;
using System.Buffers.Binary;

namespace FitsLibrary.Serialization.Image.ImageContentValueSerializer;

internal class ImageContentValueSerializerInt : IImageContentValueSerializer<int>
{
    public void WriteValue(int valueToWrite, Span<byte> target)
    {
        BinaryPrimitives.WriteInt32BigEndian(target, valueToWrite);
    }
}
