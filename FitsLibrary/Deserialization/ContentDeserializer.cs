using System.IO;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Deserialization
{
    public class ContentDeserializer : IDeserializer<Content>
    {
        Task<Content> IDeserializer<Content>.DeserializeAsync(Stream dataStream)
        {
            throw new System.NotImplementedException();
        }
    }
}
