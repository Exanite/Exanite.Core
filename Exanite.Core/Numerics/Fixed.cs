using System;
using System.Numerics;

namespace Exanite.Core.Numerics;

/// <summary>
/// A fixed point Q48.16 value (48-bits integer, 16-bits fraction).
/// </summary>
public readonly struct Fixed :
    IEquatable<Fixed>,
    IComparable<Fixed>,
    IAdditiveIdentity<Fixed, Fixed>,
    IMultiplicativeIdentity<Fixed, Fixed>
{
    // Constants
    private const int Shift = 16;
    private const long OneValue = 1L << Shift;

    public static Fixed One => new(OneValue);
    public static Fixed Zero => new(0);
    public static Fixed AdditiveIdentity => Zero;
    public static Fixed MultiplicativeIdentity => One;

    private readonly long value;

    private Fixed(long value)
    {
        this.value = value;
    }

    // Comparisons
    public static bool operator ==(Fixed left, Fixed right) => left.value == right.value;
    public static bool operator !=(Fixed left, Fixed right) => left.value != right.value;
    public static bool operator <(Fixed left, Fixed right) => left.value < right.value;
    public static bool operator >(Fixed left, Fixed right) => left.value > right.value;
    public static bool operator <=(Fixed left, Fixed right) => left.value <= right.value;
    public static bool operator >=(Fixed left, Fixed right) => left.value >= right.value;

    public int CompareTo(Fixed other) => value.CompareTo(other.value);

    public override bool Equals(object? obj) => obj is Fixed other && Equals(other);
    public bool Equals(Fixed other) => value == other.value;
    public override int GetHashCode() => value.GetHashCode();
}
