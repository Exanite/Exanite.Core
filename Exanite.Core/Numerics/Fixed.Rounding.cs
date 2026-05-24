using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed
{
    /// <summary>
    /// Rounds down to the nearest integer.
    /// </summary>
    public static Fixed Floor(Fixed x)
    {
        return new Fixed(x.Raw & ~Mask);
    }

    /// <summary>
    /// Rounds up to the nearest integer.
    /// </summary>
    public static Fixed Ceiling(Fixed x)
    {
        var fractional = x.Raw & Mask;
        return fractional == 0 ? x : Floor(x) + One;
    }

    /// <summary>
    /// Rounds towards zero to the nearest integer.
    /// </summary>
    public static Fixed Truncate(Fixed x)
    {
        var integral = x.Raw & ~Mask;
        return x.Raw < 0 ? new Fixed(integral + OneRaw) : new Fixed(integral);
    }

    /// <summary>
    /// Rounds to the nearest integer using the default rounding strategy (<see cref="MidpointRounding.ToEven"/>).
    /// </summary>
    public static Fixed Round(Fixed x) => Round(x, MidpointRounding.ToEven);

    /// <summary>
    /// Rounds to the specified number of fractional digits using the default rounding strategy (<see cref="MidpointRounding.ToEven"/>).
    /// </summary>
    public static Fixed Round(Fixed x, int digits) => Round(x, digits, MidpointRounding.ToEven);

    /// <summary>
    /// Rounds to the specified number of fractional digits using the specified rounding strategy.
    /// </summary>
    public static Fixed Round(Fixed x, int digits, MidpointRounding mode)
    {
        // Isolate to be in (-2, 2) range to avoid overflow
        const long mask = (1 << (Shift + 1)) - 1;
        long integral;
        long fractional;
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
        var fractionalRounded = Round(new Fixed(fractional * exp10), mode) / exp10;
        return new Fixed(integral) + new Fixed(fractionalRounded.Raw);
    }

    /// <summary>
    /// Rounds to the nearest integer using the specified rounding strategy.
    /// </summary>
    public static Fixed Round(Fixed x, MidpointRounding mode)
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
                    return new Fixed(integral);
                }

                if (fractional > HalfRaw)
                {
                    return new Fixed(integral + OneRaw);
                }

                return IsEvenInteger(new Fixed(integral)) ? new Fixed(integral) : new Fixed(integral + OneRaw);
            }

            case MidpointRounding.AwayFromZero:
            {
                if (fractional < HalfRaw)
                {
                    return new Fixed(integral);
                }
                if (fractional > HalfRaw)
                {
                    return new Fixed(integral + OneRaw);
                }

                return x.Raw < 0 ? new Fixed(integral) : new Fixed(integral + OneRaw);
            }

            case MidpointRounding.ToZero:
            {
                return x.Raw < 0 ? new Fixed(integral + OneRaw) : new Fixed(integral);
            }

            case MidpointRounding.ToNegativeInfinity:
            {
                return new Fixed(integral);
            }

            case MidpointRounding.ToPositiveInfinity:
            {
                return new Fixed(integral) + One;
            }

            default: throw ExceptionUtility.NotSupported(mode);
        }
    }
}
