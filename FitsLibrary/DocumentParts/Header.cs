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
        /// A list of entries contained within the header
        /// </summary>
        public IList<HeaderEntry> Entries => _entries;

        public object? this[string key]
        {
            get => _entries.Find(entry => string.Equals(entry.Key, key, System.StringComparison.Ordinal));
        }
    }
}
