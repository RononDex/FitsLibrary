

using System;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public static class Deserialization
    {

        public static IContentDeserializer GetContentDeserializerInstance(DataContentType dataContentType)
        {
            switch (dataContentType)
            {
                case DataContentType.DOUBLE:
                    return new ContentDeserializerDouble();
                case DataContentType.FLOAT:
                    return new ContentDeserializerFloat();
                case DataContentType.BYTE:
                    return new ContentDeserializerByte();
                case DataContentType.SHORT:
                    return new ContentDeserializerInt16();
                case DataContentType.INTEGER:
                    return new ContentDeserializerInt32();
                case DataContentType.LONG:
                    return new ContentDeserializerInt64();
                default:
                    throw new ArgumentException($"Invalid dataContentType {dataContentType}");

            }
        }
    }
}
