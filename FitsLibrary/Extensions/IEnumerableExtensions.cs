using System;
using System.Collections.Generic;

namespace FitsLibrary.Extensions
{
    public static class IEnumerableExtensions
    {
        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            var array = new TSource[count];
            var i = 0;
            foreach (var item in source)
            {
                array[i++] = item;
            }
            return array;
        }
    }
}
