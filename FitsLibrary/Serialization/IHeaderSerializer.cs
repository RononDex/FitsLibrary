
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Serialization
{
    public interface IHeaderSerializer
    {
        Task SerializeAsync(Header header, PipeWriter writer);
    }
}
