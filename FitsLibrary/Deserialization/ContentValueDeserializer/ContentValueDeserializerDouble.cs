using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization.ContentValueDeserializer
{
    public class ContentValueDeserializerDouble : IContentValueDeserializer<double>
    {
        public double ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return BinaryPrimitives.ReadDoubleBigEndian(currentValueBytes);
        }
    }
}
