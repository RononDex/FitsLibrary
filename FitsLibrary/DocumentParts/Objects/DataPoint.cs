using System.Collections.Generic;

namespace FitsLibrary.DocumentParts.Objects
{
    public class DataPoint
    {
        /// <summary>
        /// The coordinates in a dictionary of form [dimension index, index]
        /// </summary>
        public IReadOnlyDictionary<uint, ulong> Coordinates
        {
            get;
        }

        /// <summary>
        /// The value of the point as an object
        /// </summary>
        public object Value { get; set; }

        public DataPoint(IReadOnlyDictionary<uint, ulong> coordinates, object value)
        {
            Coordinates = coordinates;
            Value = value;
        }
    }
}
