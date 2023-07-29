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
        private static List<string> valuesAsStringList;

        /// <summary>
        /// Array returned by Enum.GetValue(typeof(T)).
        /// </summary>
        public static readonly Array Values;

        /// <summary>
        /// Max value in <typeparamref name="T"/>.
        /// </summary>
        public static readonly int Max;

        /// <summary>
        /// Min value in <typeparamref name="T"/>.
        /// </summary>
        public static readonly int Min;

        /// <summary>
        /// <see cref="Values"/> as a <see cref="List"/> of <see langword="string">strings</see>.
        /// </summary>
        public static IReadOnlyList<string> ValuesAsStringList
        {
            get
            {
                if (valuesAsStringList == null)
                {
                    valuesAsStringList = new List<string>();
                    foreach (var item in Values)
                    {
                        valuesAsStringList.Add(item.ToString());
                    }
                }

                return valuesAsStringList;
            }
        }

        static EnumUtility()
        {
            Values = Enum.GetValues(typeof(T));
            var enumerable = Values.Cast<int>();
            Max = enumerable.Max();
            Min = enumerable.Min();
        }
    }
}
