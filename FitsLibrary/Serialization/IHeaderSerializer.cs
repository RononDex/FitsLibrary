
using System.IO.Pipelines;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Serialization
{
    public interface IHeaderSerializer
    {
        void SerializeAsync(Header header, PipeWriter writer);
    }
}
