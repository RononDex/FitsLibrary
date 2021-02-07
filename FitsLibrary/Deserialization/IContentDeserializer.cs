using System.IO;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public interface IContentDeserializer
    {
        public abstract Task<Content?> DeserializeAsync(Stream dataStream, Header header);
    }
}
