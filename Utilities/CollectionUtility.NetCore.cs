using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Exanite.Core.Utilities
{
    public static partial class CollectionUtility
    {
        /// <remarks>
        /// When using the span, be careful not to cause the list's backing array to be replaced.
        /// <para/>
        /// This includes operations such as adding/removing elements and trimming the list.
        /// </remarks>
        public static Span<T> AsSpan<T>(this List<T> list)
        {
            return CollectionsMarshal.AsSpan(list);
        }
    }
}
