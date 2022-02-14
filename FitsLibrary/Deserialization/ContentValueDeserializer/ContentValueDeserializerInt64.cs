using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization.ContentValueDeserializer
{
    public class ContentValueDeserializerInt64 : IContentValueDeserializer<long>
    {
        public long ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return BinaryPrimitives.ReadInt64BigEndian(currentValueBytes);
        }
    }
}
