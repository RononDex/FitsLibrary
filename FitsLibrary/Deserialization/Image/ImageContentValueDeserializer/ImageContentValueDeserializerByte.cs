using System;

namespace FitsLibrary.Deserialization.Image.ImageContentValueDeserializer;

internal class ImageContentValueDeserializerByte : IImageContentValueDeserializer<byte>
{
    public byte ParseValue(ReadOnlySpan<byte> currentValueBytes)
    {
        return currentValueBytes[0];
    }
}
