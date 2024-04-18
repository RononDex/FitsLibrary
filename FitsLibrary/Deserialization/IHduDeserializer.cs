
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization;

internal interface IHduDeserializer<T, H> : IHduDeserializer where H : Header
{
    new Task<(bool endOfStreamReached, HeaderDataUnit<T, H> data)> DeserializeAsync(PipeReader reader, Header header);
}

internal interface IHduDeserializer
{
    Task<(bool endOfStreamReached, HeaderDataUnit data)> DeserializeAsync(PipeReader reader, Header header);
}
