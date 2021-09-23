using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public interface IExtensionDeserializer
    {
        public abstract Task<(bool endOfStreamReached, Extension parsedExtension)> DeserializeAsync(PipeReader dataStream);
    }
}
