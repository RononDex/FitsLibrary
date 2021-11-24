using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization
{
    public class ContentDeserializerByte : ContentDeserializer<byte>
    {
        protected override byte ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return currentValueBytes[0];
        }
    }
}
