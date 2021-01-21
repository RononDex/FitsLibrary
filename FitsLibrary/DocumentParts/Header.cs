using System.Collections.Generic;
using FitsLibrary.DocumentParts.Objects;

namespace FitsLibrary.DocumentParts
{
    public class Header
    {
        private readonly List<HeaderEntry> _entries;

        /// <summary>
        /// Initializes a header with the given entries
        /// </summary>
        /// <param name="entries">A list of entries used to initialize the
        /// header</param>
        public Header(List<HeaderEntry> entries)
        {
            _entries = entries;
        }

        /// <summary>
        /// Initializes an empty header
        /// </summary>
        public Header()
        {
            _entries = new List<HeaderEntry>();
        }

        /// <summary>
        /// Returns the type of the data (integer, float, etc)
        /// </summary>
        public DataContentType DataContentType => (DataContentType)this["BITPIX"]!;

        /// <summary>
        /// A list of entries contained within the header
        /// </summary>
        public IList<HeaderEntry> Entries => _entries;

        /// <summary>
        /// Gets the value for the given header entry.
        /// Returns only first found entry if there are multiple with the same key
        /// </summary>
        /// <param name="key">The key for which to search in the header</param>
        public object? this[string key] =>
            _entries.Find(entry => string.Equals(entry.Key, key, System.StringComparison.Ordinal))?.Value;
    }
}
