
using System.IO.Pipelines;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;

namespace FitsLibrary.Serialization;

internal interface IHduSerializer<T> : IHduSerializer where T : HeaderDataUnit
{
    Task SerializeAsync(PipeWriter writer, T hdu);
}

internal interface IHduSerializer
{
    Task SerializeAsync(PipeWriter writer, HeaderDataUnit hdu);
}
