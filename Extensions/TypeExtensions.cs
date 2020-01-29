using System;

namespace Exanite.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>s
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns true if the type is not abstract or generic
        /// </summary>
        public static bool IsConcrete(this Type type)
        {
            return !(type.IsAbstract || type.IsGenericType);
        }

        /// <summary>
        /// Gets the default value for the provided <see cref="Type"/>
        /// </summary>
        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }
    }
}
