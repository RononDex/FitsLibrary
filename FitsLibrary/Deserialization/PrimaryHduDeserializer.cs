
using System.IO.Pipelines;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.Deserialization.Image;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;

namespace FitsLibrary.Deserialization;

internal class PrimaryHduDeserializer<T> : IHduDeserializer<ImageDataContent<T>, ImageHeader> where T : INumber<T>
{
    public async Task<(bool endOfStreamReached, HeaderDataUnit<ImageDataContent<T>, ImageHeader> data)> DeserializeAsync(PipeReader reader, Header header)
    {
        var contentDeserializer = new ImageContentDeserializer<T>();

        var (endOfStreamReachedContent, parsedContent) = await contentDeserializer.DeserializeAsync(reader, header).ConfigureAwait(false);

        return (
                endOfStreamReached: endOfStreamReachedContent,
                data: new ImageHeaderDataUnit<T>(HeaderDataUnitType.PRIMARY, new ImageHeader(header.Entries), parsedContent));
    }

    Task<(bool endOfStreamReached, HeaderDataUnit data)> IHduDeserializer.DeserializeAsync(PipeReader reader, Header header)
    {
        return DeserializeAsync(reader, header).ContinueWith(t => ((bool endOfStreamReached, HeaderDataUnit data))t.Result);
    }
}
