using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.Objects;
using FitsLibrary.Extensions;

namespace FitsLibrary.Deserialization
{
    public class HeaderDeserializer : BaseDeserializer<Header>
    {
        /// <summary>
        /// Length of a header entry chunk, containing a single header entry
        /// </summary>
        public const int HaderEntryChunkSize = 80;
        public const int HeaderBlockSize = 2880;
        public const int LogicalValuePosition = 20;

        /// <summary>
        /// Representation of the Headers END marker
        /// "END" + 77 spaces in ASCII
        /// </summary>
        public static readonly byte[] END_MARKER =
            new List<byte> { 0x45, 0x4e, 0x44 }
                .Concat(Enumerable.Repeat(element: (byte)0x20, count: 77))
                .ToArray();

        /// <summary>
        /// Deserializes the header part of the fits document
        /// </summary>
        /// <param name="dataStream">the stream from which to read the data from (should be at position 0)</param>
        public override Header Deserialize(Stream dataStream)
        {
            PreValidateStream(dataStream);

            var endOfHeaderReached = false;
            var headerEntries = new List<HeaderEntry>();

            while (!endOfHeaderReached)
            {
                if (dataStream.Length - dataStream.Position < HeaderBlockSize)
                {
                    throw new InvalidDataException("No END marker found for the fits header, fits file might be corrupted");
                }

                var headerBlock = new byte[HeaderBlockSize];
                dataStream.Read(headerBlock, 0, headerBlock.Length);

                headerEntries.AddRange(ParseHeaderBlock(headerBlock, out endOfHeaderReached));
            }

            return new Header(headerEntries);
        }

        private List<HeaderEntry> ParseHeaderBlock(byte[] headerBlock, out bool endOfHeaderReached)
        {
            endOfHeaderReached = false;
            var headerEntryChunks = headerBlock.Split(HaderEntryChunkSize).Select(arr => arr.ToArray());
            var headerEntries = new List<HeaderEntry>();

            foreach (var headerEntryChunk in headerEntryChunks)
            {
                if (headerEntryChunk.SequenceEqual(END_MARKER))
                {
                    endOfHeaderReached = true;
                    break;
                }

                headerEntries.Add(ParseHeaderEntryChunk(headerEntryChunk));
            }

            return headerEntries;
        }

        private static HeaderEntry ParseHeaderEntryChunk(byte[] headerEntryChunk)
        {
            var key = Encoding.ASCII.GetString(headerEntryChunk[0..7]).Trim();
            if (HeaderEntryChunkHasValueMarker(headerEntryChunk))
            {
                var value = Encoding.ASCII.GetString(headerEntryChunk[10..]).Trim();
                if (value.Contains('/'))
                {
                    var comment = value[(value.IndexOf('/') + 1)..].Trim();
                    value = value[0..value.IndexOf('/')].Trim();
                    var parsedValue = ParseValue(value);
                    return new HeaderEntry(key, parsedValue, comment);
                }
                else
                {

                    var parsedValue = ParseValue(value);
                    return new HeaderEntry(
                        key: key,
                        value: parsedValue,
                        comment: null);
                }
            }
            else
            {
                return new HeaderEntry(
                    key: key,
                    value: null,
                    comment: null);
            }
        }

        private static object ParseValue(string value)
        {
            if (value.StartsWith('\''))
            {
                return value.Replace("\'", string.Empty, StringComparison.Ordinal);
            }
            else if (value.Contains("."))
            {
                return Convert.ToDouble(value);
            }
            else if (value.Length >= LogicalValuePosition
                    && (value[LogicalValuePosition - 1] == 'T' || value[LogicalValuePosition - 1] == 'F'))
            {
                return value[LogicalValuePosition - 1] == 'T';
            }
            else
            {
                return Convert.ToInt64(value);
            }
        }

        private static bool HeaderEntryChunkHasValueMarker(byte[] headerEntryChunk)
        {
            return headerEntryChunk[8] == 0x3D && headerEntryChunk[9] == 0x20;
        }

        private void PreValidateStream(Stream dataStream)
        {
            if (dataStream == null)
                throw new ArgumentNullException(nameof(dataStream), "The Stream from which to read from can not be NULL");
            if (dataStream.Length == 0)
                throw new InvalidDataException("Empty data stream provided. Stream can not be empty!");
            if (dataStream.Length < HeaderBlockSize)
                throw new InvalidDataException("The data stream provided has an invalid length, the fits data stream might be corrupted");
        }
    }
}
