using System;
using FitsLibrary.DocumentParts.Objects;

namespace FitsLibrary.DocumentParts
{
    public class Content
    {
        public Memory<DataPoint> Data { get; }

        public Content(Memory<DataPoint> dataPoints)
        {
            this.Data = dataPoints;
        }
    }
}
