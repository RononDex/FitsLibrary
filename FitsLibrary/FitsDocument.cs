using FitsLibrary.DocumentParts;

namespace FitsLibrary
{
    /// <summary>
    /// Represents a .fits document for read and write access
    /// </summary>
    public class FitsDocument
    {
        /// <summary>
        /// A list of headers in this document
        /// </summary>
        public Header Header { get; }

        /// <summary>
        /// The main data content of the fits file
        /// </summary>
        public Content? Content { get; }

        /// <summary>
        /// Creates a new .fits document with a the given data
        /// </summary>
        /// <param name="header">The main header</param>
        public FitsDocument(
            Header header,
            Content? content)
        {
            Header = header;
            Content = content;
        }
    }
}
