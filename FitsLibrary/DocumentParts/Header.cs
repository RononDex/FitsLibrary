using System.Collections.Generic;
using System.Linq;
using FitsLibrary.DocumentParts.Objects;

namespace FitsLibrary.DocumentParts
{
    public class Header
    {
        private readonly IList<HeaderEntry> _entries;

        /// <summary>
        /// A list of entries contained within the header
        /// </summary>
        public IReadOnlyList<HeaderEntry> Entries { get; }

        /// <summary>
        /// Gets the entry with the given key.
        /// Returns null if no entry exists with the given key
        /// </summary>
        /// <param name="key"></param>
        public string? this[string key]
        {
            get
            {
                if (_entries.Any(entry => string.Equals(entry.Key, key, System.StringComparison.Ordinal)))
                {
                    return _entries.Single(entry =>
                        string.Equals(entry.Key, key, System.StringComparison.Ordinal)).Value;
                }

                return null;
            }
        }

        /// <summary>
        /// Initializes a header with the given entries
        /// </summary>
        /// <param name="entries">A list of entries used to initialize the
        /// header</param>
        public Header(IList<HeaderEntry> entries)
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
    }
}
