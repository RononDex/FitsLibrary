
namespace FitsLibrary.DocumentParts
{
    public class HeaderDataUnit
    {
        public Header Header { get; private set; }
        public DataContent Data { get; private set; }

        public HeaderDataUnit(Header header, DataContent data)
        {
            this.Header = header;
            this.Data = data;
        }
    }
}
