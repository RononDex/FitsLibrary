using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;

namespace FitsLibrary.Tests;

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

    public HeaderBuilder WithContentDataType(DataContentType dataContentType)
    {
        return WithHeaderEntry(
            key: "BITPIX",
            value: (int)dataContentType,
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

    public HeaderBuilder WithAxisOfSize(int dimensionIndex, long size)
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
