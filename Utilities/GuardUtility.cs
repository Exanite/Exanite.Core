using System.Diagnostics.CodeAnalysis;
using Exanite.Core.Runtime;

namespace Exanite.Core.Utilities;

/// <remarks>
/// Guards are always enabled. Also see <see cref="AssertUtility"/>.
/// </remarks>
public static class GuardUtility
{
    [DoesNotReturn]
    public static void Throw(string errorMessage)
    {
        throw new GuardException(errorMessage);
    }

    public static void IsTrue([DoesNotReturnIf(false)] bool condition, string errorMessage)
    {
        if (!condition)
        {
            Throw(errorMessage);
        }
    }

    public static void IsFalse([DoesNotReturnIf(true)] bool condition, string errorMessage)
    {
        if (condition)
        {
            Throw(errorMessage);
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
