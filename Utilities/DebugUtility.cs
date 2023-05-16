using System.Collections;
using System.Text;
using UnityEngine;

namespace Exanite.Core.Utilities
{
    public static class DebugUtility
    {
        /// <summary>
        ///     Logs the <paramref name="name"/> and <paramref name="value"/>
        ///     formatted as 'name: value' to the Unity console.
        /// </summary>
        public static void LogVariable(string name, object value)
        {
            Debug.Log($"{name}: {value}");
        }

        /// <summary>
        ///     Logs values and lists (and IEnumerables) in an array-like format.
        /// </summary>
        public static T Dump<T>(this T value)
        {
            if (value is IEnumerable enumerable && value is not string)
            {
                DumpEnumerable(enumerable);

                return value;
            }

            Log(value);

            return value;
        }

        private static void DumpEnumerable(this IEnumerable enumerable)
        {
            var stringBuilder = new StringBuilder();
            FormatEnumerable(enumerable, stringBuilder);

            Log(stringBuilder.ToString());
        }

        private static void FormatEnumerable(IEnumerable enumerable, StringBuilder stringBuilder)
        {
            stringBuilder.Append("[");

            var isFirst = true;

            foreach (var value in enumerable)
            {
                if (!isFirst)
                {
                    stringBuilder.Append(", ");
                }

                if (value is IEnumerable nestedEnumerable && value is not string)
                {
                    FormatEnumerable(nestedEnumerable, stringBuilder);
                }
                else
                {
                    stringBuilder.Append(value);
                }

                isFirst = false;
            }

            stringBuilder.Append("]");
        }

        private static void Log(object value)
        {
            if (value is Object context)
            {
                // If provided a context object, clicking on the log message will select that object
                Debug.Log(value, context);
            }
            else
            {
                Debug.Log(value);
            }
        }
    }
}
