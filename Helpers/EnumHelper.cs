using System;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.Core.Helpers
{
    /// <summary>
    /// Static class used to hold different types of data about an <see cref="Enum"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class EnumHelper<T> where T : Enum
    {
        private static List<string> valuesAsStringList;

        /// <summary>
        /// Array returned by Enum.GetValue(typeof(T))
        /// </summary>
        public static readonly Array Values;
        /// <summary>
        /// Max value in <typeparamref name="T"/>
        /// </summary>
        public static readonly int Max;
        /// <summary>
        /// Min value in <typeparamref name="T"/>
        /// </summary>
        public static readonly int Min;
        /// <summary>
        /// <see cref="Values"/> as a <see cref="List"/> of <see langword="string"/>s
        /// </summary>
        public static List<string> ValuesAsStringList
        {
            get
            {
                if (ValuesAsStringList == null)
                {
                    valuesAsStringList = new List<string>();
                    foreach (var item in Values)
                    {
                        ValuesAsStringList.Add(item.ToString());
                    }
                }

                return valuesAsStringList;
            }
        }

        static EnumHelper()
        {
            Values = Enum.GetValues(typeof(T));
            IEnumerable<int> enumerable = Values.Cast<int>();
            Max = enumerable.Max();
            Min = enumerable.Min();
        }

        public static List<string> EnumToStringList()
        {
            var result = new List<string>();

            foreach (var item in Values)
            {
                result.Add(item.ToString());
            }

            return result;
        }
    }
}
