using System;
using System.Diagnostics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed128
{
    public static Fixed128 Log2Fast(Fixed128 x)
    {
        if (x <= 0)
        {
            if (x == 0)
            {
                GuardUtility.Throw("Cannot take the logarithm of 0");
            }

            GuardUtility.Throw("Cannot take the logarithm of a negative number");
        }

        var leadingZeroCount = (int)Int128.LeadingZeroCount(x.Raw);
        var distanceToOneBit = leadingZeroCount - (BitCount - 1 - Shift);
        var shiftNormalize = distanceToOneBit;
        var shiftInitial = shiftNormalize;

        var normalizedX = shiftInitial < 0 ? x.Raw >> -shiftInitial : x.Raw << shiftInitial;
        AssertExpectedRange(normalizedX, Shift, 1M, 2M);

        // Calculate index into LUT
        var rawIndex = (normalizedX - OneRaw) << Log2LutBits;
        var index = (int)(rawIndex >> Shift);
        var fraction = (int)(rawIndex & Mask);

        Int128 normalizedResult;
        if (index >= ((1 << Log2LutBits) - 1))
        {
            normalizedResult = OneRaw;
        }
        else
        {
            // Get values and interpolate
            var y0 = Log2Lut[index];
            var y1 = Log2Lut[index + 1];
            normalizedResult = (((Int128)y0 << Shift) + ((Int128)(y1 - y0) * fraction) >> Log2LutShift);
        }

        return new Fixed128(normalizedResult) - shiftNormalize;
    }
}
