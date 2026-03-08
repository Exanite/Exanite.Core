using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// A fixed point Q48.16 value (48-bits integer, 16-bits fraction).
/// Has a precision of 4 decimal places.
/// </summary>
public readonly partial struct Fixed :
    INumber<Fixed>,
    IMinMaxValue<Fixed>,
    ISignedNumber<Fixed>,
    IFloatingPointConstants<Fixed>
{
    // Constants
    private const int Shift = 16;
    private const int Mask = (1 << Shift) - 1;
    private const long OneValue = 1L << Shift;
    private const long HalfValue = OneValue / 2;

    public static Fixed One => new(OneValue);
    public static Fixed Zero => new(0);
    public static Fixed NegativeOne => new(-OneValue);

    public static Fixed AdditiveIdentity => Zero;
    public static Fixed MultiplicativeIdentity => One;

    public static Fixed MaxValue => new(long.MaxValue);
    public static Fixed MinValue => new(long.MinValue + 1);

    public static Fixed E => new(178145); // Equal to floor(e * 2^16)
    public static Fixed Pi => new(205887); // Equal to floor(pi * 2^16)
    public static Fixed Tau => new(411774); // Equal to floor(pi * 2^16) * 2

    public static int Radix => 2;

    /// <summary>
    /// The number of decimal places of precision.
    /// </summary>
    public static int Precision => 4;

    private readonly long value;

    private Fixed(long value)
    {
        this.value = value;
    }

    // Conversion: Safe - No precision loss possible
    public static implicit operator Fixed(byte value) => new((long)value << Shift);
    public static implicit operator Fixed(short value) => new((long)value << Shift);
    public static implicit operator Fixed(int value) => new((long)value << Shift);
    public static implicit operator Fixed(decimal value) => new((long)value * OneValue);

    // Conversion: Potentially unsafe - Can exceed 48-bit integer range
    public static explicit operator Fixed(long value) => new(value << Shift);

    // Conversion: Unsafe - Non-deterministic
    // Consider using FromFraction or FromParts instead
    public static explicit operator Fixed(float value) => new((long)(value * OneValue));
    public static explicit operator Fixed(double value) => new((long)(value * OneValue));

    // Conversion: Loss of fraction
    public static explicit operator int(Fixed value) => (int)(value.value >> Shift);
    public static explicit operator long(Fixed value) => value.value >> Shift;

    // Conversion: Loss of precision
    public static explicit operator float(Fixed value) => (float)value.value / OneValue;
    public static explicit operator double(Fixed value) => (double)value.value / OneValue;

    /// <inheritdoc cref="FromParts(long,int)"/>
    public static Fixed FromParts(int integral, int fractional)
    {
        AssertUtility.IsFalse(fractional < 0, "Fractional part cannot be negative");

        if (fractional == 0)
        {
            return new Fixed(integral * OneValue);
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

        return new Fixed(integral * OneValue + long.Sign(integral) * ((fractional * OneValue) / divisor));
    }

    /// <summary>
    /// Creates a fixed point number by using combining an integral part and a fractional part.
    /// <br/>
    /// Eg: FromParts(1, 1) -> 1.1
    /// <br/>
    /// Eg: FromParts(1, 123) -> 1.123
    /// </summary>
    public static Fixed FromParts(long integral, int fractional)
    {
        AssertUtility.IsTrue(integral <= (1L << 48) , "Integral part must be less than or equal to 2^48");
        AssertUtility.IsFalse(fractional < 0, "Fractional part cannot be negative");

        if (fractional == 0)
        {
            return new Fixed(integral * OneValue);
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

        return new Fixed(integral * OneValue + long.Sign(integral) * ((fractional * OneValue) / divisor));
    }

    /// <summary>
    /// Creates a fixed point number by dividing the numerator by the denominator.
    /// </summary>
    public static Fixed FromFraction(Fixed numerator, Fixed denominator)
    {
        return numerator / denominator;
    }

    // Rounding

    /// <summary>
    /// Rounds down to the nearest integer.
    /// </summary>
    public static Fixed Floor(Fixed value)
    {
        return new Fixed(value.value & ~Mask);
    }

    /// <summary>
    /// Rounds up to the nearest integer.
    /// </summary>
    public static Fixed Ceiling(Fixed value)
    {
        var fractional = value.value & Mask;
        return fractional == 0 ? value : Floor(value) + One;
    }

    /// <summary>
    /// Rounds to the nearest integer.
    /// If halfway between an even and odd value, returns the even value.
    /// </summary>
    public static Fixed Round(Fixed value)
    {
        var integral = value.value & ~Mask;
        var fractional = value.value & Mask;

        if (fractional < HalfValue)
        {
            return new Fixed(integral);
        }

        if (fractional > HalfValue)
        {
            return new Fixed(integral + OneValue);
        }

        return IsEvenInteger(new Fixed(integral)) ? new Fixed(integral) : new Fixed(integral + OneValue);
    }

    // Operators
    public static Fixed operator +(Fixed left, Fixed right) => new(left.value + right.value);
    public static Fixed operator -(Fixed left, Fixed right) => new(left.value - right.value);
    public static Fixed operator %(Fixed left, Fixed right) => new(left.value % right.value);

    public static Fixed operator *(Fixed left, Fixed right)
    {
        return new Fixed((long)(((Int128)left.value * right.value) >> Shift));
    }

    public static Fixed operator /(Fixed left, Fixed right)
    {
        return new Fixed((long)(((Int128)left.value << Shift) / right.value));
    }

    public static Fixed operator +(Fixed value) => value;
    public static Fixed operator -(Fixed value) => new(-value.value);
    public static Fixed operator --(Fixed value) => value - One;
    public static Fixed operator ++(Fixed value) => value + One;

    // Comparisons
    public static bool operator ==(Fixed left, Fixed right) => left.value == right.value;
    public static bool operator !=(Fixed left, Fixed right) => left.value != right.value;
    public static bool operator <(Fixed left, Fixed right) => left.value < right.value;
    public static bool operator >(Fixed left, Fixed right) => left.value > right.value;
    public static bool operator <=(Fixed left, Fixed right) => left.value <= right.value;
    public static bool operator >=(Fixed left, Fixed right) => left.value >= right.value;

    public int CompareTo(object? obj) => obj is Fixed other ? CompareTo(other) : throw new ArgumentException();
    public int CompareTo(Fixed other) => value.CompareTo(other.value);

    public override bool Equals(object? obj) => obj is Fixed other && Equals(other);
    public bool Equals(Fixed other) => value == other.value;
    public override int GetHashCode() => value.GetHashCode();

    // Properties
    public static bool IsZero(Fixed value) => value.value == 0;
    public static bool IsPositive(Fixed value) => value.value >= 0; // Int32.IsPositive uses >=
    public static bool IsNegative(Fixed value) => value.value < 0;

    public static bool IsInteger(Fixed value) => (value.value & (OneValue - 1)) == 0;
    public static bool IsEvenInteger(Fixed value) => IsInteger(value) && (value.value >> Shift) % 2 == 0;
    public static bool IsOddInteger(Fixed value) => IsInteger(value) && (value.value >> Shift) % 2 != 0;

    public static bool IsRealNumber(Fixed value) => true;
    public static bool IsComplexNumber(Fixed value) => false;
    public static bool IsImaginaryNumber(Fixed value) => false;

    public static bool IsFinite(Fixed value) => true;
    public static bool IsInfinity(Fixed value) => false;
    public static bool IsPositiveInfinity(Fixed value) => false;
    public static bool IsNegativeInfinity(Fixed value) => false;

    public static bool IsNormal(Fixed value) => value.value != 0;
    public static bool IsCanonical(Fixed value) => true;
    public static bool IsNaN(Fixed value) => false;
    public static bool IsSubnormal(Fixed value) => false;

    // Magnitude methods
    public static Fixed Abs(Fixed value) => new(long.Abs(value.value));
    public static Fixed MaxMagnitude(Fixed x, Fixed y) => Abs(x) > Abs(y) ? x : y;
    public static Fixed MaxMagnitudeNumber(Fixed x, Fixed y) => MaxMagnitude(x, y);
    public static Fixed MinMagnitude(Fixed x, Fixed y) => Abs(x) < Abs(y) ? x : y;
    public static Fixed MinMagnitudeNumber(Fixed x, Fixed y) => MinMagnitude(x, y);

    // Creation methods
    public static bool TryConvertFromChecked<TOther>(TOther value, out Fixed result) where TOther : INumberBase<TOther>
    {
        if (TOther.IsInteger(value) && TryConvertToLong<TOther, long>(value, out var longValue))
        {
            result = new Fixed(longValue * OneValue);
            return true;
        }

        if (TryConvertToDecimal<TOther, decimal>(value, out var decimalValue))
        {
            result = new Fixed((long)(decimalValue * OneValue));
            return true;
        }

        result = default;
        return false;
    }

    private static bool TryConvertToLong<TFrom, TTo>(TFrom value, out long result)
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<long>
    {
        return TTo.TryConvertFromChecked(value, out result);
    }

    private static bool TryConvertToDecimal<TFrom, TTo>(TFrom value, out decimal result)
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<decimal>
    {
        return TTo.TryConvertFromChecked(value, out result);
    }

    public static bool TryConvertToChecked<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        var decimalValue = (decimal)value.value / OneValue;
        return TOther.TryConvertFromChecked(decimalValue, out result);
    }

    public static Fixed CreateChecked<TOther>(TOther value) where TOther : INumberBase<TOther> => TryConvertFromChecked(value, out var result) ? result : throw new NotSupportedException($"Failed to create a fixed point value from the provided value: {value}");
    public static Fixed CreateSaturating<TOther>(TOther value) where TOther : INumberBase<TOther> => CreateChecked(value);
    public static Fixed CreateTruncating<TOther>(TOther value) where TOther : INumberBase<TOther> => CreateChecked(value);
    public static bool TryConvertFromSaturating<TOther>(TOther value, out Fixed result) where TOther : INumberBase<TOther> => TryConvertFromChecked(value, out result);
    public static bool TryConvertFromTruncating<TOther>(TOther value, out Fixed result) where TOther : INumberBase<TOther> => TryConvertFromChecked(value, out result);
    public static bool TryConvertToSaturating<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => TryConvertToChecked(value, out result);
    public static bool TryConvertToTruncating<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => TryConvertToChecked(value, out result);

    // Formatting
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return ((decimal)value / OneValue).ToString(format, formatProvider);
    }

    public override string ToString() => ToString(null, null);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return ((decimal)value / OneValue).TryFormat(destination, out charsWritten, format, provider);
    }

    // Parsing
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Fixed result)
    {
        if (long.TryParse(s, style, provider, out var longValue))
        {
            result = new Fixed(longValue * OneValue);
            return true;
        }

        if (decimal.TryParse(s, style, provider, out var decimalValue))
        {
            result = new Fixed((long)(decimalValue * OneValue));
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Fixed result) => TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Fixed result) => TryParse((ReadOnlySpan<char>)s, provider, out result);
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Fixed result) => TryParse((ReadOnlySpan<char>)s, style, provider, out result);

    public static Fixed Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        if (!TryParse(s, style, provider, out var result))
        {
            throw new FormatException($"Input string '{s.ToString()}' was not in a correct format.");
        }

        return result;
    }

    public static Fixed Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider);
    public static Fixed Parse(string s, IFormatProvider? provider) => Parse((ReadOnlySpan<char>)s, provider);
    public static Fixed Parse(string s, NumberStyles style, IFormatProvider? provider) => Parse((ReadOnlySpan<char>)s, style, provider);
}
