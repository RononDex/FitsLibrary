using System.IO.Pipelines;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.Validation.Header;

namespace FitsLibrary.Serialization.Image;

internal class ImageHduSerializer<T>(IHeaderSerializer headerSerializer, HeaderValidator headerValidator) : IHduSerializer<ImageHeaderDataUnit<T>> where T : INumber<T>
{
    private HeaderValidator HeaderValidator { get; } = headerValidator;

    private IHeaderSerializer HeaderSerializer { get; } = headerSerializer;

    public async Task SerializeAsync(PipeWriter writer, ImageHeaderDataUnit<T> hdu)
    {
        await this.HeaderValidator.ValidateHeader(hdu.Header);
        var imageSerializer = new ImageSerializer<T>();
        await this.HeaderSerializer.SerializeAsync(hdu.Header, writer);
        await imageSerializer.SerializeAsync(writer, hdu.Data, hdu.Header);
    }


    public Task SerializeAsync(PipeWriter writer, HeaderDataUnit hdu)
    {
        return this.SerializeAsync(writer, (ImageHeaderDataUnit<T>)hdu);
    }
}

