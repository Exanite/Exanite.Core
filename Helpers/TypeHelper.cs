using System;

namespace Exanite.Core.Helpers
{
    /// <summary>
    /// Helper class for types
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
