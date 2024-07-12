using System;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.Core.Utilities
{
    /// <summary>
    /// Caches properties related to <see cref="Enum">Enums</see>.
    /// </summary>
    public static class EnumUtility<T> where T : Enum
    {
        /// <summary>
        /// Array returned by Enum.GetValue(typeof(T)).
        /// </summary>
        public static IReadOnlyList<T> Values { get; }

        /// <summary>
        /// Max value in <typeparamref name="T"/>.
        /// </summary>
        public static int Max { get; }

        /// <summary>
        /// Min value in <typeparamref name="T"/>.
        /// </summary>
        public static int Min { get; }

        /// <summary>
        /// <see cref="Values"/> as a list of strings.
        /// </summary>
        public static IReadOnlyList<string> ValuesAsStrings { get; }

        static EnumUtility()
        {
            Values = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            ValuesAsStrings = Values.Select(x => x.ToString()).ToList();

            var enumerable = Values.Cast<int>();
            Max = enumerable.Max();
            Min = enumerable.Min();
        }
    }
}
