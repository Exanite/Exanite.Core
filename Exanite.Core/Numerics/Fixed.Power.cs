using System;

namespace Exanite.Core.Numerics;

public partial struct Fixed// : IPowerFunctions<Fixed>
{
    // public static Fixed Pow(Fixed x, Fixed y);

    public static Fixed Exp2(Fixed x)
    {
        if (x == 0)
        {
            return One;
        }

        if (x >= IntegralBitCount)
        {
            // Anything bigger than 2^47 will overflow
            return MaxValue;
        }

        if (x <= -(FractionalBitCount + 1))
        {
            // Anything smaller than 2^(-17) will result in 0
            return Zero;
        }

        // Use Q86.42 for intermediate math to avoid precision loss
        const int internalShift = 42;

        // Split 2^x into 2^n * 2^f
        // Where n is the integral part and f is the fractional part
        var n = (int)(x.Raw >> Shift);
        var f = (Int128)(x.Raw & Mask) << (internalShift - Shift); // Q42

        // Use Taylor series of 2^x
        var result = (Int128)1 << internalShift;

        var f2 = (f * f) >> internalShift;
        var f3 = (f2 * f) >> internalShift;
        var f4 = (f2 * f2) >> internalShift;
        var f5 = (f3 * f2) >> internalShift;

        // Terms are Q16
        result += (Exp2Term1 * f) >> Shift;
        result += (Exp2Term2 * f2) >> Shift;
        result += (Exp2Term3 * f3) >> Shift;
        result += (Exp2Term4 * f4) >> Shift;
        result += (Exp2Term5 * f5) >> Shift;

        // Calculate the final product:
        // result * 2^n
        var finalProduct = n < 0 ? result >> -n : result << n;
        return new Fixed((long)(finalProduct >> (internalShift - Shift)));
    }
}
