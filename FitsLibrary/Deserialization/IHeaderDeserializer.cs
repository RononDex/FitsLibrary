using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization;

internal interface IHeaderDeserializer
{
    public abstract Task<(bool endOfStreamReached, Header header)> DeserializeAsync(PipeReader dataStream);
}
