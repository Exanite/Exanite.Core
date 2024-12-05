using System.Diagnostics.CodeAnalysis;
using Exanite.Core.Runtime;

namespace Exanite.Core.Utilities
{
    /// <remarks>
    /// Guards are always enabled. See <see cref="AssertUtility"/>.
    /// </remarks>
    public static class GuardUtility
    {
        public static void IsTrue([DoesNotReturnIf(false)] bool condition, string? message = null)
        {
            if (!condition)
            {
                throw new GuardException(message ?? "Condition was false");
            }
        }

        public static void IsFalse([DoesNotReturnIf(true)] bool condition, string? message = null)
        {
            if (!condition)
            {
                throw new GuardException(message ?? "Condition was true");
            }
        }

        public static T NotNull<T>(T? value, string? message = null) where T : notnull
        {
            IsTrue(value != null, message ?? "Value was null");

            return value;
        }
    }
}
