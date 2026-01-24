using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Exanite.Core.Runtime;

namespace Exanite.Core.Utilities;

/// <remarks>
/// Asserts are disabled in Release mode. Also see <see cref="GuardUtility"/>.
/// </remarks>
public static class AssertUtility
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Throw(string errorMessage)
    {
        throw new AssertException(errorMessage);
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsTrue([DoesNotReturnIf(false)] bool condition, string errorMessage)
    {
        if (!condition)
        {
            Throw(errorMessage);
        }
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsFalse([DoesNotReturnIf(true)] bool condition, string errorMessage)
    {
        if (condition)
        {
            Throw(errorMessage);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(T? value, string? errorMessage = null) where T : notnull
    {
        IsTrue(value != null, errorMessage ?? "Value was null");

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(T? value, string? errorMessage = null) where T : struct
    {
        IsTrue(value != null, errorMessage ?? "Value was null");

        return value.Value;
    }
}
