using System;
using System.Runtime.CompilerServices;

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

    // public static Fixed Tan(Fixed x);
    // public static Fixed Cos(Fixed x);
    // public static Fixed Sin(Fixed x);

    // public static Fixed TanPi(Fixed x);
    // public static Fixed CosPi(Fixed x);
    // public static Fixed SinPi(Fixed x);

    // public static (Fixed Sin, Fixed Cos) SinCos(Fixed x);
    // public static (Fixed SinPi, Fixed CosPi) SinCosPi(Fixed x);

    // public static Fixed Atan(Fixed x);
    // public static Fixed Acos(Fixed x);
    // public static Fixed Asin(Fixed x);

    // public static Fixed AtanPi(Fixed x);
    // public static Fixed AcosPi(Fixed x);
    // public static Fixed AsinPi(Fixed x);

    // TODO: Also do Atan2 since it's useful

    public static Fixed Sin(Fixed x)
    {
        var normalizedX = WrapToTauRange(x.raw);
        return new Fixed(SinFromNormalized(normalizedX));
    }

    public static Fixed SinPi(Fixed x)
    {
        var normalizedX = WrapToTwoRange(x.raw);
        return new Fixed(SinPiFromNormalized(normalizedX));
    }

    public static Fixed Cos(Fixed x)
    {
        return Sin(x + PiHalf);
    }

    public static Fixed CosPi(Fixed x)
    {
        return SinPi(x + Half);
    }

    public static Fixed Tan(Fixed x)
    {
        var normalizedX = WrapToPiRange(x.raw);

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
        const long piHalfReciprocalRaw = (1L << (TanLutBits + Shift + piHalfReciprocalShift)) / PiHalfRaw; // Q(TanLutBits + piHalfReciprocalShift)

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

    public static Fixed TanPi(Fixed x)
    {
        var normalizedX = WrapToOneRange(x.raw);

        // Handle inverted portion
        var isInverted = normalizedX > HalfRaw;
        if (isInverted)
        {
            normalizedX = OneRaw - normalizedX;
        }

        // Calculate index into LUT
        var rawIndex = normalizedX << (TanLutBits + 1);
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

    public static (Fixed Sin, Fixed Cos) SinCos(Fixed x)
    {
        var sinNormalizedX = WrapToTauRange(x.raw);
        var sinRaw = SinFromNormalized(sinNormalizedX);

        var cosNormalizedX = sinNormalizedX + PiHalfRaw;
        if (cosNormalizedX >= TauRaw)
        {
            cosNormalizedX -= TauRaw;
        }
        var cosRaw = SinFromNormalized(cosNormalizedX);

        return (new Fixed(sinRaw), new Fixed(cosRaw));
    }

    public static (Fixed SinPi, Fixed CosPi) SinCosPi(Fixed x)
    {
        var sinNormalizedX = WrapToTwoRange(x.raw);
        var sinRaw = SinPiFromNormalized(sinNormalizedX);

        var cosNormalizedX = sinNormalizedX + HalfRaw;
        if (cosNormalizedX >= TwoRaw)
        {
            cosNormalizedX -= TwoRaw;
        }
        var cosRaw = SinPiFromNormalized(cosNormalizedX);

        return (new Fixed(sinRaw), new Fixed(cosRaw));
    }

    public static Fixed Atan(Fixed x)
    {
        var isNegative = x.raw < 0;
        var absX = Abs(x);

        long result;
        if (absX <= One)
        {
            result = AtanFromNormalized(absX.raw);
        }
        else
        {
            // Use identity: atan(x) = pi/2 - atan(1/x)
            result = PiHalfRaw - AtanFromNormalized((One / absX).raw);
        }

        return new Fixed(isNegative ? -result : result);
    }

    /// <summary>
    /// Reduces the input value to be in the range [0, 1).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long WrapToOneRange(long x)
    {
        return x & Mask;
    }

    /// <summary>
    /// Reduces the input value to be in the range [0, 2).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long WrapToTwoRange(long x)
    {
        return x & TwoMask;
    }

    /// <summary>
    /// Reduces the input value to be in the range [0, pi).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long WrapToPiRange(long x)
    {
        var normalizedX = (long)((((Int128)x << (PiPreciseShift - Shift)) % PiPreciseRaw) >> PiPreciseShift - Shift);
        if (normalizedX < 0)
        {
            normalizedX += PiRaw;
        }

        return normalizedX;
    }

    /// <summary>
    /// Reduces the input value to be in the range [0, 2pi).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long WrapToTauRange(long x)
    {
        var normalizedX = (long)((((Int128)x << (TauPreciseShift - Shift)) % TauPreciseRaw) >> TauPreciseShift - Shift);
        if (normalizedX < 0)
        {
            normalizedX += TauRaw;
        }

        return normalizedX;
    }

    /// <summary>
    /// Calculates the sine of the provided value.
    /// The provided value must be in the range [0, 2pi).
    /// See <see cref="WrapToTauRange"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long SinFromNormalized(long normalizedX)
    {
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
        const long piHalfReciprocalRaw = (1L << (SinLutBits + Shift + piHalfReciprocalShift)) / PiHalfRaw; // Q(SinLutBits + piHalfReciprocalShift)

        // Calculate index into LUT
        var rawIndex = (normalizedX * piHalfReciprocalRaw) >> piHalfReciprocalShift;
        var index = (int)(rawIndex >> Shift);
        var fraction = (int)(rawIndex & Mask);
        if (index >= ((1 << SinLutBits) - 1))
        {
            return isNegative ? -OneRaw : OneRaw;
        }

        // Get values and interpolate
        var y0 = SinLut[index];
        var y1 = SinLut[index + 1];
        var result = y0 + (((y1 - y0) * fraction) >> Shift);
        return isNegative ? -result : result;
    }

    /// <summary>
    /// Calculates the sine of the provided value after multiplying by pi.
    /// The provided value must be in the range [0, 2)
    /// See <see cref="WrapToTwoRange"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long SinPiFromNormalized(long normalizedX)
    {
        // Handle negative portion
        var isNegative = normalizedX > OneRaw;
        if (isNegative)
        {
            normalizedX -= OneRaw;
        }

        // Handle mirrored portion
        if (normalizedX > HalfRaw)
        {
            normalizedX = OneRaw - normalizedX;
        }

        // Calculate index into LUT
        // We do this by simply using the top bits of x as the index and the rest as the fraction
        var rawIndex = normalizedX << (SinLutBits + 1);
        var index = (int)(rawIndex >> Shift);
        var fraction = (int)(rawIndex & Mask);
        if (index >= ((1 << SinLutBits) - 1))
        {
            return isNegative ? -OneRaw : OneRaw;
        }

        // Get values and interpolate
        var y0 = SinLut[index];
        var y1 = SinLut[index + 1];
        var result = y0 + (((y1 - y0) * fraction) >> Shift);
        return isNegative ? -result : result;
    }

    /// <summary>
    /// Calculates the arctangent of the provided value.
    /// The provided value must be in the range [0, 1].
    /// </summary>
    private static long AtanFromNormalized(long x)
    {
        // x is guaranteed [0, 1] here
        // Map [0, 1] to AtanLut
        var rawIndex = (x << AtanLutBits);
        var index = (int)(rawIndex >> Shift);
        var fraction = (int)(rawIndex & Mask);

        if (index >= ((1 << AtanLutBits) - 1))
        {
            return PiFourthRaw;
        }

        var y0 = AtanLut[index];
        var y1 = AtanLut[index + 1];
        var result = y0 + (((y1 - y0) * fraction) >> Shift);
        return result;
    }
}
