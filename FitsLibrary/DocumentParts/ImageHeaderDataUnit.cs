
using System.Numerics;
using FitsLibrary.DocumentParts.ImageData;

namespace FitsLibrary.DocumentParts;

public class ImageHeaderDataUnit<T> : HeaderDataUnit<ImageDataContent<T>> where T : INumber<T>
{
    public ImageHeaderDataUnit(HeaderDataUnitType type, Header header, ImageDataContent<T> data) : base(type, header, data)
    {
    }
}
