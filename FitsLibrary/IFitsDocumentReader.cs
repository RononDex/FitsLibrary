using System;
using System.IO;
using System.Threading.Tasks;

namespace FitsLibrary
{
    public interface IFitsDocumentReader<T> where T : INumber<T>
    {
        Task<FitsDocument<T>> ReadAsync(Stream inputStream);

        Task<FitsDocument<T>> ReadAsync(string filePath);
    }
}
