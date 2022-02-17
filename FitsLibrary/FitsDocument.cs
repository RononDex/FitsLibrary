using System;
using System.Collections.Generic;
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

        /// <summary>
        /// A list of all extensions in the fits file
        /// </summary>
        public List<Extension> Extensions { get; }

        /// <summary>
        /// Returns the value at the given coordinates as a byte
        /// </summary>
        /// <param name="coordinates">coordinates inside the multi dimensional array</param>
        public T GetValueAt(params int[] coordinates)
        {
            var index = GetIndexByCoordinates(coordinates);
            return RawData!.Value.Span[index];
        }

        private int GetIndexByCoordinates(params int[] coordinates)
        {
            var index = 0;
            var axisIndexFactorsSpan = AxisIndexFactors.Span;
            for (var i = 0; i < coordinates.Length; i++)
            {
                index += coordinates[i] * axisIndexFactorsSpan[i];
            }

            return index;
        }
    }
}
