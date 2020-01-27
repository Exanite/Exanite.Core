using System.Collections.Generic;

namespace Exanite.Core.Extensions
{
    /// <summary>
    /// Extension methods for collections
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds a single element to the start of the collection
        /// </summary>
        public static IEnumerable<T> AddToStart<T>(this IEnumerable<T> collection, T element)
        {
            yield return element;

            foreach (var item in collection)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Adds a single element to the end of the collection
        /// </summary>
        public static IEnumerable<T> AddToEnd<T>(this IEnumerable<T> collection, T element)
        {
            foreach (var item in collection)
            {
                yield return item;
            }

            yield return element;
        }
    }
}
