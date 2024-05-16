using System.IO.Pipelines;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.Deserialization.Image;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.Validation.Header;

namespace FitsLibrary.Deserialization.Primary;

internal class PrimaryHduDeserializer<T>(HeaderValidator headerValidator) : IHduDeserializer<ImageDataContent<T>> where T : INumber<T>
{
    private HeaderValidator HeaderValidator { get; } = headerValidator;

    public async Task<(bool endOfStreamReached, HeaderDataUnit<ImageDataContent<T>> data)> DeserializeAsync(PipeReader reader, Header header)
    {
        await HeaderValidator.ValidateHeader(header).ConfigureAwait(false);

        var contentDeserializer = new ImageContentDeserializer<T>();

        var (endOfStreamReachedContent, parsedContent) = await contentDeserializer.DeserializeAsync(reader, header).ConfigureAwait(false);

        return (
                endOfStreamReached: endOfStreamReachedContent,
                data: new ImageHeaderDataUnit<T>(HeaderDataUnitType.PRIMARY, header, parsedContent));
    }

    async Task<(bool endOfStreamReached, HeaderDataUnit data)> IHduDeserializer.DeserializeAsync(PipeReader reader, Header header)
    {
        var (endOfStreamReached, data) = await DeserializeAsync(reader, header);
        return (endOfStreamReached, data);
    }
}
