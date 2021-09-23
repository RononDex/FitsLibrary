using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public interface IHeaderDeserializer
    {
        public abstract Task<(bool endOfStreamReached, Header? parsedHeader)> DeserializeAsync(PipeReader dataStream);
    }
}
