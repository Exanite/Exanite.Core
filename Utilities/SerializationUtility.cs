using System;
using System.Text;

namespace Exanite.Core.Utilities
{
    public static class SerializationUtility
    {
        public const string NullSerializedValue = "null";

        /// <summary>
        /// Serialize type into a format loadable by <see cref="Type.GetType()"/>.
        /// This format is similar to <see cref="Type.AssemblyQualifiedName"/>, but only includes type and assembly names.
        /// </summary>
        /// <remarks>
        /// This is probably better implemented by using <see cref="Type.AssemblyQualifiedName"/> and stripping the unnecessary information.
        /// </remarks>
        public static string SerializeType(Type? type)
        {
            if (type == null)
            {
                return NullSerializedValue;
            }

            var builder = new StringBuilder();
            BuildTypeName(type, builder);

            return builder.ToString();
        }

        public static Type? DeserializeType(string value)
        {
            if (value == NullSerializedValue)
            {
                return null;
            }

            var result = Type.GetType(value);
            if (result == null)
            {
                throw new ArgumentException($"Could not deserialize type: '{value}'", nameof(value));
            }

            return result;
        }

        private static void BuildTypeName(Type type, StringBuilder builder)
        {
            if (!string.IsNullOrEmpty(type.Namespace))
            {
                builder.Append(type.Namespace);
                builder.Append(".");
            }

            BuildNestedName(type, builder);
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                builder.Append("[");
                {
                    var isFirst = true;
                    var genericTypes = type.GetGenericArguments();
                    foreach (var genericType in genericTypes)
                    {
                        if (!isFirst)
                        {
                            builder.Append(", ");
                        }

                        isFirst = false;

                        builder.Append("[");
                        builder.Append(SerializeType(genericType));
                        builder.Append("]");
                    }
                }
                builder.Append("]");
            }
            builder.Append(", ");
            builder.Append(type.Assembly.GetName().Name);
        }

        private static void BuildNestedName(Type type, StringBuilder builder)
        {
            if (type.DeclaringType != null)
            {
                BuildNestedName(type.DeclaringType, builder);

                builder.Append("+");
            }

            builder.Append(type.Name);
        }
    }
}
