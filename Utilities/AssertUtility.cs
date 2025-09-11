using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Exanite.Core.Runtime;

namespace Exanite.Core.Utilities;

/// <remarks>
/// Asserts are disabled in Release mode. Also see <see cref="GuardUtility"/>.
/// </remarks>
public static class AssertUtility
{
    [Conditional("DEBUG")]
    public static void IsTrue([DoesNotReturnIf(false)] bool condition, string errorMessage)
    {
        if (!condition)
        {
            throw new AssertException(errorMessage);
        }
    }

    [Conditional("DEBUG")]
    public static void IsFalse([DoesNotReturnIf(true)] bool condition, string errorMessage)
    {
        if (condition)
        {
            throw new AssertException(errorMessage);
        }
    }

    public static T NotNull<T>(T? value, string? errorMessage = null) where T : notnull
    {
        IsTrue(value != null, errorMessage ?? "Value was null");

        return value;
    }

    public static T NotNull<T>(T? value, string? errorMessage = null) where T : struct
    {
        IsTrue(value != null, errorMessage ?? "Value was null");

        return value.Value;
    }
}
