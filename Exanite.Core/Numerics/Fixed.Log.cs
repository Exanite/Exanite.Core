using System.Diagnostics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed
{
    public static Fixed Log2(Fixed x)
    {
        return (Fixed)Fixed128.Log2Fast(x);
    }

    public static Fixed Log(Fixed x) => Log2(x) * new Fixed(LogETwoRaw);
    public static Fixed Log10(Fixed x) => Log2(x) * new Fixed(Log10TwoRaw);

    // This is very inaccurate
    // Might revisit later
    public static Fixed Log(Fixed x, Fixed newBase) => Log2(x) / Log2(newBase);

    [Conditional("DEBUG")]
    private static void AssertExpectedRange(long x, decimal inclusiveMin, decimal exclusiveMax)
    {
        AssertUtility.IsFalse((decimal)x / (1L << Shift) < inclusiveMin, "Internal: Value is less than the required minimum");
        AssertUtility.IsFalse((decimal)x / (1L << Shift) >= exclusiveMax, "Internal: Value is greater than the required maximum");
    }
}
