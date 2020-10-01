namespace FitsLibrary.DocumentParts.Objects
{
    public class HeaderEntry
    {
        public string Key { get; }

        public string Value { get; }

        public string? Comment { get; }

        public HeaderEntry(string key, string value, string? comment)
        {
            Key = key;
            Value = value;
            Comment = comment;
        }
    }
}
