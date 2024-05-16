using System.IO;
using System.Threading.Tasks;

namespace FitsLibrary;

public interface IFitsDocumentReader
{
    Task<FitsDocument> ReadAsync(Stream inputStream);

    Task<FitsDocument> ReadAsync(string filePath);
}
