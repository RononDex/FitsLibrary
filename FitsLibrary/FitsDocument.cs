using System;
using System.Collections.Generic;
using System.Numerics;
using FitsLibrary.DocumentParts;

namespace FitsLibrary
{
    /// <summary>
    /// Represents a .fits document for read and write access
    /// </summary>
    public class FitsDocument<T> where T : INumber<T>
    {
        private Memory<int> AxisIndexFactors;

        /// <summary>
        /// Creates a new .fits document with a the given data
        /// </summary>
        /// <param name="header">The main header</param>
        /// <param name="content">The data content of the fits document</param>
        public FitsDocument(
            Header header,
            Memory<T>? content,
            List<Extension>? extensions = null)
        {
            Header = header;
            RawData = content;
            Extensions = extensions ?? new List<Extension>();

            InitHelperData();
        }

        private void InitHelperData()
        {
            if (RawData.HasValue)
            {
                AxisIndexFactors = new int[Header.NumberOfAxisInMainContent];
                var span = AxisIndexFactors.Span;
                span[0] = 1;

                for (var i = 1; i < AxisIndexFactors.Length; i++)
                {
                    span[i] = span[i - 1] * Convert.ToInt32(Header[$"NAXIS{i}"]);
                }
            }
        }

        /// <summary>
        /// The main data content of the fits file
        /// </summary>
        public Memory<T>? RawData { get; }

        /// <summary>
        /// A list of headers in this document
        /// </summary>
        public Header Header { get; }

    }
}
