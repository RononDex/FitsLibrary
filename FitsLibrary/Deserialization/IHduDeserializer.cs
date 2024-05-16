
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization;

internal interface IHduDeserializer<T> : IHduDeserializer
{
    new Task<(bool endOfStreamReached, HeaderDataUnit<T> data)> DeserializeAsync(PipeReader reader, Header header);
}

internal interface IHduDeserializer
{
    Task<(bool endOfStreamReached, HeaderDataUnit data)> DeserializeAsync(PipeReader reader, Header header);
}
