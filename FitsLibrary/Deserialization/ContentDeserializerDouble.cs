using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization
{
    public class ContentDeserializerDouble : ContentDeserializer<double>
    {
        protected override double ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return BinaryPrimitives.ReadDoubleBigEndian(currentValueBytes);
        }
    }
}
