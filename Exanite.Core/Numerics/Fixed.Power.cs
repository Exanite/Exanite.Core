using System;

namespace Exanite.Core.Numerics;

public partial struct Fixed
{
    public static Fixed Pow(Fixed x, Fixed y)
    {
        if (IsInteger(y))
        {
            switch ((int)y)
            {
                case -3:
                {
                    // xx and xxx are Q(Shift * 2)
                    const int xxShift = Shift * 2;
                    var xx = (Int128)x.Raw * x.Raw;
                    var xxx = (xx >> Shift) * x.Raw;

                    const int oneShift = 126;
                    var result = (((Int128)1 << oneShift) / xxx) >> (oneShift - xxShift - Shift);
                    return new Fixed((long)result);
                }
                case -2:
                {
                    // xx is Q(Shift * 2)
                    const int xxShift = Shift * 2;
                    var xx = (Int128)x.Raw * x.Raw;

                    const int oneShift = 126;
                    var result = (((Int128)1 << oneShift) / xx) >> (oneShift - xxShift - Shift);
                    return new Fixed((long)result);
                }
                case -1:
                {
                    const int oneShift = 126;
                    var result = (((Int128)1 << oneShift) / x.Raw) >> (oneShift - Shift - Shift);
                    return new Fixed((long)result);
                }
                case 0:
                {
                    return 1;
                }
                case 1:
                {
                    return x;
                }
                case 2:
                {
                    return checked(x * x);
                }
                case 3:
                {
                    checked
                    {
                        // xx and xxx are Q32
                        var xx = (Int128)x.Raw * x.Raw;
                        var xxx = (xx >> Shift) * x.Raw;
                        var result = xxx >> Shift;
                        return new Fixed((long)result);
                    }
                }
            }
        }

        // Use identity:
        // x^y = 2^(y * log2(x))
        return Exp2(y * Log2(x));
    }

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
        var f = (Int128)(x.Raw & Mask) << (internalShift - Shift);

        // Use Taylor series of 2^x
        var f2 = (f * f) >> internalShift;
        var f3 = (f2 * f) >> internalShift;
        var f4 = (f2 * f2) >> internalShift;
        var f5 = (f3 * f2) >> internalShift;
        var f6 = (f3 * f3) >> internalShift;

        // Fractional results are stored as Q84
        Int128 fractionalResult = 0;
        fractionalResult += Exp2Term1 * f;
        fractionalResult += Exp2Term2 * f2;
        fractionalResult += Exp2Term3 * f3;
        fractionalResult += Exp2Term4 * f4;
        fractionalResult += Exp2Term5 * f5;
        fractionalResult += Exp2Term6 * f6;

        // Result is stored as Q42
        var result = ((Int128)1 << internalShift) + (fractionalResult >> Exp2TermShift);

        // Calculate the final product:
        // result * 2^n
        var finalProduct = n < 0 ? result >> -n : result << n;
        return new Fixed((long)(finalProduct >> (internalShift - Shift)));
    }
}
