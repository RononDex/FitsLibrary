using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.Objects;

namespace FitsLibrary.Deserialization.Head;

internal class HeaderDeserializer : IHeaderDeserializer
{
    /// <summary>
    /// Length of a header entry chunk, containing a single header entry
    /// </summary>
    public const int HeaderEntryChunkSize = 80;
    public const int HeaderBlockSize = 2880;
    public const int LogicalValuePosition = 20;
    public const char ContinuedStringMarker = '&';
    private const string ContinueKeyWord = "CONTINUE";

    /// <summary>
    /// Representation of the Headers END marker
    /// "END" + 77 spaces in ASCII
    /// </summary>
    public static readonly byte[] END_MARKER =
        [0x45, 0x4e, 0x44, .. Enumerable.Repeat(element: (byte)0x20, count: 77)];

    /// <summary>
    /// Deserializes the header part of the fits document
    /// </summary>
    /// <param name="dataStream">the stream from which to read the data from (should be at position 0)</param>
    /// <exception cref="InvalidDataException"></exception>
    public async Task<(bool endOfStreamReached, Header? header)> DeserializeAsync(PipeReader dataStream)
    {
        PreValidateStream(dataStream);

        var endOfHeaderReached = false;
        var endOfStreamReached = false;
        var headerEntries = new List<HeaderEntry>();

        while (!endOfHeaderReached)
        {
            var result = await dataStream.ReadAsync().ConfigureAwait(false);
            var headerBlock = result.Buffer;
            if (headerBlock.Length < HeaderBlockSize)
            {
                Console.WriteLine("WARNING: Failed to parse the header, it appears the file does not conform to the fits standard and has an invalid amount of bytes at the end");
                return (true, null);
            }

            headerEntries.AddRange(ParseHeaderBlock(headerBlock, out endOfHeaderReached));
            dataStream.AdvanceTo(result.Buffer.GetPosition(HeaderBlockSize), result.Buffer.End);

            if (!endOfHeaderReached && result.Buffer.Length <= HeaderBlockSize && result.IsCompleted)
            {
                await dataStream.CompleteAsync().ConfigureAwait(false);
                throw new InvalidDataException("No END marker found for the fits header, fits file might be corrupted");
            }

            endOfStreamReached = endOfHeaderReached && result.IsCompleted && result.Buffer.Length <= HeaderBlockSize;
        }

        return (endOfStreamReached, header: new Header(headerEntries));
    }

    private static List<HeaderEntry> ParseHeaderBlock(ReadOnlySequence<byte> headerBlock, out bool endOfHeaderReached)
    {
        endOfHeaderReached = false;
        var currentIndex = 0;
        Span<byte> headerBlockSpan = stackalloc byte[Convert.ToInt32(HeaderBlockSize)];
        headerBlock.Slice(0, HeaderBlockSize).CopyTo(headerBlockSpan);
        var headerEntries = new List<HeaderEntry>();
        var isContinued = false;

        while (currentIndex < HeaderBlockSize)
        {
            var headerEntryChunk = headerBlockSpan.Slice(currentIndex, HeaderEntryChunkSize);
            currentIndex += HeaderEntryChunkSize;

            if (headerEntryChunk.SequenceEqual(END_MARKER))
            {
                endOfHeaderReached = true;
                break;
            }

            var parsedHeaderEntry = ParseHeaderEntryChunk(headerEntryChunk);
            if (!isContinued)
            {
                if (ValueIsStringAndHasContinueMarker(parsedHeaderEntry.Value))
                {
                    isContinued = true;
                    parsedHeaderEntry.Value = (parsedHeaderEntry.Value as string)!.Trim()[..^1];
                }

                headerEntries.Add(parsedHeaderEntry);
            }
            else
            {
                if (!string.Equals(parsedHeaderEntry.Key, ContinueKeyWord, StringComparison.Ordinal))
                {
                    throw new InvalidDataException("Unfinished continued value found");
                }
                var valueToAppend = parsedHeaderEntry.Value as string;
                if (ValueIsStringAndHasContinueMarker(parsedHeaderEntry.Value))
                {
                    valueToAppend = valueToAppend!.Trim()[..^1];
                    isContinued = true;
                }
                else
                {
                    isContinued = false;
                }
                headerEntries[^1].Value = $"{headerEntries[^1].Value as string}{valueToAppend}";
                if (parsedHeaderEntry.Comment != null)
                {
                    headerEntries[^1].Comment += $" {parsedHeaderEntry.Comment}";
                }
            }
        }

        return headerEntries;
    }

    private static bool ValueIsStringAndHasContinueMarker(object? value)
    {
        return value is string parsedString && parsedString.Trim().EndsWith(ContinuedStringMarker);
    }

    private static HeaderEntry ParseHeaderEntryChunk(ReadOnlySpan<byte> headerEntryChunk)
    {
        var key = Encoding.ASCII.GetString(headerEntryChunk.Slice(0, 8)).Trim();
        var isCommentOrHistoryEntry = IsCommentOrHistoryEntry(key);
        if (HeaderEntryChunkHasValueMarker(headerEntryChunk)
                || HeaderEntryEntryChunkHasContinueMarker(key)
                || isCommentOrHistoryEntry)
        {
            ReadOnlySpan<char> value = Encoding.ASCII.GetString(headerEntryChunk.Slice(10, 70)).Trim();
            if (value.IndexOf('/') != -1)
            {
                var comment = value[(value.IndexOf('/') + 1)..].Trim().Trim('\0');
                value = value[0..value.IndexOf('/')].Trim();
                var parsedValue = ParseValue(value, isCommentOrHistoryEntry);
                return new HeaderEntry(key, parsedValue, new string(comment));
            }
            else
            {
                var parsedValue = ParseValue(value, isCommentOrHistoryEntry);
                return new HeaderEntry(
                    key: key,
                    value: parsedValue,
                    comment: null);
            }
        }

        return new HeaderEntry(
            key: key,
            value: null,
            comment: null);
    }

    private static bool IsCommentOrHistoryEntry(string key)
    {
        return key == "HISTORY" || key == "COMMENT";
    }

    private static bool HeaderEntryEntryChunkHasContinueMarker(string key)
    {
        return string.Equals(key, ContinueKeyWord, StringComparison.Ordinal);
    }

    private static object? ParseValue(ReadOnlySpan<char> value, bool isCommentOrHistoryEntry)
    {
        value = value.Trim('\0').Trim(' ');
        if (value.Length == 0)
        {
            return null;
        }
        else if (value[0] == '\'')
        {
            return new String(value[1..^1]);
        }
        else if (isCommentOrHistoryEntry)
        {
            return new String(value);
        }
        else if (value.IndexOf('.') != -1)
        {
            return double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        }
        else if (value.Length == 1 && (value[0] == 'T' || value[0] == 'F'))
        {
            return value[0] == 'T';
        }
        return long.Parse(value);
    }

    private static bool HeaderEntryChunkHasValueMarker(ReadOnlySpan<byte> headerEntryChunk)
    {
        return headerEntryChunk[8] == 0x3D && headerEntryChunk[9] == 0x20;
    }

    private static void PreValidateStream(PipeReader dataStream)
    {
        if (dataStream == null)
            throw new ArgumentNullException(nameof(dataStream), "The Stream from which to read from can not be NULL");
    }
}
