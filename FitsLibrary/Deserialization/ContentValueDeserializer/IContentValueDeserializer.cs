using System;
using System.Numerics;

namespace FitsLibrary.Deserialization.ContentValueDeserializer
{
    public interface IContentValueDeserializer<TData> where TData : INumber<TData>
    {
        TData ParseValue(ReadOnlySpan<byte> currentValueBytes);
    }

}
