using System;

namespace FitsLibrary.Serialization.Image.ImageContentValueSerializer;

internal class ImageContentValueSerilaizerByte : IImageContentValueSerializer<byte>
{
    public void WriteValue(byte valueToWrite, Span<byte> target)
    {
        target[0] = valueToWrite;
    }
}
