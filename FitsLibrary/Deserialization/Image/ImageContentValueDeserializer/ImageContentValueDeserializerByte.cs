using System;

namespace FitsLibrary.Deserialization.Image.ImageContentValueDeserializer;

public class ImageContentValueDeserializerByte : IImageContentValueDeserializer<byte>
{
    public byte ParseValue(ReadOnlySpan<byte> currentValueBytes)
    {
        return currentValueBytes[0];
    }
}
