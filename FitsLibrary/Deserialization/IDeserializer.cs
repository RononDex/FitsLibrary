using System.IO;
using System.Threading.Tasks;

namespace FitsLibrary.Deserialization
{
    public interface IDeserializer<T>
    {
        public abstract Task<T> DeserializeAsync(Stream dataStream);
    }
}
