using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apicalypse.Extensions
{
    /// <summary>
    /// Extension methods for IEnumerable
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Checks if all values in source can be found in value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source array</param>
        /// <param name="value">The array where to search for source values</param>
        /// <returns></returns>
        public static bool IsContainedIn<T>(this IEnumerable<T> source, IEnumerable<T> value)
        {
            foreach (var sourceValue in source)
                if (!value.Contains(sourceValue))
                    return false;

            return true;
        }

        public static bool IsAnyIn<T>(this IEnumerable<T> source, IEnumerable<T> value)
        {
            return source.Any(sourceValue => value.Contains(sourceValue));
        }
    }
}
