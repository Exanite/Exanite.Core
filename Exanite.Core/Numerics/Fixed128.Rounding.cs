using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed128
{
    /// <summary>
    /// Rounds down to the nearest integer.
    /// </summary>
    public static Fixed128 Floor(Fixed128 x)
    {
        return new Fixed128(x.Raw & ~Mask);
    }

    /// <summary>
    /// Rounds up to the nearest integer.
    /// </summary>
    public static Fixed128 Ceiling(Fixed128 x)
    {
        var fractional = x.Raw & Mask;
        return fractional == 0 ? x : Floor(x) + One;
    }

    /// <summary>
    /// Rounds towards zero to the nearest integer.
    /// </summary>
    public static Fixed128 Truncate(Fixed128 x)
    {
        var integral = x.Raw & ~Mask;
        return x.Raw < 0 ? new Fixed128(integral + OneRaw) : new Fixed128(integral);
    }

    /// <summary>
    /// Rounds to the nearest integer using the default rounding strategy (<see cref="MidpointRounding.ToEven"/>).
    /// </summary>
    public static Fixed128 Round(Fixed128 x) => Round(x, MidpointRounding.ToEven);

    /// <summary>
    /// Rounds to the specified number of fractional digits using the default rounding strategy (<see cref="MidpointRounding.ToEven"/>).
    /// </summary>
    public static Fixed128 Round(Fixed128 x, int digits) => Round(x, digits, MidpointRounding.ToEven);

    /// <summary>
    /// Rounds to the specified number of fractional digits using the specified rounding strategy.
    /// </summary>
    public static Fixed128 Round(Fixed128 x, int digits, MidpointRounding mode)
    {
        // Isolate to be in (-2, 2) range to avoid overflow
        var mask = ((Int128)1 << (Shift + 1)) - 1;
        Int128 integral;
        Int128 fractional;
        if (x >= 0)
        {
            integral = x.Raw & ~mask;
            fractional = x.Raw & mask;
        }
        else
        {
            integral = -(-x.Raw & ~mask);
            fractional = -(-x.Raw & mask);
        }

        var exp10 = M.Exp10(digits);
        var fractionalRounded = Round(new Fixed128(fractional * exp10), mode) / exp10;
        return new Fixed128(integral) + new Fixed128(fractionalRounded.Raw);
    }

    /// <summary>
    /// Rounds to the nearest integer using the specified rounding strategy.
    /// </summary>
    public static Fixed128 Round(Fixed128 x, MidpointRounding mode)
    {
        var integral = x.Raw & ~Mask;
        var fractional = x.Raw & Mask;

        if (fractional == 0)
        {
            return x;
        }

        switch (mode)
        {
            case MidpointRounding.ToEven:
            {
                if (fractional < HalfRaw)
                {
                    return new Fixed128(integral);
                }

                if (fractional > HalfRaw)
                {
                    return new Fixed128(integral + OneRaw);
                }

                return IsEvenInteger(new Fixed128(integral)) ? new Fixed128(integral) : new Fixed128(integral + OneRaw);
            }

            case MidpointRounding.AwayFromZero:
            {
                if (fractional < HalfRaw)
                {
                    return new Fixed128(integral);
                }
                if (fractional > HalfRaw)
                {
                    return new Fixed128(integral + OneRaw);
                }

                return x.Raw < 0 ? new Fixed128(integral) : new Fixed128(integral + OneRaw);
            }

            case MidpointRounding.ToZero:
            {
                return x.Raw < 0 ? new Fixed128(integral + OneRaw) : new Fixed128(integral);
            }

            case MidpointRounding.ToNegativeInfinity:
            {
                return new Fixed128(integral);
            }

            case MidpointRounding.ToPositiveInfinity:
            {
                return new Fixed128(integral) + One;
            }

            default: throw ExceptionUtility.NotSupported(mode);
        }
    }
}
