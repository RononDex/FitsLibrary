using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization
{
    public class ContentDeserializerInt32 : ContentDeserializer<int>
    {
        protected override int ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return BinaryPrimitives.ReadInt32BigEndian(currentValueBytes);
        }
    }
}
