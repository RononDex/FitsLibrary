
using System;
using System.Collections.Generic;

namespace FitsLibrary.Extensions
{
    public static class StringExtensions
    {
        public static string[] SplitInParts(this string s, int partLength)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));
            }

            var strings = new string[s.Length % partLength == 0 ? s.Length / partLength : s.Length / partLength + 1];
            for (var i = 0; i < strings.Length; i++)
            {
                strings[i] = s.Substring(i * partLength, Math.Min(partLength, s.Length - (i * partLength)));
            }
            return strings;
        }
    }
}
