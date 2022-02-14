using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization.ContentValueDeserializer
{
    public class ContentValueDeserializerInt32 : IContentValueDeserializer<int>
    {
        public int ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return BinaryPrimitives.ReadInt32BigEndian(currentValueBytes);
        }
    }
}
