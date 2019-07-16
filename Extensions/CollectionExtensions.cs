using System;
using System.Collections;

namespace Exanite.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }

        public static bool IsNullOrEmpty(this IList list)
        {
            return (list == null || list.Count == 0);
        }
    }
}
