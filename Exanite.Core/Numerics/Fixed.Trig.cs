using System;

namespace Exanite.Core.Numerics;

public partial struct Fixed// : ITrigonometricFunctions<Fixed>
{
    /// <summary>
    /// Q4.60 format. Equal to round(tau * 2^60).
    /// </summary>
    // Generated using: (long)decimal.Round((decimal)double.Tau * (1L << 60))
    private const long TauPreciseRaw = 7244019458077126904;
    private const int TauPreciseShift = 60;

    /// <summary>
    /// Q4.60 format. Equal to round(pi * 2^60).
    /// </summary>
    // Generated using: (long)decimal.Round((decimal)double.Pi * (1L << 60))
    private const long PiPreciseRaw = 3622009729038557687;
    private const int PiPreciseShift = 60;

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

    public static Fixed Tan(Fixed x)
    {
        var normalizedX = (long)((((Int128)x.value << (PiPreciseShift - Shift)) % PiPreciseRaw) >> PiPreciseShift - Shift);
        if (normalizedX < 0)
        {
            normalizedX += PiRaw;
        }

        // Handle inverted portion
        var isInverted = normalizedX > PiHalfRaw;
        if (isInverted)
        {
            normalizedX = PiRaw - normalizedX;
        }

        // Precalculate reciprocal so we can multiply instead of divide
        // This also incorporates the shift for the LUT
        // Shifting before dividing is more efficient and precise than shifting after
        const int piHalfReciprocalShift = 32;
        const long piHalfReciprocalRaw = (1L << (TanLutBits + Shift + piHalfReciprocalShift)) / PiHalfRaw;

        // Calculate index into LUT
        var rawIndex = (normalizedX * piHalfReciprocalRaw) >> piHalfReciprocalShift;
        var index = (int)(rawIndex >> Shift);
        var fraction = (int)(rawIndex & Mask);
        if (index >= ((1 << TanLutBits) - 1))
        {
            return isInverted ? MinValue : MaxValue;
        }

        // Get values and interpolate
        var y0 = TanLut[index];
        var y1 = TanLut[index + 1];
        var result = y0 + (((y1 - y0) * fraction) >> Shift);
        return new Fixed(isInverted ? -result : result);
    }

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
