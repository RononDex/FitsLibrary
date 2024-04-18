using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization;

internal interface IContentDeserializer
{
    Task<(bool endOfStreamReached, DataContent data)> DeserializeAsync(PipeReader dataStream, Header header);
}
