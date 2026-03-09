using System;

namespace Exanite.Core.Numerics;

public partial struct Fixed// : ITrigonometricFunctions<Fixed>
{
    // Q4.60
    private const long TauPreciseRaw = 7244019458077126904;
    private const int TauPreciseShift = 60;

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

    public static Fixed Cos(Fixed x)
    {
        return Sin(x + PiHalf);
    }

    public static Fixed Sin(Fixed x)
    {
        var normalizedX = (long)((((Int128)x.value << (TauPreciseShift - Shift)) % TauPreciseRaw) >> TauPreciseShift - Shift);
        if (normalizedX < 0)
        {
            normalizedX += TauRaw;
        }

        // Handle negative portion
        var isNegative = normalizedX > PiRaw;
        if (isNegative)
        {
            normalizedX -= PiRaw;
        }

        // Handle mirrored portion
        if (normalizedX > PiHalfRaw)
        {
            normalizedX = PiRaw - normalizedX;
        }

        // Precalculate reciprocal so we can multiply instead of divide
        // This also incorporates the shift for the LUT
        // Shifting before dividing is more efficient and precise than shifting after
        const int piHalfReciprocalShift = 32;
        const long piHalfReciprocalRaw = (1L << (SinLutBits + Shift + piHalfReciprocalShift)) / PiHalfRaw;

        // Calculate index into LUT
        var rawIndex = (normalizedX * piHalfReciprocalRaw) >> piHalfReciprocalShift;
        var index = (int)(rawIndex >> Shift);
        var fraction = (int)(rawIndex & Mask);
        if (index >= ((1 << SinLutBits) - 1))
        {
            return isNegative ? -One : One;
        }

        // Get values and interpolate
        var y0 = SinLut[index];
        var y1 = SinLut[index + 1];
        var result = y0 + (((y1 - y0) * fraction) >> Shift);
        return new Fixed(isNegative ? -result : result);
    }
}
