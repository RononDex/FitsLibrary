using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace FitsLibrary
{
    public interface IFitsDocumentWriter<T> where T : INumber<T>
    {
        Task WriteAsync(FitsDocument<T> document, string filePath);

        Task WriteAsync(FitsDocument<T> document, Stream writeToStream);
    }
}
