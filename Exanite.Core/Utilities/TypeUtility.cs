using System;

namespace Exanite.Core.Utilities;

public static class TypeUtility
{
    /// <summary>
    /// Returns true if the type is not abstract or generic.
    /// </summary>
    public static bool IsConcrete(this Type type)
    {
        return !(type.IsAbstract || type.IsGenericType);
    }

    /// <summary>
    /// Gets the default value for the provided <see cref="Type"/>.
    /// </summary>
    public static object? GetDefaultValue(this Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }

        return null;
    }

    /// <inheritdoc cref="FormatConciseName(Type)"/>
    public static string FormatConciseName<T>()
    {
        return FormatConciseName(typeof(T));
    }

    /// <summary>
    /// Returns a value in the format: Type&lt;GenericType&gt;
    /// <br/>
    /// Namespaces will be omitted.
    /// Generics will use the name of their type, instead of the default backtick format.
    /// </summary>
    public static string FormatConciseName(Type type)
    {
        var result = "";

        // Format the type name
        var backtickIndex = type.Name.IndexOf('`');
        result += backtickIndex >= 0 ? type.Name.Substring(0, backtickIndex) : type.Name;

        // Format the generic type parameters
        var generics = type.GetGenericArguments();
        if (generics.Length != 0)
        {
            result += "<";

            var isFirst = true;
            foreach (var generic in generics)
            {
                if (!isFirst)
                {
                    result += ", ";
                }

                isFirst = false;
                result += FormatConciseName(generic);
            }

            result += ">";
        }

        return result;
    }
}
