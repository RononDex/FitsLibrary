using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization
{
    public class ContentDeserializerInt64 : ContentDeserializer<long>
    {
        protected override long ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return BinaryPrimitives.ReadInt64BigEndian(currentValueBytes);
        }
    }
}
