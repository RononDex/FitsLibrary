using System.IO;

namespace FitsLibrary.Deserialization
{
    public abstract class BaseDeserializer<T>
    {
        public abstract T Deserialize(Stream dataStream);
    }
}
