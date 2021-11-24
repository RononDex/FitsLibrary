using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization
{
    public class ContentDeserializerFloat : ContentDeserializer<float>
    {
        protected override float ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return BinaryPrimitives.ReadSingleBigEndian(currentValueBytes);
        }
    }
}
