using System;
using System.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// A fixed point Q96.32 value (96-bits signed integer, 32-bits fraction).
/// Has a precision of 9 decimal places.
/// </summary>
/// <remarks>
/// This is designed to be used as an intermediate data type for operations
/// that need more precision than what the normal <see cref="Fixed"/> type allows.
/// <para/>
/// As such, <see cref="Fixed128"/> does not provide a complete suite of operations.
/// <para/>
/// Implementation note:
/// The fast methods are called "fast" because they are designed
/// to be used for <see cref="Fixed"/>, which does not need as much precision.
/// </remarks>
public readonly partial struct Fixed128 :
    INumber<Fixed128>,
    IMinMaxValue<Fixed128>,
    ISignedNumber<Fixed128>
{
    // Constants
    public const int BitCount = 128;
    public const int IntegralBitCount = BitCount - Shift - 1;
    public const int FractionalBitCount = Shift;

    public const int Shift = Fixed.Shift * 2;
    private const long Mask = OneRaw - 1;

    private const long HalfRaw = OneRaw / 2;
    private const long HalfMask = HalfRaw - 1;

    private const long OneRaw = 1L << Shift;

    private const long TwoRaw = OneRaw * 2;
    private const long TwoMask = TwoRaw - 1;

    public static Fixed128 Zero => new(0);
    public static Fixed128 Half => new(HalfRaw);
    public static Fixed128 One => new(OneRaw);
    public static Fixed128 Two => new(TwoRaw);
    public static Fixed128 NegativeOne => new(-OneRaw);

    public static Fixed128 AdditiveIdentity => Zero;
    public static Fixed128 MultiplicativeIdentity => One;

    public static Fixed128 MaxValue => new(Int128.MaxValue);
    public static Fixed128 MinValue => new(Int128.MinValue + 1);

    public static Fixed128 Epsilon => new(1);

    public static int Radix => 2;

    internal readonly Int128 Raw;

    internal Fixed128(Int128 raw)
    {
        Raw = raw;
    }

    // To Fixed128

    // Conversion: Safe - No precision loss possible
    public static implicit operator Fixed128(byte value) => new((Int128)value << Shift);
    public static implicit operator Fixed128(short value) => new((Int128)value << Shift);
    public static implicit operator Fixed128(int value) => new((Int128)value << Shift);
    public static implicit operator Fixed128(uint value) => new((Int128)value << Shift);
    public static implicit operator Fixed128(long value) => new((Int128)value << Shift);
    public static implicit operator Fixed128(ulong value) => new((Int128)value << Shift);
    public static implicit operator Fixed128(Fixed value) => new((Int128)value.Raw << (Shift - Fixed.Shift));

    // Conversion: Unsafe - Non-deterministic
    public static explicit operator Fixed128(decimal value) => new((Int128)decimal.Round(value * OneRaw, MidpointRounding.ToEven));

    public static explicit operator Fixed128(float value) => new((Int128)float.Round(value * OneRaw, MidpointRounding.ToEven));
    public static explicit operator checked Fixed128(float value) => new(checked((Int128)float.Round(value * OneRaw, MidpointRounding.ToEven)));

    public static explicit operator Fixed128(double value) => new((Int128)double.Round(value * OneRaw, MidpointRounding.ToEven));
    public static explicit operator checked Fixed128(double value) => new(checked((Int128)double.Round(value * OneRaw, MidpointRounding.ToEven)));

    // From Fixed128

    // Conversion: Loss of fraction / sign
    public static explicit operator int(Fixed128 value) => (int)(value.Raw >> Shift);
    public static explicit operator checked int(Fixed128 value) => checked((int)(value.Raw >> Shift));

    public static explicit operator uint(Fixed128 value) => (uint)(value.Raw >> Shift);
    public static explicit operator checked uint(Fixed128 value) => checked((uint)(value.Raw >> Shift));

    public static explicit operator long(Fixed128 value) => (long)(value.Raw >> Shift);
    public static explicit operator checked long(Fixed128 value) => checked((long)(value.Raw >> Shift));

    public static explicit operator ulong(Fixed128 value) => (ulong)(value.Raw >> Shift);
    public static explicit operator checked ulong(Fixed128 value) => checked((ulong)(value.Raw >> Shift));

    public static explicit operator Int128(Fixed128 value) => value.Raw >> Shift;

    // Conversion: Loss of precision
    public static explicit operator Fixed(Fixed128 value)
    {
        // Round to even for only the truncated portion
        const int shiftAmount = Shift - Fixed.Shift;
        var mask = ((Int128)1 << shiftAmount) - 1;
        var half = (Int128)1 << (shiftAmount - 1);

        var fractional = value.Raw & mask;
        if (fractional < half)
        {
            return new Fixed((long)(value.Raw >> shiftAmount));
        }

        if (fractional > half)
        {
            return new Fixed((long)((value.Raw >> shiftAmount) + 1));
        }

        var result = (long)(value.Raw >> shiftAmount);
        return (result & 1) == 0 ? new Fixed(result) : new Fixed(result + 1);
    }

    public static explicit operator checked Fixed(Fixed128 value)
    {
        // Round to even for only the truncated portion
        const int shiftAmount = Shift - Fixed.Shift;
        var mask = ((Int128)1 << shiftAmount) - 1;
        var half = (Int128)1 << (shiftAmount - 1);

        var fractional = value.Raw & mask;
        if (fractional < half)
        {
            return new Fixed(checked((long)(value.Raw >> shiftAmount)));
        }

        if (fractional > half)
        {
            return new Fixed(checked((long)((value.Raw >> shiftAmount) + 1)));
        }

        var result = checked((long)(value.Raw >> shiftAmount));
        return (result & 1) == 0 ? new Fixed(result) : new Fixed(result + 1);
    }

    // Conversion: Loss of precision / determinism
    public static explicit operator float(Fixed128 value) => (float)value.Raw / OneRaw;
    public static explicit operator double(Fixed128 value) => (double)value.Raw / OneRaw;
    public static explicit operator decimal(Fixed128 value) => (decimal)value.Raw / OneRaw;

    /// <summary>
    /// Creates a fixed point number by using combining an integral part and a fractional part.
    /// <br/>
    /// Eg: FromDecimal(1, 1, 1) -> 1.1
    /// <br/>
    /// Eg: FromDecimal(1, 123, 3) -> 1.123
    /// </summary>
    public static Fixed128 FromDecimal(Int128 integral, int fractional, int decimalPlaces)
    {
        GuardUtility.IsFalse(fractional < 0, "Fractional part cannot be negative");
        GuardUtility.IsFalse(decimalPlaces <= 0 && fractional != 0, "Decimal places must be strictly positive when the fractional part is non-zero");
        GuardUtility.IsFalse(decimalPlaces > 9, "At most 9 decimal places is supported");
        if (integral > ((Int128)1L << IntegralBitCount))
        {
            GuardUtility.Throw($"Integral part must be less than or equal to 2^{IntegralBitCount}");
        }

        if (fractional == 0)
        {
            return new Fixed128(integral * OneRaw);
        }

        var divisor = M.Exp10(decimalPlaces);
        var fractionalRaw = (((long)fractional << 1) * OneRaw) / divisor;
        fractionalRaw += fractionalRaw & 1;
        fractionalRaw >>= 1;

        return new Fixed128(integral * OneRaw + ((integral >> 127) | 1) * fractionalRaw);
    }

    /// <summary>
    /// Creates a fixed point number by dividing the numerator by the denominator.
    /// </summary>
    public static Fixed128 FromFraction(Fixed128 numerator, Fixed128 denominator)
    {
        return numerator / denominator;
    }

    // Rounding

    /// <summary>
    /// Rounds down to the nearest integer.
    /// </summary>
    public static Fixed128 Floor(Fixed128 value)
    {
        return new Fixed128(value.Raw & ~Mask);
    }

    /// <summary>
    /// Rounds up to the nearest integer.
    /// </summary>
    public static Fixed128 Ceiling(Fixed128 value)
    {
        var fractional = value.Raw & Mask;
        return fractional == 0 ? value : Floor(value) + One;
    }

    /// <summary>
    /// Rounds to the nearest integer using the specified rounding strategy.
    /// </summary>
    public static Fixed128 Round(Fixed128 value, MidpointRounding rounding = MidpointRounding.ToEven)
    {
        var integral = value.Raw & ~Mask;
        var fractional = value.Raw & Mask;

        if (fractional == 0)
        {
            return value;
        }

        switch (rounding)
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

                return value.Raw < 0 ? new Fixed128(integral) : new Fixed128(integral + OneRaw);
            }

            case MidpointRounding.ToZero:
            {
                return value.Raw < 0 ? new Fixed128(integral + OneRaw) : new Fixed128(integral);
            }

            case MidpointRounding.ToNegativeInfinity:
            {
                return Floor(value);
            }

            case MidpointRounding.ToPositiveInfinity:
            {
                return Ceiling(value);
            }

            default: throw ExceptionUtility.NotSupportedEnumValue(rounding);
        }
    }

    // Operators
    public static Fixed128 operator +(Fixed128 left, Fixed128 right) => new(left.Raw + right.Raw);
    public static Fixed128 operator -(Fixed128 left, Fixed128 right) => new(left.Raw - right.Raw);
    public static Fixed128 operator *(Fixed128 left, Fixed128 right) => new((left.Raw * right.Raw) >> Shift);
    public static Fixed128 operator /(Fixed128 left, Fixed128 right) => new((left.Raw << Shift) / right.Raw);
    public static Fixed128 operator %(Fixed128 left, Fixed128 right) => new(left.Raw % right.Raw);

    public static Fixed128 operator +(Fixed128 value) => value;
    public static Fixed128 operator -(Fixed128 value) => new(-value.Raw);
    public static Fixed128 operator --(Fixed128 value) => value - One;
    public static Fixed128 operator ++(Fixed128 value) => value + One;

    // Checked operators
    public static Fixed128 operator checked +(Fixed128 left, Fixed128 right) => new(checked(left.Raw + right.Raw));
    public static Fixed128 operator checked -(Fixed128 left, Fixed128 right) => new(checked(left.Raw - right.Raw));
    public static Fixed128 operator checked *(Fixed128 left, Fixed128 right) => new(checked((left.Raw * right.Raw) >> Shift));
    public static Fixed128 operator checked /(Fixed128 left, Fixed128 right) => new(checked((left.Raw << Shift) / right.Raw));

    public static Fixed128 operator checked -(Fixed128 value) => new(checked(-value.Raw));
    public static Fixed128 operator checked --(Fixed128 value) => checked(value - One);
    public static Fixed128 operator checked ++(Fixed128 value) => checked(value + One);

    // Comparisons
    public static bool operator ==(Fixed128 left, Fixed128 right) => left.Raw == right.Raw;
    public static bool operator !=(Fixed128 left, Fixed128 right) => left.Raw != right.Raw;
    public static bool operator <(Fixed128 left, Fixed128 right) => left.Raw < right.Raw;
    public static bool operator >(Fixed128 left, Fixed128 right) => left.Raw > right.Raw;
    public static bool operator <=(Fixed128 left, Fixed128 right) => left.Raw <= right.Raw;
    public static bool operator >=(Fixed128 left, Fixed128 right) => left.Raw >= right.Raw;

    public int CompareTo(object? obj) => obj is Fixed128 other ? CompareTo(other) : throw new ArgumentException();
    public int CompareTo(Fixed128 other) => Raw.CompareTo(other.Raw);

    public override bool Equals(object? obj) => obj is Fixed128 other && Equals(other);
    public bool Equals(Fixed128 other) => Raw == other.Raw;
    public override int GetHashCode() => Raw.GetHashCode();

    // Properties
    public static bool IsZero(Fixed128 value) => value.Raw == 0;
    public static bool IsPositive(Fixed128 value) => value.Raw >= 0; // Int32.IsPositive uses >=
    public static bool IsNegative(Fixed128 value) => value.Raw < 0;

    public static bool IsInteger(Fixed128 value) => (value.Raw & (OneRaw - 1)) == 0;
    public static bool IsEvenInteger(Fixed128 value) => IsInteger(value) && (value.Raw >> Shift) % 2 == 0;
    public static bool IsOddInteger(Fixed128 value) => IsInteger(value) && (value.Raw >> Shift) % 2 != 0;

    public static bool IsRealNumber(Fixed128 value) => true;
    public static bool IsComplexNumber(Fixed128 value) => false;
    public static bool IsImaginaryNumber(Fixed128 value) => false;

    public static bool IsFinite(Fixed128 value) => true;
    public static bool IsInfinity(Fixed128 value) => false;
    public static bool IsPositiveInfinity(Fixed128 value) => false;
    public static bool IsNegativeInfinity(Fixed128 value) => false;

    public static bool IsNormal(Fixed128 value) => value.Raw != 0;
    public static bool IsCanonical(Fixed128 value) => true;
    public static bool IsNaN(Fixed128 value) => false;
    public static bool IsSubnormal(Fixed128 value) => false;

    // Magnitude
    public static Fixed128 Sign(Fixed128 value) => new(Int128.Sign(value.Raw) * OneRaw);
    public static Fixed128 SignNonZero(Fixed128 value) => new(((value.Raw >> 63) | 1) * OneRaw);
    public static Fixed128 Abs(Fixed128 value) => new(Int128.Abs(value.Raw));
    public static Fixed128 MaxMagnitude(Fixed128 x, Fixed128 y) => Abs(x) > Abs(y) ? x : y;
    public static Fixed128 MaxMagnitudeNumber(Fixed128 x, Fixed128 y) => MaxMagnitude(x, y);
    public static Fixed128 MinMagnitude(Fixed128 x, Fixed128 y) => Abs(x) < Abs(y) ? x : y;
    public static Fixed128 MinMagnitudeNumber(Fixed128 x, Fixed128 y) => MinMagnitude(x, y);

    // ToFromRaw
    public Int128 ToRaw()
    {
        return Raw;
    }

    public static Fixed128 FromRaw(Int128 raw)
    {
        return new Fixed128(raw);
    }
}
