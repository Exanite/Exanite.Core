using System;
using System.Collections;
using System.Text;
#if UNITY_2021_3_OR_NEWER
using UnityEngine;
using Object = UnityEngine.Object;
#endif

namespace Exanite.Core.Utilities
{
    public static class DebugUtility
    {
        /// <summary>
        /// Called when <see cref="Log"/> is called. Defaults to <see cref="DefaultLogAction"/>.
        /// <para/>
        /// This can be replaced to allow custom logging implementations to be used.
        /// </summary>
        public static Action<object> LogAction = DefaultLogAction;

        /// <summary>
        /// Logs the value using the current <see cref="LogAction"/>. Defaults to <see cref="DefaultLogAction"/>.
        /// </summary>
        public static void Log(object value)
        {
            LogAction?.Invoke(value);
        }

        /// <summary>
        /// Logs the <paramref name="name"/> and <paramref name="value"/>
        /// formatted as 'name: value' to the Unity console.
        /// </summary>
        public static void LogVariable(string name, object value)
        {
            Log($"{name}: {value}");
        }

        /// <summary>
        /// Uses <see cref="Format"/> to format the provided value and logs it.
        /// <para/>
        /// The provided value is returned to allow <see cref="Dump{T}"/>
        /// to be inserted in the middle of statements for convenience.
        /// </summary>
        public static T Dump<T>(this T value)
        {
            Log(Format(value));

            return value;
        }

        /// <summary>
        /// Formats collections (and IEnumerables) in an array-like format.
        /// </summary>
        public static string Format(object value)
        {
            if (value == null)
            {
                return "Null";
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                var stringBuilder = new StringBuilder();
                FormatEnumerable(enumerable, stringBuilder);

                return stringBuilder.ToString();
            }

            return value.ToString();
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

        /// <summary>
        /// Logs the value using Debug.Log on Unity and Console.WriteLine elsewhere.
        /// </summary>
        public static void DefaultLogAction(object value)
        {
#if UNITY_2021_3_OR_NEWER
            if (value is Object context)
            {
                // If provided a context object, clicking on the log message will select that object
                Debug.Log(value, context);
            }
            else
            {
                Debug.Log(value);
            }
#else
            Console.WriteLine(value);
#endif
        }
    }
}
