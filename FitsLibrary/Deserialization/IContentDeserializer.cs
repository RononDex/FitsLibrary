using System;
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public interface IContentDeserializer<TData> where TData : INumber<TData>
    {
        public Task<(bool endOfStreamReached, Memory<TData>? contentData)> DeserializeAsync(PipeReader dataStream, Header header);
    }
}
