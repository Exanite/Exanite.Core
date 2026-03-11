using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// A fixed point Q48.16 value (48-bits signed integer, 16-bits fraction).
/// Has a precision of 4 decimal places.
/// </summary>
/// <remarks>
/// This is designed to be used in scenarios where
/// performance is preferred over precision.
/// <para/>
/// Be aware that complex operations tend to lose precision very fast.
/// See the test cases to understand roughly how precise each method is.
/// Also note that the precision for the most part can be improved,
/// either by improved algorithms or increased performance/memory cost.
/// </remarks>
public readonly partial struct Fixed :
    INumber<Fixed>,
    IMinMaxValue<Fixed>,
    ISignedNumber<Fixed>,
    ITrigonometricFunctions<Fixed>,
    ILogarithmicFunctions<Fixed>,
    IPowerFunctions<Fixed>,
    IRootFunctions<Fixed>
{
    // Constants
    public const int BitCount = 64;
    public const int IntegralBitCount = BitCount - Shift - 1;
    public const int FractionalBitCount = Shift;

    public const int Shift = 16;
    private const int Mask = (int)(OneRaw - 1);

    private const long HalfRaw = OneRaw / 2;
    private const long HalfMask = HalfRaw - 1;

    private const long OneRaw = 1L << Shift;

    private const long TwoRaw = OneRaw * 2;
    private const long TwoMask = TwoRaw - 1;

    public static Fixed Zero => new(0);
    public static Fixed Half => new(HalfRaw);
    public static Fixed One => new(OneRaw);
    public static Fixed Two => new(TwoRaw);
    public static Fixed NegativeOne => new(-OneRaw);

    public static Fixed AdditiveIdentity => Zero;
    public static Fixed MultiplicativeIdentity => One;

    public static Fixed MaxValue => new(long.MaxValue);
    public static Fixed MinValue => new(long.MinValue + 1);

    public static Fixed E => new(ERaw);
    public static Fixed Pi => new(PiRaw);
    public static Fixed PiHalf => new(PiHalfRaw);
    public static Fixed PiInverse => new(PiInverseRaw);
    public static Fixed Tau => new(TauRaw);

    public static Fixed Epsilon => new(1);

    public static int Radix => 2;

    internal readonly long Raw;

    internal Fixed(long raw)
    {
        Raw = raw;
    }

    // To Fixed

    // Conversion: Safe - No precision loss possible
    public static implicit operator Fixed(byte value) => new((long)value << Shift);
    public static implicit operator Fixed(short value) => new((long)value << Shift);
    public static implicit operator Fixed(int value) => new((long)value << Shift);
    public static implicit operator Fixed(uint value) => new((long)value << Shift);

    // Conversion: Can exceed range
    public static explicit operator Fixed(long value) => new(value << Shift);
    public static explicit operator checked Fixed(long value)
    {
        if (value > (long)MaxValue)
        {
            throw new OverflowException();
        }

        return new Fixed(value << Shift);
    }

    public static explicit operator Fixed(decimal value) => new((long)(value * OneRaw));

    // Conversion: Unsafe - Non-deterministic
    // Consider using FromFraction or FromParts instead
    public static explicit operator Fixed(float value) => new((long)(value * OneRaw));
    public static explicit operator checked Fixed(float value) => new(checked((long)(value * OneRaw)));

    public static explicit operator Fixed(double value) => new((long)(value * OneRaw));
    public static explicit operator checked Fixed(double value) => new(checked((long)(value * OneRaw)));

    // From Fixed

    // Conversion: Loss of fraction / sign
    public static explicit operator int(Fixed value) => (int)(value.Raw >> Shift);
    public static explicit operator checked int(Fixed value) => checked((int)(value.Raw >> Shift));

    public static explicit operator uint(Fixed value) => (uint)(value.Raw >> Shift);
    public static explicit operator checked uint(Fixed value) => checked((uint)(value.Raw >> Shift));

    public static explicit operator long(Fixed value) => value.Raw >> Shift;

    public static explicit operator ulong(Fixed value) => (ulong)(value.Raw >> Shift);
    public static explicit operator checked ulong(Fixed value) => checked((ulong)(value.Raw >> Shift));

    // Conversion: Loss of precision / determinism
    public static explicit operator float(Fixed value) => (float)value.Raw / OneRaw;
    public static explicit operator double(Fixed value) => (double)value.Raw / OneRaw;
    public static explicit operator decimal(Fixed value) => (decimal)value.Raw / OneRaw;

    /// <inheritdoc cref="FromDecimal(long,int,int)"/>
    public static Fixed FromDecimal(int integral, int fractional, int decimalPlaces)
    {
        GuardUtility.IsFalse(fractional < 0, "Fractional part cannot be negative");
        GuardUtility.IsFalse(decimalPlaces <= 0 && fractional != 0, "Decimal places must be strictly positive when the fractional part is non-zero");

        if (fractional == 0)
        {
            return new Fixed(integral * OneRaw);
        }

        var divisor = 10;
        for (var i = 1; i < decimalPlaces; i++)
        {
            divisor *= 10;
        }

        return new Fixed(integral * OneRaw + ((integral >> 31) | 1) * ((fractional * OneRaw) / divisor));
    }

    /// <summary>
    /// Creates a fixed point number by using combining an integral part and a fractional part.
    /// <br/>
    /// Eg: FromParts(1, 1, 1) -> 1.1
    /// <br/>
    /// Eg: FromParts(1, 123, 3) -> 1.123
    /// </summary>
    public static Fixed FromDecimal(long integral, int fractional, int decimalPlaces)
    {
        GuardUtility.IsFalse(fractional < 0, "Fractional part cannot be negative");
        GuardUtility.IsFalse(decimalPlaces <= 0 && fractional != 0, "Decimal places must be strictly positive when the fractional part is non-zero");
        if (integral > (1L << IntegralBitCount))
        {
            GuardUtility.Throw($"Integral part must be less than or equal to 2^{IntegralBitCount}");
        }

        if (fractional == 0)
        {
            return new Fixed(integral * OneRaw);
        }

        var divisor = 10;
        for (var i = 1; i < decimalPlaces; i++)
        {
            divisor *= 10;
        }

        return new Fixed(integral * OneRaw + ((integral >> 63) | 1) * ((fractional * OneRaw) / divisor));
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
        return new Fixed(value.Raw & ~Mask);
    }

    /// <summary>
    /// Rounds up to the nearest integer.
    /// </summary>
    public static Fixed Ceiling(Fixed value)
    {
        var fractional = value.Raw & Mask;
        return fractional == 0 ? value : Floor(value) + One;
    }

    /// <summary>
    /// Rounds to the nearest integer.
    /// If halfway between an even and odd value, returns the even value.
    /// </summary>
    public static Fixed Round(Fixed value)
    {
        var integral = value.Raw & ~Mask;
        var fractional = value.Raw & Mask;

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

    // Operators
    public static Fixed operator +(Fixed left, Fixed right) => new(left.Raw + right.Raw);
    public static Fixed operator -(Fixed left, Fixed right) => new(left.Raw - right.Raw);
    public static Fixed operator *(Fixed left, Fixed right) => new((long)(((Int128)left.Raw * right.Raw) >> Shift));
    public static Fixed operator /(Fixed left, Fixed right) => new((long)(((Int128)left.Raw << Shift) / right.Raw));
    public static Fixed operator %(Fixed left, Fixed right) => new(left.Raw % right.Raw);

    public static Fixed operator +(Fixed value) => value;
    public static Fixed operator -(Fixed value) => new(-value.Raw);
    public static Fixed operator --(Fixed value) => value - One;
    public static Fixed operator ++(Fixed value) => value + One;

    // Checked operators
    public static Fixed operator checked +(Fixed left, Fixed right) => new(checked(left.Raw + right.Raw));
    public static Fixed operator checked -(Fixed left, Fixed right) => new(checked(left.Raw - right.Raw));
    public static Fixed operator checked *(Fixed left, Fixed right) => new(checked((long)(((Int128)left.Raw * right.Raw) >> Shift)));
    public static Fixed operator checked /(Fixed left, Fixed right) => new(checked((long)(((Int128)left.Raw << Shift) / right.Raw)));

    public static Fixed operator checked -(Fixed value) => new(checked(-value.Raw));
    public static Fixed operator checked --(Fixed value) => checked(value - One);
    public static Fixed operator checked ++(Fixed value) => checked(value + One);

    // Comparisons
    public static bool operator ==(Fixed left, Fixed right) => left.Raw == right.Raw;
    public static bool operator !=(Fixed left, Fixed right) => left.Raw != right.Raw;
    public static bool operator <(Fixed left, Fixed right) => left.Raw < right.Raw;
    public static bool operator >(Fixed left, Fixed right) => left.Raw > right.Raw;
    public static bool operator <=(Fixed left, Fixed right) => left.Raw <= right.Raw;
    public static bool operator >=(Fixed left, Fixed right) => left.Raw >= right.Raw;

    public int CompareTo(object? obj) => obj is Fixed other ? CompareTo(other) : throw new ArgumentException();
    public int CompareTo(Fixed other) => Raw.CompareTo(other.Raw);

    public override bool Equals(object? obj) => obj is Fixed other && Equals(other);
    public bool Equals(Fixed other) => Raw == other.Raw;
    public override int GetHashCode() => Raw.GetHashCode();

    // Properties
    public static bool IsZero(Fixed value) => value.Raw == 0;
    public static bool IsPositive(Fixed value) => value.Raw >= 0; // Int32.IsPositive uses >=
    public static bool IsNegative(Fixed value) => value.Raw < 0;

    public static bool IsInteger(Fixed value) => (value.Raw & (OneRaw - 1)) == 0;
    public static bool IsEvenInteger(Fixed value) => IsInteger(value) && (value.Raw >> Shift) % 2 == 0;
    public static bool IsOddInteger(Fixed value) => IsInteger(value) && (value.Raw >> Shift) % 2 != 0;

    public static bool IsRealNumber(Fixed value) => true;
    public static bool IsComplexNumber(Fixed value) => false;
    public static bool IsImaginaryNumber(Fixed value) => false;

    public static bool IsFinite(Fixed value) => true;
    public static bool IsInfinity(Fixed value) => false;
    public static bool IsPositiveInfinity(Fixed value) => false;
    public static bool IsNegativeInfinity(Fixed value) => false;

    public static bool IsNormal(Fixed value) => value.Raw != 0;
    public static bool IsCanonical(Fixed value) => true;
    public static bool IsNaN(Fixed value) => false;
    public static bool IsSubnormal(Fixed value) => false;

    // Magnitude methods
    public static Fixed Sign(Fixed value) => new(long.Sign(value.Raw) * OneRaw);
    public static Fixed SignNonZero(Fixed value) => new(((value.Raw >> 63) | 1) * OneRaw);
    public static Fixed Abs(Fixed value) => new(long.Abs(value.Raw));
    public static Fixed MaxMagnitude(Fixed x, Fixed y) => Abs(x) > Abs(y) ? x : y;
    public static Fixed MaxMagnitudeNumber(Fixed x, Fixed y) => MaxMagnitude(x, y);
    public static Fixed MinMagnitude(Fixed x, Fixed y) => Abs(x) < Abs(y) ? x : y;
    public static Fixed MinMagnitudeNumber(Fixed x, Fixed y) => MinMagnitude(x, y);

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
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Fixed result)
    {
        if (long.TryParse(s, style, provider, out var longValue))
        {
            result = new Fixed(longValue * OneRaw);
            return true;
        }

        if (decimal.TryParse(s, style, provider, out var decimalValue))
        {
            result = new Fixed((long)(decimalValue * OneRaw));
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

    // ToFromRaw

    public long ToRaw()
    {
        return Raw;
    }

    public static Fixed FromRaw(long raw)
    {
        return new Fixed(raw);
    }
}
