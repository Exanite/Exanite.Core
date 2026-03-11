using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// A fixed point Q96.32 value (96-bits signed integer, 32-bits fraction).
/// Has a precision of 9 decimal places.
/// </summary>
public readonly partial struct Fixed128 :
    INumber<Fixed128>,
    IMinMaxValue<Fixed128>,
    ISignedNumber<Fixed128>
{
    // Constants
    public const int BitCount = 128;
    public const int IntegralBitCount = BitCount - Shift;
    public const int FractionalBitCount = Shift;

    public const int Shift = 32;
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

    /// <summary>
    /// The number of decimal places of precision.
    /// </summary>
    public static int Precision => 9;

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
    public static implicit operator Fixed128(Fixed value) => new((Int128)value.Raw << (Shift - Fixed.Shift));

    // Conversion: Can exceed range
    public static explicit operator Fixed128(long value) => new(value << Shift);
    public static explicit operator Fixed128(decimal value) => new((Int128)value * OneRaw);

    // Conversion: Unsafe - Non-deterministic
    // Consider using FromFraction or FromParts instead
    public static explicit operator Fixed128(float value) => new((Int128)(value * OneRaw));
    public static explicit operator Fixed128(double value) => new((Int128)(value * OneRaw));

    // From Fixed128

    // Conversion: Loss of fraction / sign
    public static explicit operator int(Fixed128 value) => (int)(value.Raw >> Shift);
    public static explicit operator uint(Fixed128 value) => (uint)(value.Raw >> Shift);
    public static explicit operator long(Fixed128 value) => (long)(value.Raw >> Shift);
    public static explicit operator ulong(Fixed128 value) => (ulong)(value.Raw >> Shift);
    public static explicit operator Int128(Fixed128 value) => value.Raw >> Shift;

    // Conversion: Loss of precision
    public static explicit operator Fixed(Fixed128 value) => new((long)(value.Raw >> (Shift - Fixed.Shift)));

    // Conversion: Loss of precision / determinism
    public static explicit operator float(Fixed128 value) => (float)value.Raw / OneRaw;
    public static explicit operator double(Fixed128 value) => (double)value.Raw / OneRaw;
    public static explicit operator decimal(Fixed128 value) => (decimal)value.Raw / OneRaw;

    /// <summary>
    /// Creates a fixed point number by using combining an integral part and a fractional part.
    /// <br/>
    /// Eg: FromParts(1, 1) -> 1.1
    /// <br/>
    /// Eg: FromParts(1, 123) -> 1.123
    /// </summary>
    public static Fixed128 FromParts(Int128 integral, int fractional)
    {
        AssertUtility.IsTrue(integral <= ((Int128)1L << IntegralBitCount) , "Integral part must be less than or equal to 2^96");
        AssertUtility.IsFalse(fractional < 0, "Fractional part cannot be negative");

        if (fractional == 0)
        {
            return new Fixed128(integral * OneRaw);
        }

        // Determine the number of digits in the fractional part
        // Note: Not sure if a LUT is actually faster here, but I don't want to benchmark right now
        var divisor = 10;
        foreach (var digitLimit in DigitsLut)
        {
            if (fractional < digitLimit)
            {
                break;
            }

            divisor *= 10;
        }

        return new Fixed128(integral * OneRaw + ((integral >> 127) | 1) * ((fractional * OneRaw) / divisor));
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
    /// Rounds to the nearest integer.
    /// If halfway between an even and odd value, returns the even value.
    /// </summary>
    public static Fixed128 Round(Fixed128 value)
    {
        var integral = value.Raw & ~Mask;
        var fractional = value.Raw & Mask;

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

    // Operators
    public static Fixed128 operator +(Fixed128 left, Fixed128 right) => new(left.Raw + right.Raw);
    public static Fixed128 operator -(Fixed128 left, Fixed128 right) => new(left.Raw - right.Raw);
    public static Fixed128 operator %(Fixed128 left, Fixed128 right) => new(left.Raw % right.Raw);

    public static Fixed128 operator *(Fixed128 left, Fixed128 right)
    {
        return new Fixed128((left.Raw * right.Raw) >> Shift);
    }

    public static Fixed128 operator /(Fixed128 left, Fixed128 right)
    {
        return new Fixed128((left.Raw << Shift) / right.Raw);
    }

    public static Fixed128 operator +(Fixed128 value) => value;
    public static Fixed128 operator -(Fixed128 value) => new(-value.Raw);
    public static Fixed128 operator --(Fixed128 value) => value - One;
    public static Fixed128 operator ++(Fixed128 value) => value + One;

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

    // Magnitude methods
    public static Fixed128 Sign(Fixed128 value) => new(Int128.Sign(value.Raw) * OneRaw);
    public static Fixed128 SignNonZero(Fixed128 value) => new(((value.Raw >> 63) | 1) * OneRaw);
    public static Fixed128 Abs(Fixed128 value) => new(Int128.Abs(value.Raw));
    public static Fixed128 MaxMagnitude(Fixed128 x, Fixed128 y) => Abs(x) > Abs(y) ? x : y;
    public static Fixed128 MaxMagnitudeNumber(Fixed128 x, Fixed128 y) => MaxMagnitude(x, y);
    public static Fixed128 MinMagnitude(Fixed128 x, Fixed128 y) => Abs(x) < Abs(y) ? x : y;
    public static Fixed128 MinMagnitudeNumber(Fixed128 x, Fixed128 y) => MinMagnitude(x, y);

    // Formatting
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return ((decimal)Raw / OneRaw).ToString(format, formatProvider);
    }

    public override string ToString() => ToString(null, null);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return ((decimal)Raw / OneRaw).TryFormat(destination, out charsWritten, format, provider);
    }

    // Parsing
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Fixed128 result)
    {
        if (Int128.TryParse(s, style, provider, out var int128Value))
        {
            result = new Fixed128(int128Value * OneRaw);
            return true;
        }

        if (decimal.TryParse(s, style, provider, out var decimalValue))
        {
            result = new Fixed128((Int128)(decimalValue * OneRaw));
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Fixed128 result) => TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Fixed128 result) => TryParse((ReadOnlySpan<char>)s, provider, out result);
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Fixed128 result) => TryParse((ReadOnlySpan<char>)s, style, provider, out result);

    public static Fixed128 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        if (!TryParse(s, style, provider, out var result))
        {
            throw new FormatException($"Input string '{s.ToString()}' was not in a correct format.");
        }

        return result;
    }

    public static Fixed128 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider);
    public static Fixed128 Parse(string s, IFormatProvider? provider) => Parse((ReadOnlySpan<char>)s, provider);
    public static Fixed128 Parse(string s, NumberStyles style, IFormatProvider? provider) => Parse((ReadOnlySpan<char>)s, style, provider);

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
