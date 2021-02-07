using System;
using System.Collections.Generic;
using System.Linq;

namespace FitsLibrary.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static byte[] ReverseFast(this byte[] input)
        {
            var res = new byte[input.Length];
            for (var i = 0; i < res.Length; i++)
            {
                res[i] = input[input.Length - (i + 1)];
            }

            return res;
        }

        /// <summary>
        /// Converts a byte array to big endian if the current system is running on a little Endian hardware
        /// </summary>
        /// <param name="dataToConvert">The data which should get converted</param>
        public static byte[] ConvertLittleEndianToBigEndianIfNecessary(this byte[] dataToConvert)
        {
            return BitConverter.IsLittleEndian
                ? dataToConvert.ReverseFast()
                : dataToConvert;
        }

        /// <summary>
        /// Converts a byte array from big endian to little endian if the current system is running on a little Endian hardware
        /// </summary>
        /// <param name="dataToConvert">The data which should get converted</param>
        public static byte[] ConvertBigEndianToLittleEndianIfNecessary(this byte[] dataToConvert)
        {
            return BitConverter.IsLittleEndian
                ? dataToConvert.ReverseFast()
                : dataToConvert;
        }
    }
}
