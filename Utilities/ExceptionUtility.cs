using System;

namespace Exanite.Core.Utilities
{
    public static class ExceptionUtility
    {
        public static NotSupportedException NotSupportedEnumValue<T>(T value) where T : Enum
        {
            return new NotSupportedException($"{value} is not a supported {typeof(T)}.");
        }
    }
}