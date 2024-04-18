
namespace FitsLibrary.DocumentParts;

public class HeaderDataUnit<T, H> : HeaderDataUnit where H : Header
{
    public T Data { get; }
    public new H Header { get; }

    public HeaderDataUnit(HeaderDataUnitType type, H header, T data) : base(type, header)
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
