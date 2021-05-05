using System;

namespace FitsLibrary.DocumentParts
{
    public class Content
    {
        public Memory<object> Data { get; }

        public Content(Memory<object> dataPoints)
        {
            this.Data = dataPoints;
        }
    }
}
