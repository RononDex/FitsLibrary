using System;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public interface IContentDeserializer
    {
        public abstract Task<(bool endOfStreamReached, Memory<object>? contentData)> DeserializeAsync(PipeReader dataStream, Header header);
    }
}
