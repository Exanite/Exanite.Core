using System;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.Core.Helpers
{
    /// <summary>
    /// Contains miscellaneous helper methods for types
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Returns true if the type is not abstract or generic
        /// </summary>
        public static bool IsConcrete(this Type type)
        {
            return !(type.IsAbstract || type.IsGenericType);
        }
    }
}
