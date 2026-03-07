using System;

namespace Exanite.Core.Numerics;

/// <summary>
/// A fixed point Q48.16 value (48-bits integer, 16-bits fraction).
/// </summary>
public readonly struct Fixed : IEquatable<Fixed>, IComparable<Fixed>
{
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
