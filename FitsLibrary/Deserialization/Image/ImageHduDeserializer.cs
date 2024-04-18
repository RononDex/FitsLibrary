using System.IO.Pipelines;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;

namespace FitsLibrary.Deserialization.Image;

internal class ImageHduDeserializer<T> : IHduDeserializer<ImageDataContent<T>> where T : INumber<T>
{
    public async Task<(bool endOfStreamReached, HeaderDataUnit<ImageDataContent<T>> data)> DeserializeAsync(PipeReader reader, Header header)
    {
        var contentDeserializer = new ImageContentDeserializer<T>();

        var (endOfStreamReachedContent, parsedContent) = await contentDeserializer.DeserializeAsync(reader, header).ConfigureAwait(false);

        return (
                endOfStreamReached: endOfStreamReachedContent,
                data: new ImageHeaderDataUnit<T>(HeaderDataUnitType.IMAGE, header, parsedContent));
    }

    Task<(bool endOfStreamReached, HeaderDataUnit data)> IHduDeserializer.DeserializeAsync(PipeReader reader, Header header)
    {
        return DeserializeAsync(reader, header).ContinueWith(t => ((bool endOfStreamReached, HeaderDataUnit data))t.Result);
    }
}
