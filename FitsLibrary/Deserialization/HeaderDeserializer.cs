using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private const int HaderEntryChunkSize = 80;
        private const int HeaderBlockSize = 2880;

        /// <summary>
        /// Representation of the Headers END marker
        /// "END" + 77 spaces in ASCII
        /// </summary>
        public static readonly byte[] END_MARKER =
            new List<byte>{ 0x45, 0x4e, 0x44}
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

                endOfHeaderReached = ParseHeaderBlock(headerBlock);
            }

            return new Header(headerEntries);
        }

        private bool ParseHeaderBlock(byte[] headerBlock)
        {
            var endOfHeaderReached = false;
            var headerEntryChunks = headerBlock.Split(HaderEntryChunkSize);

            foreach (var headerEntryChunk in headerEntryChunks)
            {
                if (headerEntryChunk.SequenceEqual(END_MARKER))
                {
                    endOfHeaderReached = true;
                }
            }

            return endOfHeaderReached;
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
