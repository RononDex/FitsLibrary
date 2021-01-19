using System.Collections.Generic;
using FitsLibrary.DocumentParts.Objects;

namespace FitsLibrary.DocumentParts
{
    public class Content
    {
        private readonly List<DataPoint> dataPoints;

        public IReadOnlyList<DataPoint> Data => dataPoints;
    }
}
