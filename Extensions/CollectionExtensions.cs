using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.Core.Extensions
{
    /// <summary>
    /// Contains miscellaneous extension methods for collections
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns if the collection is null or empty
        /// </summary>
        public static bool IsNullOrEmpty(this IEnumerable collection)
        {
            return collection == null || collection.IsEmpty();
        }

        /// <summary>
        /// Returns if the collection is empty
        /// </summary>
        /// <exception cref="System.NullReferenceException"></exception>
        public static bool IsEmpty(this IEnumerable collection)
        {
            return !collection.Cast<object>().Any();
        }

        /// <summary>
        /// Returns if the collection is null or empty
        /// </summary>
        /// <exception cref="System.NullReferenceException"></exception>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || collection.IsEmpty();
        }

        /// <summary>
        /// Returns if the collection is empty
        /// </summary>
        /// <exception cref="System.NullReferenceException"></exception>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return !collection.Any();
        }

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
