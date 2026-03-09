namespace Exanite.Core.Numerics;

public partial struct Fixed// : ITrigonometricFunctions<Fixed>
{
    // public static Fixed Acos(Fixed x);
    // public static Fixed AcosPi(Fixed x);
    // public static Fixed Asin(Fixed x);
    // public static Fixed AsinPi(Fixed x);
    // public static Fixed Atan(Fixed x);
    // public static Fixed AtanPi(Fixed x);
    // public static Fixed Cos(Fixed x);
    // public static Fixed CosPi(Fixed x);
    // public static Fixed Sin(Fixed x);
    // public static (Fixed Sin, Fixed Cos) SinCos(Fixed x);
    // public static (Fixed SinPi, Fixed CosPi) SinCosPi(Fixed x);
    // public static Fixed SinPi(Fixed x);
    // public static Fixed Tan(Fixed x);
    // public static Fixed TanPi(Fixed x);

    public static Fixed Sin(Fixed x)
    {
        var normalizedX = x.value % Tau.value;
        if (normalizedX < 0)
        {
            normalizedX += Tau.value;
        }

        // Handle negative portion
        var isNegative = normalizedX > Pi.value;
        if (isNegative)
        {
            normalizedX -= Pi.value;
        }

        // Handle mirrored portion
        if (normalizedX > Pi.value / 2)
        {
            normalizedX = Pi.value - normalizedX;
        }

        var rawIndex = (normalizedX << Shift) * (1 << SinLutBits) / (Pi.value / 2);
        var index = (int)(rawIndex >> Shift);
        var fraction = (int)(rawIndex & Mask);
        if (index >= ((1 << SinLutBits) - 1))
        {
            return isNegative ? -One : One;
        }

        var y0 = SinLut[index];
        var y1 = SinLut[index + 1];

        var result = y0 + (((y1 - y0) * fraction) >> Shift);

        return new Fixed(isNegative ? -result : result); // TODO
    }
}
