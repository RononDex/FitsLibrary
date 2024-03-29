namespace FitsLibrary.DocumentParts.Objects
{
    public class HeaderEntry
    {
        public string Key { get; set; }

        public object? Value { get; set; }

        public string? Comment { get; set; }

        public HeaderEntry(string key, object? value, string? comment)
        {
            Key = key;
            Value = value;
            Comment = comment;
        }
    }
}
