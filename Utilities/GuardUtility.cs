using System.Diagnostics.CodeAnalysis;
using Exanite.Core.Runtime;

namespace Exanite.Core.Utilities
{
    /// <remarks>
    /// Guards are always enabled. See <see cref="AssertUtility"/>.
    /// </remarks>
    public static class GuardUtility
    {
        public static void IsTrue([DoesNotReturnIf(false)] bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new GuardException(errorMessage);
            }
        }

        public static void IsFalse([DoesNotReturnIf(true)] bool condition, string errorMessage)
        {
            if (condition)
            {
                throw new GuardException(errorMessage);
            }
        }

        public static T NotNull<T>(T? value, string? errorMessage = null) where T : notnull
        {
            IsTrue(value != null, errorMessage ?? "Value was null");

            return value;
        }
    }
}
