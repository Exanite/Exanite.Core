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

        // Split 2^x into 2^n * 2^f
        // Where n is the integral part and f is the fractional part
        var n = (int)(x.Raw >> Shift);
        var f = x.Raw & Mask;

        // Use Taylor series of 2^x
        var result = OneRaw;

        var f2 = (f * f) >> Shift;
        var f3 = (f2 * f) >> Shift;
        var f4 = (f2 * f2) >> Shift;

        result += (Exp2Term1 * f) >> Shift;
        result += (Exp2Term2 * f2) >> Shift;
        result += (Exp2Term3 * f3) >> Shift;
        result += (Exp2Term4 * f4) >> Shift;

        // Calculate the final product:
        // result * 2^n
        return new Fixed(n < 0 ? result << n : result >> -n);
    }

}
