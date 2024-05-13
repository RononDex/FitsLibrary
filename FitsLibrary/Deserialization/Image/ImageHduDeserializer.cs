using System.IO.Pipelines;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.Validation.Header;

namespace FitsLibrary.Deserialization.Image;

internal class ImageHduDeserializer<T>(HeaderValidator headerValidator) : IHduDeserializer<ImageDataContent<T>> where T : INumber<T>
{
    private HeaderValidator HeaderValidator { get; } = headerValidator;

    public async Task<(bool endOfStreamReached, HeaderDataUnit<ImageDataContent<T>> data)> DeserializeAsync(PipeReader reader, Header header)
    {
        await this.HeaderValidator.ValidateHeader(header).ConfigureAwait(false);

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
