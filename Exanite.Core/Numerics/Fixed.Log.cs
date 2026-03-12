using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed
{
    public static Fixed Log2(Fixed x)
    {
        if (x <= 0)
        {
            if (x == 0)
            {
                GuardUtility.Throw("Cannot take the logarithm of 0");
            }

            GuardUtility.Throw("Cannot take the logarithm of a negative number");
        }

        var leadingZeroCount = (int)long.LeadingZeroCount(x.Raw);
        var distanceToOneBit = leadingZeroCount - (BitCount - 1 - Shift);
        var shiftNormalize = distanceToOneBit;
        var shiftInitial = shiftNormalize;

        var normalizedX = shiftInitial < 0 ? x.Raw >> -shiftInitial : x.Raw << shiftInitial;
        AssertExpectedRange(normalizedX, 1M, 2M);

        // From https://github.com/asik/FixedMath.Net/blob/b2adac7713eda01fdd31578dd5a1d15f8f7ba067/src/Fix64.cs#L481
        // Iterate for the fraction portion
        var result = -shiftInitial * OneRaw;
        var b = 1L << (Shift - 1);
        var z = normalizedX;
        for (var i = 0; i < Shift; i++)
        {
            z = (z * z) >> Shift;
            if (z >= (OneRaw << 1))
            {
                z >>= 1;
                result |= b;
            }

            b >>= 1;
        }

        return new Fixed(result);
    }

    public static Fixed Log(Fixed x) => Log2(x) * new Fixed(LogETwoRaw);
    public static Fixed Log10(Fixed x) => Log2(x) * new Fixed(Log10TwoRaw);

    // This is very inaccurate
    // Might revisit later
    public static Fixed Log(Fixed x, Fixed newBase) => Log2(x) / Log2(newBase);
}
