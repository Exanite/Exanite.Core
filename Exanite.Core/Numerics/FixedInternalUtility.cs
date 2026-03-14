using System;
using System.Diagnostics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

internal static class FixedInternalUtility
{
    public static void ThrowOverflowException()
    {
        throw new OverflowException();
    }

    [Conditional("DEBUG")]
    public static void AssertExpectedRange(long x, int shift, decimal inclusiveMin, decimal exclusiveMax)
    {
        AssertUtility.IsFalse((decimal)x / (1L << shift) < inclusiveMin, "Internal: Value is less than the required minimum");
        AssertUtility.IsFalse((decimal)x / (1L << shift) >= exclusiveMax, "Internal: Value is greater than the required maximum");
    }

    [Conditional("DEBUG")]
    public static void AssertExpectedRange(Int128 x, int shift, decimal inclusiveMin, decimal exclusiveMax)
    {
        AssertUtility.IsFalse((decimal)x / (1L << shift) < inclusiveMin, "Internal: Value is less than the required minimum");
        AssertUtility.IsFalse((decimal)x / (1L << shift) >= exclusiveMax, "Internal: Value is greater than the required maximum");
    }
}
