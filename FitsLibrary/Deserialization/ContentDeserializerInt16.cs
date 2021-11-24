using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization
{
    public class ContentDeserializerInt16 : ContentDeserializer<short>
    {
        protected override short ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return BinaryPrimitives.ReadInt16BigEndian(currentValueBytes);
        }
    }
}
