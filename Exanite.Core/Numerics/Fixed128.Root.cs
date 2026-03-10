using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed128
{
    public static Fixed128 Sqrt(Fixed128 x)
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
        // However, note that this was originally designed for Fixed (Q48.16),
        // so the benefits aren't as big for Fixed128 (Q96.32)
        // This must be even
        const int internalShift = 40;

        // Normalize x using an even shift so that the shift can be safely halved later
        // This leads to x being in the interval [0.5, 2)
        var leadingZeroCount = (int)Int128.LeadingZeroCount(x.Raw);
        var evenNormalizeShift = (leadingZeroCount - (128 - 1 - internalShift)) & ~1;
        var normalizedX = evenNormalizeShift >= 0 ? x.Raw << evenNormalizeShift : x.Raw >> -evenNormalizeShift;

        // Calculate LUT index of initial guess
        // 2 is represented with 41 + 2 bits, but we are exclusive of 2
        const int availableBitCount = internalShift + 2 - 1;
        var lutIndex = (int)(normalizedX >> (availableBitCount - SqrtLutBits));
        var y = (Int128)SqrtLut[lutIndex - SqrtLutOffset] << (internalShift - Fixed.Shift);

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
        return new Fixed128(fixed128Value >> (internalShift - Shift));
    }
}
