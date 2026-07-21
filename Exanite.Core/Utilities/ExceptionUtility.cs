using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Exanite.Core.Utilities;

public static class ExceptionUtility
{
    public static NotSupportedException NotSupportedException<T>(T value)
    {
        return new NotSupportedException($"{value} is not a supported {typeof(T)}.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void NotSupported<T>(T value)
    {
        throw NotSupportedException($"{value} is not a supported {typeof(T)}.");
    }

    /// <remarks>
    /// This overload is provided for cases where this method is used as an expression.
    /// </remarks>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T NotSupported<T>(object value)
    {
        throw NotSupportedException($"{value} is not a supported {typeof(T)}.");
    }
}
