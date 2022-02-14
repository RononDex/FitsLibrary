using System;
using System.Buffers.Binary;

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
