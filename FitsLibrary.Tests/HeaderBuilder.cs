using FitsLibrary.DocumentParts;

namespace FitsLibrary.Tests
{
    internal class HeaderBuilder
    {
        private readonly Header header = new();

        public HeaderBuilder WithNumberOfAxis(int numberOfAxis)
        {
            header.Entries.Add(new(
                key: "NAXIS",
                value: numberOfAxis,
                comment: null));

            return this;
        }

        public Header Build()
        {
            return header;
        }
    }
}
