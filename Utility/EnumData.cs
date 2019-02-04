using System;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.Utility
{
    public class EnumData<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        /// <summary>
        /// Array returned by Enum.GetValue(typeof(T))
        /// </summary>
        public readonly Array Values;
        /// <summary>
        /// Max value in <typeparamref name="T"/>
        /// </summary>
        public readonly int Max;
        /// <summary>
        /// Min value in T
        /// </summary>
        public readonly int Min;
        /// <summary>
        /// Used for serialization
        /// </summary>
        public readonly List<string> LastEnumValueData;

        private static EnumData<T> instance;
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EnumData<T> Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new EnumData<T>();
                }

                return instance;
            }
        }

        /// <summary>
        /// Creates a new <see cref="EnumData{T}"/>
        /// </summary>
        private EnumData()
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"{typeof(T)} is not an Enum");

            Values = Enum.GetValues(typeof(T));
            IEnumerable<int> enumerable = Values.Cast<int>();
            Max = enumerable.Max();
            Min = enumerable.Min();

            LastEnumValueData = new List<string>();
            foreach (var item in Values)
            {
                LastEnumValueData.Add(item.ToString());
            }
        }

        public static List<string> EnumToStringList<T2>()
        {
            List<string> result = new List<string>();

            foreach (var item in Enum.GetValues(typeof(T2)))
            {
                result.Add(item.ToString());
            }

            return result;
        }
    }
}
