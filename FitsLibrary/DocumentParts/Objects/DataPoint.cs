using System;

namespace FitsLibrary.DocumentParts.Objects
{
    public class DataPoint
    {
        /// <summary>
        /// The coordinates in a dictionary of form [dimension index, index]
        /// </summary>
        public ulong[] Coordinates
        {
            get;
        }

        /// <summary>
        /// The value of the point as an object
        /// </summary>
        public object Value { get; set; }

        public DataPoint(ulong[] coordinates, object value)
        {
            Coordinates = coordinates!;
            Value = value;
        }
    }
}
