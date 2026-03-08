using System;
using System.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed // : IRootFunctions<Fixed>
{
    // public static Fixed Cbrt(Fixed x)
    // {
    //     throw new System.NotImplementedException();
    // }
    //
    // public static Fixed Hypot(Fixed x, Fixed y)
    // {
    //     throw new System.NotImplementedException();
    // }
    //
    // public static Fixed RootN(Fixed x, int n)
    // {
    //     throw new System.NotImplementedException();
    // }

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
            // This uses Q96.48 for better precision

            // Normalize x so that it is in the range [0.5, 2) // TODO: This is now wrong (maybe?)
            var leadingZeroCount = (int)long.LeadingZeroCount(x.value) + 64;
            var evenNormalizeShift = (leadingZeroCount - (128 - 1 - Shift * 2)) & ~1;
            var normalizedX = evenNormalizeShift >= 0 ? (Int128)x.value << evenNormalizeShift : (Int128)x.value >> -evenNormalizeShift;

            var y = (Int128)1 << (Shift * 2); // TODO: Use LUT
            while (true)
            {
                // Inverse Newton-Raphson method:-4
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
            var finalShift = (evenNormalizeShift - Shift) / 2; // TODO: Figure out why I need the shift here. I think I understand, but it's better to fully verify.
            var fixed128Value = finalShift >= 0 ? normalizedResult >> finalShift : normalizedResult << -finalShift;
            return new Fixed((long)(fixed128Value >> Shift));
        }
    }

    // AI generated code for reference
    // private static readonly uint[] SqrtInverseLut = {
    //     65536, 63554, 61703, 59970, 58344, 56815, 55373, 54011,
    //     52722, 51500, 50341, 49241, 48194, 47198, 46249, 45344,
    //     44481, 43656, 42867, 42111, 41387, 40693, 40026, 39385,
    //     38769, 38176, 37605, 37054, 36523, 36010, 35515, 35036,
    //     34572, 34124, 33690, 33269, 32861, 32466, 32082, 31709,
    //     31347, 30995, 30652, 30318, 29993, 29676, 29367, 29065,
    //     28771, 28484, 28203, 27929, 27660, 27398, 27141, 26889,
    //     26643, 26401, 26165, 25933, 25705, 25482, 25263, 25048
    // };
    //
    // public static Fixed Sqrt(Fixed value)
    // {
    //     long x = value.value; // Assuming private long _value;
    //
    //     if (x <= 0) return x < 0 ? throw new ArithmeticException() : Zero;
    //
    //     // 1. Normalization (Handle dynamic range via CLZ)
    //     int lz = System.Numerics.BitOperations.LeadingZeroCount((ulong)x);
    //     int shiftAmount = lz - (63 - Shift);
    //     long xNorm = shiftAmount >= 0 ? x << shiftAmount : x >> -shiftAmount;
    //
    //     // 2. Initial Guess from LUT (using top bits of normalized value)
    //     // We use 6 bits for the index
    //     int index = (int)((xNorm >> (Shift - 6)) & 0x3F);
    //     long y = SqrtInverseLut[index];
    //
    //     // 3. One NR Iteration: y = y * (3 - x*y*y) / 2
    //     // In fixed point: y = (y * ( (3 << Shift) - ((xNorm * y) >> Shift) * y >> Shift )) >> (Shift + 1)
    //     long xy = (xNorm * y) >> Shift;
    //     long xyy = (xy * y) >> Shift;
    //     long threeMinusXyy = (3L << Shift) - xyy;
    //     y = (y * threeMinusXyy) >> (Shift + 1);
    //
    //     // 4. Final result: sqrt(x) = x * (1/sqrt(x))
    //     long result = (xNorm * y) >> Shift;
    //
    //     // 5. Denormalize (adjust for initial shift)
    //     // Since sqrt(x * 2^n) = sqrt(x) * 2^(n/2)
    //     int finalShift = shiftAmount / 2;
    //     return new Fixed(finalShift >= 0 ? result >> finalShift : result << -finalShift);
    // }
}
