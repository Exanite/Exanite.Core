using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty(this IEnumerable collection)
        {
            return collection == null || collection.IsEmpty();
        }

        public static bool IsEmpty(this IEnumerable collection)
        {
            return !collection.Cast<object>().Any();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || collection.IsEmpty();
        }

        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return !collection.Any();
        }
    }
}
