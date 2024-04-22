
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Serialization;

internal interface IContentSerializer
{
    Task SerializeAsync(PipeWriter writer, DataContent dataContent, DocumentParts.Header header);
}
