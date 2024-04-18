
namespace FitsLibrary.DocumentParts;

public class HeaderDataUnit<T> : HeaderDataUnit
{
    public T Data { get; }

    public HeaderDataUnit(HeaderDataUnitType type, Header header, T data) : base(type, header)
    {
        this.Data = data;
    }
}

public abstract class HeaderDataUnit
{
    protected HeaderDataUnit(HeaderDataUnitType type, Header header)
    {
        this.Type = type;
        this.Header = header;
    }
    public Header Header { get; }
    public HeaderDataUnitType Type { get; }
}
