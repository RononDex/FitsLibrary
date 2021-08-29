
using System;

namespace FitsLibrary.DocumentParts
{
    public class Extension
    {
        private readonly Header extensionsHeader;
        private readonly Memory<object>? extensionsData;

        public Extension(Header extensionsHeader, Memory<object>? extensionsData)
        {
            this.extensionsData = extensionsData;
            this.extensionsHeader = extensionsHeader;
        }

        public string ExtensionType
        {
            get
            {
                return (string)extensionsHeader["XTENSION"]!;
            }
        }
    }
}
