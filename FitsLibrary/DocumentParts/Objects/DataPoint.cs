namespace FitsLibrary.DocumentParts.Objects
{
    public struct DataPoint
    {
        /// <summary>
        /// The coordinates in a dictionary of form [dimension index, index]
        /// </summary>
        public uint[] Coordinates
        {
            get;
        }

        /// <summary>
        /// The value of the point as an object
        /// </summary>
        public object Value { get; set; }

        public DataPoint(uint[] coordinates, object value)
        {
            Coordinates = coordinates!;
            Value = value;
        }
    }
}
