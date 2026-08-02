using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Exanite.Core.Utilities;

public static class ExceptionUtility
{
    public static NotSupportedException NotSupported<T>(T value)
    {
        return new NotSupportedException($"{value} is not a supported {typeof(T)}.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNotSupported<T>(T value)
    {
        throw NotSupported($"{value} is not a supported {typeof(T)}.");
    }

    /// <remarks>
    /// This overload is provided for cases where this method is used as an expression.
    /// </remarks>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNotSupported<T>(object value)
    {
        throw NotSupported($"{value} is not a supported {typeof(T)}.");
    }
}
