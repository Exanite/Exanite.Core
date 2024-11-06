using System.Diagnostics;

namespace Exanite.Core.Utilities
{
    public static class GuardUtility
    {
        public static T AssertNotNull<T>(T? value) where T : notnull
        {
            Debug.Assert(value != null, "value != null");

            return value;
        }
    }
}
