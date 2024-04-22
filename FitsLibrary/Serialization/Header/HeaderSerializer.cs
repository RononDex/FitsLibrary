using System;
using System.Globalization;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts.Objects;
using FitsLibrary.Extensions;

namespace FitsLibrary.Serialization.Header;

internal class HeaderSerializer : IHeaderSerializer
{
    public const char ContinuedStringMarker = '&';
    public const int HeaderBlockSize = 2880;
    public const int HeaderEntryChunkSize = 80;
    public const int LogicalValuePosition = 20;

    public Task SerializeAsync(DocumentParts.Header header, PipeWriter writer)
    {
        var headerBlockBuilder = new StringBuilder();
        var headerEntryBuilder = new StringBuilder();
        foreach (var headerEntry in header.Entries)
        {
            headerEntryBuilder.Clear();
            headerEntryBuilder.Append(headerEntry.Key.PadRight(8));

            if (headerEntry.Value != null)
            {
                _ = headerEntryBuilder.Append("= ");
                var serializedValue = GetSerializedValue(headerEntry.Value);

                // Handle strings and long strings
                if (headerEntry.Value is string)
                {
                    HandleStringEntries(headerEntryBuilder, serializedValue, headerEntry);
                }
                else
                {
                    _ = headerEntryBuilder.Append(serializedValue);

                    if (!string.IsNullOrEmpty(headerEntry.Comment))
                    {
                        headerEntryBuilder.AppendFormat(" / {0}", headerEntry.Comment);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(headerEntry.Comment))
                {
                    _ = headerEntryBuilder.Append(headerEntry.Comment);
                }
            }

            _ = headerBlockBuilder.Append(headerEntryBuilder.ToString().PadRight(headerEntryBuilder.Length % HeaderEntryChunkSize == 0
                        ? 0
                        : ((headerEntryBuilder.Length / HeaderEntryChunkSize) + 1) * HeaderEntryChunkSize));
        }


        _ = headerBlockBuilder.Append("END".PadRight(HeaderEntryChunkSize));
        var numberOfHeaderBlocks = headerBlockBuilder.Length / HeaderBlockSize;
        if (headerBlockBuilder.Length % HeaderBlockSize != 0)
        {
            numberOfHeaderBlocks++;
        }
        var length = numberOfHeaderBlocks * HeaderBlockSize;
        var writerMemory = writer.GetMemory(sizeHint: length);
        var bytes = Encoding.ASCII.GetBytes(headerBlockBuilder.ToString().PadRight(length), writerMemory.Span);
        writer.Advance(bytes);
        return writer.FlushAsync().AsTask();
    }
    private static string GetSerializedValue(object value)
    {
        return value switch
        {
            bool bValue => bValue ? "T" : "F",
            string sValue => sValue.Replace("'", "''", StringComparison.Ordinal),
            int iValue => string.Format(CultureInfo.InvariantCulture, "{0,20}", iValue),
            uint iValue => string.Format(CultureInfo.InvariantCulture, "{0,20}", iValue),
            long lValue => string.Format(CultureInfo.InvariantCulture, "{0,20}", lValue),
            ulong lValue => string.Format(CultureInfo.InvariantCulture, "{0,20}", lValue),
            float fValue => string.Format(CultureInfo.InvariantCulture, "{0,20}", fValue),
            double dValue => string.Format(CultureInfo.InvariantCulture, "{0,20}", dValue),
            short sValue => string.Format(CultureInfo.InvariantCulture, "{0,20}", sValue),
            ushort sValue => string.Format(CultureInfo.InvariantCulture, "{0,20}", sValue),
            byte bValue => string.Format(CultureInfo.InvariantCulture, "{0,20}", bValue),
            _ => value.ToString()!
        };
    }

    private static void HandleMultiLineString(StringBuilder headerEntryBuilder, string serializedValue, HeaderEntry entry)
    {
        var subStrings = serializedValue.SplitInParts(67);
        for (var i = 0; i < subStrings.Length; i++)
        {
            if (i != 0)
            {
                headerEntryBuilder.Append("CONTINUE  ");
            }
            if (i != subStrings.Length - 1)
            {
                headerEntryBuilder.AppendFormat("'{0}&'", subStrings[i]);
            }
            else
            {
                if (entry.Comment == null || entry.Comment.Length <= HeaderEntryChunkSize - 10 - subStrings[i].Length - 2 - 3)
                {
                    headerEntryBuilder.AppendFormat("'{0}'", subStrings[i]);
                    if (entry.Comment != null)
                    {
                        headerEntryBuilder.AppendFormat(" / {0}", entry.Comment);
                    }
                }
                else
                {
                    var firstCommentPart = entry.Comment.Substring(0, HeaderEntryChunkSize - 10 - subStrings[i].Length - 3 - 3);
                    var restOfComment = entry.Comment.Substring(firstCommentPart.Length);
                    headerEntryBuilder.AppendFormat("'{0}&' / {1}", subStrings[i], firstCommentPart);

                    var commentSplit = restOfComment.SplitInParts(HeaderEntryChunkSize - 10 - 3 - 3);
                    for (var j = 0; j < commentSplit.Length; j++)
                    {
                        if (j != commentSplit.Length - 1)
                        {
                            headerEntryBuilder.AppendFormat("CONTINUE  '&' / {0}", commentSplit[j]);
                        }
                        else
                        {
                            headerEntryBuilder.AppendFormat("CONTINUE  '' / {0}", commentSplit[j]);
                        }
                    }
                }
            }
        }
    }

    private static void HandleSingleLineString(StringBuilder headerEntryBuilder, string serializedValue, HeaderEntry entry)
    {
        if (entry.Comment == null)
        {
            headerEntryBuilder.AppendFormat("'{0}'", serializedValue);
        }
        else
        {
            headerEntryBuilder.AppendFormat("'{0}' / {1}", serializedValue, entry.Comment);
        }
    }

    private static void HandleStringEntries(StringBuilder headerEntryBuilder, string serializedValue, HeaderEntry entry)
    {
        if (serializedValue.Length <= 70)
        {
            HandleSingleLineString(headerEntryBuilder, serializedValue, entry);
        }
        else
        {
            HandleMultiLineString(headerEntryBuilder, serializedValue, entry);
        }
    }
}
