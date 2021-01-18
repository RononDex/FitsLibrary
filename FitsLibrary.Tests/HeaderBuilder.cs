using FitsLibrary.DocumentParts;

namespace FitsLibrary.Tests
{
    internal class HeaderBuilder
    {
        private readonly Header header = new();

        public HeaderBuilder WithValidFitsFormat()
        {
            return WithHeaderEntry(
                key: "SIMPLE",
                value: true,
                comment: null);
        }

        public HeaderBuilder WithInvalidFitsFormat()
        {
            return WithHeaderEntry(
                key: "SIMPLE",
                value: false,
                comment: null);
        }

        public HeaderBuilder WithBitsPerValue(int bitpix)
        {
            return WithHeaderEntry(
                key: "BITPIX",
                value: bitpix,
                comment: null);
        }

        public HeaderBuilder WithNumberOfAxis(object numberOfAxis)
        {
            return WithHeaderEntry(
                key: "NAXIS",
                value: numberOfAxis,
                comment: null);
        }

        public HeaderBuilder WithHeaderEntry(string key, object? value, string? comment)
        {
            header.Entries.Add(new(
                key: key,
                value: value,
                comment: comment));

            return this;
        }

        public HeaderBuilder WithEndEntry()
        {
            return WithHeaderEntry(
                key: "END",
                value: null,
                comment: null);
        }

        public HeaderBuilder WithDimensionOfSize(int dimensionIndex, object size)
        {
            return WithHeaderEntry(
                key: $"NAXIS{dimensionIndex}",
                value: size,
                comment: null);
        }

        public HeaderBuilder WithEmptyHeader()
        {
            header.Entries.Clear();
            return this;
        }

        public Header Build()
        {
            return header;
        }
    }
}
