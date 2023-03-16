using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization.ContentValueDeserializer
{
    public class ContentValueDeserializerFloat : IContentValueDeserializer<float>
    {
        public float ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return BinaryPrimitives.ReadSingleBigEndian(currentValueBytes);
        }
    }
}
