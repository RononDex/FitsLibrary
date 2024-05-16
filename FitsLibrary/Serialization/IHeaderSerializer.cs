
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace FitsLibrary.Serialization;

internal interface IHeaderSerializer
{
    Task SerializeAsync(DocumentParts.Header header, PipeWriter writer);
}
