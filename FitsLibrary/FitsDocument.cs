using System;
using FitsLibrary.DocumentParts;

namespace FitsLibrary
{
    /// <summary>
    /// Represents a .fits document for read and write access
    /// </summary>
    public class FitsDocument
    {
        private Memory<int> AxisIndexFactors;

        /// <summary>
        /// Creates a new .fits document with a the given data
        /// </summary>
        /// <param name="header">The main header</param>
        /// <param name="content">The data content of the fits document</param>
        public FitsDocument(
            Header header,
            Memory<object>? content)
        {
            Header = header;
            RawData = content;

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
        public Memory<object>? RawData { get; }

        /// <summary>
        /// A list of headers in this document
        /// </summary>
        public Header Header { get; }

        /// <summary>
        /// Returns the value at the given coordinates as a byte
        /// </summary>
        /// <param name="coordinates">coordinates inside the multi dimensional array</param>
        public byte GetByteValueAt(params int[] coordinates)
        {
            var index = GetIndexByCoordinates(coordinates);
            return (byte)RawData!.Value.Span[index];
        }

        /// <summary>
        /// Returns the value at the given coordinates as a 32-bit integer
        /// </summary>
        /// <param name="coordinates">coordinates inside the multi dimensional array</param>
        public int GetInt32ValueAt(params int[] coordinates)
        {
            var index = GetIndexByCoordinates(coordinates);
            return (int)RawData!.Value.Span[index];
        }

        /// <summary>
        /// Returns the value at the given coordinates as a 16-bit integer
        /// </summary>
        /// <param name="coordinates">coordinates inside the multi dimensional array</param>
        public short GetInt16ValueAt(params int[] coordinates)
        {
            var index = GetIndexByCoordinates(coordinates);
            return (short)RawData!.Value.Span[index];
        }

        /// <summary>
        /// Returns the value at the given coordinates as a 64-bit integer
        /// </summary>
        /// <param name="coordinates">coordinates inside the multi dimensional array</param>
        public long GetInt64ValueAt(params int[] coordinates)
        {
            var index = GetIndexByCoordinates(coordinates);
            return (long)RawData!.Value.Span[index];
        }

        /// <summary>
        /// Returns the value at the given coordinates as a 32-bit float
        /// </summary>
        /// <param name="coordinates">coordinates inside the multi dimensional array</param>
        public float GetFloat32ValueAt(params int[] coordinates)
        {
            var index = GetIndexByCoordinates(coordinates);
            return (float)RawData!.Value.Span[index];
        }

        /// <summary>
        /// Returns the value at the given coordinates as a 64-bit float
        /// </summary>
        /// <param name="coordinates">coordinates inside the multi dimensional array</param>
        public double GetFloat64ValueAt(params int[] coordinates)
        {
            var index = GetIndexByCoordinates(coordinates);
            return (double)RawData!.Value.Span[index];
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
