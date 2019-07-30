﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.Core.Helpers
{
    /// <summary>
    /// Holds cached data for an <see cref="Enum"/>
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
        public static readonly long Max;
        /// <summary>
        /// Min value in <typeparamref name="T"/>
        /// </summary>
        public static readonly long Min;
        /// <summary>
        /// <see cref="Values"/> as a <see cref="List"/> of <see langword="string"/>s
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

        static EnumHelper()
        {
            Values = Enum.GetValues(typeof(T));
            IEnumerable<long> enumerable = Values.Cast<long>();
            Max = enumerable.Max();
            Min = enumerable.Min();
        }
    }
}