using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public interface IContentDeserializer
    {
        public abstract Task<Content?> DeserializeAsync(PipeReader dataStream, Header header);
    }
}
