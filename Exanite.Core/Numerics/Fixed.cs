using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Exanite.Core.Numerics;

/// <summary>
/// A fixed point Q48.16 value (48-bits integer, 16-bits fraction).
/// </summary>
public readonly struct Fixed :
    IEquatable<Fixed>,
    IComparable<Fixed>,
    IAdditiveIdentity<Fixed, Fixed>,
    IMultiplicativeIdentity<Fixed, Fixed>,
    IAdditionOperators<Fixed, Fixed, Fixed>,
    ISubtractionOperators<Fixed, Fixed, Fixed>,
    IMultiplyOperators<Fixed, Fixed, Fixed>,
    IDivisionOperators<Fixed, Fixed, Fixed>,
    IEqualityOperators<Fixed, Fixed, bool>,
    IIncrementOperators<Fixed>,
    IDecrementOperators<Fixed>,
    IUnaryPlusOperators<Fixed, Fixed>,
    IUnaryNegationOperators<Fixed, Fixed>,
    INumber<Fixed>
{
    // Constants
    private const int Shift = 16;
    private const long OneValue = 1L << Shift;

    public static Fixed One => new(OneValue);
    public static Fixed Zero => new(0);
    public static Fixed AdditiveIdentity => Zero;
    public static Fixed MultiplicativeIdentity => One;

    public static int Radix => 2;

    private readonly long value;

    private Fixed(long value)
    {
        this.value = value;
    }

    // Operators
    // TODO: Use Int128 to avoid overflows
    public static Fixed operator +(Fixed left, Fixed right) => new(left.value + right.value);
    public static Fixed operator -(Fixed left, Fixed right) => new(left.value - right.value);
    public static Fixed operator *(Fixed left, Fixed right) => new((left.value * right.value) >> Shift);
    public static Fixed operator /(Fixed left, Fixed right) => new((left.value << Shift) / right.value);
    public static Fixed operator %(Fixed left, Fixed right) => new(left.value % right.value);

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
    public static bool IsOddInteger(Fixed value) => IsInteger(value) && (value.value >> Shift) % 2 == 1;

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
        if (TryConvertToDouble<TOther, double>(value, out var doubleValue))
        {
            result = new Fixed((long)(doubleValue * OneValue));
            return true;
        }

        result = default;
        return false;
    }

    private static bool TryConvertToDouble<TOther, TDouble>(TOther value, out double result)
        where TOther : INumberBase<TOther>
        where TDouble : INumberBase<double>
    {
        return TDouble.TryConvertFromChecked(value, out result);
    }

    public static bool TryConvertToChecked<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        var doubleValue = value.value / (double)OneValue;
        return TOther.TryConvertFromChecked(doubleValue, out result);
    }

    public static Fixed CreateChecked<TOther>(TOther value) where TOther : INumberBase<TOther> => TryConvertFromChecked(value, out var result) ? result : throw new OverflowException();
    public static Fixed CreateSaturating<TOther>(TOther value) where TOther : INumberBase<TOther> => CreateChecked(value);
    public static Fixed CreateTruncating<TOther>(TOther value) where TOther : INumberBase<TOther> => CreateChecked(value);
    public static bool TryConvertFromSaturating<TOther>(TOther value, out Fixed result) where TOther : INumberBase<TOther> => TryConvertFromChecked(value, out result);
    public static bool TryConvertFromTruncating<TOther>(TOther value, out Fixed result) where TOther : INumberBase<TOther> => TryConvertFromChecked(value, out result);
    public static bool TryConvertToSaturating<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => TryConvertToChecked(value, out result);
    public static bool TryConvertToTruncating<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => TryConvertToChecked(value, out result);

    // public string ToString(string? format, IFormatProvider? formatProvider);
    // public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider);

    // public static Fixed Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider);
    // public static Fixed Parse(string s, NumberStyles style, IFormatProvider? provider);
    // public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Fixed result);
    // public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Fixed result);

    // public static Fixed Parse(string s, IFormatProvider? provider);
    // public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Fixed result);
    // public static Fixed Parse(ReadOnlySpan<char> s, IFormatProvider? provider);
    // public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Fixed result);
}
