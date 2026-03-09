using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed // : IRootFunctions<Fixed>
{
    // public static Fixed Cbrt(Fixed x);
    // public static Fixed Hypot(Fixed x, Fixed y);
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

        // checked // Uncomment when debugging
        {
            // This uses Q96.32 for better precision
            // TODO: 96 is probably extremely excessive since the values are normalized

            // Normalize x using an even shift so that the shift can be safely halved later
            // This leads to x being in the interval [0.5, 2)
            var leadingZeroCount = (int)long.LeadingZeroCount(x.value) + 64;
            var evenNormalizeShift = (leadingZeroCount - (128 - 1 - Shift * 2)) & ~1;
            var normalizedX = evenNormalizeShift >= 0 ? (Int128)x.value << evenNormalizeShift : (Int128)x.value >> -evenNormalizeShift;

            // Calculate LUT index of initial guess
            // 2 is represented with 32 + 2 bits, but we are exclusive of 2
            const int availableBitCount = 32 + 2 - 1;
            var lutIndex = (int)(normalizedX >> (availableBitCount - SqrtLutBits));
            var y = (Int128)SqrtLut[lutIndex - SqrtLutOffset] << Shift;

            while (true)
            {
                // Inverse Newton-Raphson method:
                // y_n+1 = (y_n * (3 - x * y_n * y_n)) >> 1
                var xyy = (normalizedX * y * y) >> (Shift * 4);
                var threeMinusXyy = ((Int128)3 << (Shift * 2)) - xyy;
                var yNext = (y * threeMinusXyy) >> (Shift * 2 + 1);
                if (yNext == y)
                {
                    break;
                }

                y = yNext;
            }

            // We need to cancel out the normalization step we did above
            var normalizedResult = (normalizedX * y) >> (Shift * 2);
            var finalShift = (evenNormalizeShift - Shift) / 2;
            var fixed128Value = finalShift >= 0 ? normalizedResult >> finalShift : normalizedResult << -finalShift;
            return new Fixed((long)(fixed128Value >> Shift));
        }
    }
}
