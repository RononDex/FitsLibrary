using System;
using System.IO.Pipelines;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public interface IContentDeserializer<T> where T : INumber<T>
    {
        public Task<(bool endOfStreamReached, Memory<T>? contentData)> DeserializeAsync(PipeReader dataStream, Header header);
    }
}
