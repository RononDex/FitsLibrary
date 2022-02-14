using System;

namespace FitsLibrary.Deserialization.ContentValueDeserializer
{
    public interface IContentValueDeserializer<TData> where TData : INumber<TData>
    {
        TData ParseValue(ReadOnlySpan<byte> currentValueBytes);
    }

}
