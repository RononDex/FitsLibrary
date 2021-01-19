using System.IO;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public interface IHeaderDeserializer
    {
        public abstract Task<Header> DeserializeAsync(Stream dataStream);
    }
}
