using System;
using System.Buffers.Binary;

namespace FitsLibrary.Deserialization.ContentValueDeserializer
{
    public class ContentValueDeserializerInt16 : IContentValueDeserializer<short>
    {
        public short ParseValue(ReadOnlySpan<byte> currentValueBytes)
        {
            return BinaryPrimitives.ReadInt16BigEndian(currentValueBytes);
        }
    }
}
