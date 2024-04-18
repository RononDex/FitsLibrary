using System.IO;
using System.Threading.Tasks;

namespace FitsLibrary;

public interface IFitsDocumentWriter
{
    Task WriteAsync(FitsDocument document, string filePath);

    Task WriteAsync(FitsDocument document, Stream writeToStream);
}
