namespace FitsLibrary.DocumentParts.Objects
{
    public class HeaderEntry
    {
        public string Key { get; }

        public object? Value { get; }

        public string? Comment { get; }

        public HeaderEntry(string key, object? value, string? comment)
        {
            Key = key;
            Value = value;
            Comment = comment;
        }
    }
}
