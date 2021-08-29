using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public interface IExtensionDeserializer
    {
        public abstract Task<Extension> DeserializeAsync(PipeReader dataStream);
    }
}
