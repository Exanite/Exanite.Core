using System.Diagnostics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed// : ILogarithmicFunctions<Fixed>
{
    public static Fixed Log2(Fixed x)
    {
        var leadingZeroCount = (int)long.LeadingZeroCount(x.Raw);
        var distanceToOneBit = leadingZeroCount - (BitCount - 1 - Shift);
        var shiftNormalize = distanceToOneBit;
        var shiftInitial = shiftNormalize;

        var normalizedX = shiftInitial < 0 ? x.Raw >> -shiftInitial : x.Raw << shiftInitial;
        AssertExpectedRange(normalizedX, 1M, 2M);

        // Calculate index into LUT
        var rawIndex = (normalizedX - OneRaw) << Log2LutBits;
        var index = (int)(rawIndex >> Shift);
        var fraction = (int)(rawIndex & Mask);

        long normalizedResult;
        if (index >= ((1 << Log2LutBits) - 1))
        {
            normalizedResult = OneRaw;
        }
        else
        {
            // Get values and interpolate
            var y0 = Log2Lut[index];
            var y1 = Log2Lut[index + 1];
            normalizedResult = y0 + (((y1 - y0) * fraction) >> Shift);
        }

        return new Fixed(normalizedResult) - shiftNormalize;
    }

    //public static Fixed Log(Fixed x);
    //public static Fixed Log(Fixed x, Fixed newBase);
    //public static Fixed Log10(Fixed x);

    [Conditional("DEBUG")]
    private static void AssertExpectedRange(long x, decimal inclusiveMin, decimal exclusiveMax)
    {
        AssertUtility.IsFalse((decimal)x / (1L << Shift) < inclusiveMin, "Internal: Value is less than the required minimum");
        AssertUtility.IsFalse((decimal)x / (1L << Shift) >= exclusiveMax, "Internal: Value is greater than the required maximum");
    }
}
