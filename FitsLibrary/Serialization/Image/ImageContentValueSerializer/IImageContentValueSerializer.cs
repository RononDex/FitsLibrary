using System;
using System.Numerics;

namespace FitsLibrary.Serialization.Image.ImageContentValueSerializer;

internal interface IImageContentValueSerializer<TData> where TData : INumber<TData>
{
    void WriteValue(TData valueToWrite, Span<byte> target);
}
