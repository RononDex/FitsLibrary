using System;

namespace FitsLibrary.Deserialization.ContentValueDeserializer
{
    public class ContentValueDeserializerByte : IContentValueDeserializer<byte>
    {
        public byte ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return currentValueBytes[0];
        }
    }
}
