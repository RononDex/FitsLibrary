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

        public object Value { get; set; }
    }
}
