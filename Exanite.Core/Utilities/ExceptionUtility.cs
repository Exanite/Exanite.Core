using System;

namespace Exanite.Core.Utilities;

public static class ExceptionUtility
{
    public static NotSupportedException NotSupported<T>(T value)
    {
        return new NotSupportedException($"{value} is not a supported {typeof(T)}.");
    }
}
