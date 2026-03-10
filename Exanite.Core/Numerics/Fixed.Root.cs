using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed // : IRootFunctions<Fixed>
{
    // public static Fixed Cbrt(Fixed x);
    // public static Fixed RootN(Fixed x, int n);

    public static Fixed Sqrt(Fixed x)
    {
        if (x < 0)
        {
            GuardUtility.Throw("Cannot take the square root of a negative number");
        }

        if (x == 0)
        {
            return 0;
        }

        // This uses Q88.40 for better precision
        // This must be even
        const int internalShift = 40;

        // Normalize x using an even shift so that the shift can be safely halved later
        // This leads to x being in the interval [0.5, 2)
        var leadingZeroCount = (int)long.LeadingZeroCount(x.raw) + 64;
        var evenNormalizeShift = (leadingZeroCount - (128 - 1 - internalShift)) & ~1;
        var normalizedX = evenNormalizeShift >= 0 ? (Int128)x.raw << evenNormalizeShift : (Int128)x.raw >> -evenNormalizeShift;

        // Calculate LUT index of initial guess
        // 2 is represented with 41 + 2 bits, but we are exclusive of 2
        const int availableBitCount = internalShift + 2 - 1;
        var lutIndex = (int)(normalizedX >> (availableBitCount - SqrtLutBits));
        var y = (Int128)SqrtLut[lutIndex - SqrtLutOffset] << (internalShift - Shift);

        var three = (Int128)3 << internalShift;
        while (true)
        {
            // Inverse Newton-Raphson method:
            // y_n+1 = (y_n * (3 - x * y_n * y_n)) >> 1
            var xyy = (normalizedX * y * y) >> (internalShift * 2);
            var threeMinusXyy = three - xyy;
            var yNext = (y * threeMinusXyy) >> (internalShift + 1);
            if (yNext == y)
            {
                break;
            }

            y = yNext;
        }

        // We need to cancel out the normalization step we did above
        var normalizedResult = (normalizedX * y) >> internalShift;
        var finalShift = (evenNormalizeShift - (internalShift - Shift)) / 2;
        var fixed128Value = finalShift >= 0 ? normalizedResult >> finalShift : normalizedResult << -finalShift;
        return new Fixed((long)(fixed128Value >> (internalShift - Shift)));
    }

    public static Fixed Hypot(Fixed x, Fixed y)
    {
        var x2 = (Int128)x.raw * x.raw;
        var y2 = (Int128)y.raw * y.raw;
        var sum = (x2 + y2) >> Shift;
        return Sqrt(new Fixed((long)sum));
    }

    public static Fixed Hypot(Fixed x, Fixed y, Fixed z)
    {
        var x2 = (Int128)x.raw * x.raw;
        var y2 = (Int128)y.raw * y.raw;
        var z2 = (Int128)z.raw * z.raw;
        var sum = (x2 + y2 + z2) >> Shift;
        return Sqrt(new Fixed((long)sum));
    }

    public static Fixed Hypot(Fixed x, Fixed y, Fixed z, Fixed w)
    {
        var x2 = (Int128)x.raw * x.raw;
        var y2 = (Int128)y.raw * y.raw;
        var z2 = (Int128)z.raw * z.raw;
        var w2 = (Int128)w.raw * w.raw;
        var sum = (x2 + y2 + z2 + w2) >> Shift;
        return Sqrt(new Fixed((long)sum));
    }
}
