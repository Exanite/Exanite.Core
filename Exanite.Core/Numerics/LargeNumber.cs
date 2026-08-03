using System;
using System.Globalization;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// Used to store very large numbers (up to 999.999999x(10^(2^63))).
/// <para/>
/// Actual value = <see cref="Value"/> * (10 ^ (
/// <see cref="Multiplier"/> * 3)).
/// </summary>
public struct LargeNumber : IEquatable<LargeNumber>, IComparable<LargeNumber>
{
    private double value;
    private long multiplier;

    /// <summary>
    /// Value of this <see cref="LargeNumber"/> Formatted as xxx.yyyyyy
    /// where x = significant digits and y = trailing digits.
    /// </summary>
    public double Value
    {
        get
        {
            ShiftPlaces();

            return value;
        }

        set
        {
            this.value = value;
            ShiftPlaces();
        }
    }

    /// <summary>
    /// Multiplier of this <see cref="LargeNumber"/>.
    /// </summary>
    public long Multiplier
    {
        get
        {
            ShiftPlaces();

            return multiplier;
        }

        set => multiplier = value;
    }

    /// <summary>
    /// Creates a new <see cref="LargeNumber"/>.
    /// </summary>
    public LargeNumber(double value = 0, long multiplier = 0)
    {
        this.value = value;
        this.multiplier = multiplier;

        ShiftPlaces();
    }

    /// <summary>
    /// Shifts the value and multiplier of this <see cref="LargeNumber"/>.
    /// </summary>
    private void ShiftPlaces()
    {
        while (Math.Abs(value) >= 1000) // More than 1000 or less than -1000
        {
            value /= 1000;
            multiplier++;
        }

        while (Math.Abs(value) < 1) // Between -1 and 1
        {
            value *= 1000;
            multiplier--;
        }

        if (value == 0)
        {
            multiplier = 0;
        }
    }

    public static implicit operator LargeNumber(double value)
    {
        return new LargeNumber(value);
    }

    public static LargeNumber operator *(LargeNumber a, LargeNumber b)
    {
        return new LargeNumber(a.Value * b.Value, a.Multiplier + b.Multiplier);
    }

    public static LargeNumber operator /(LargeNumber a, LargeNumber b)
    {
        return new LargeNumber(a.Value / b.Value, a.Multiplier - b.Multiplier);
    }

    public static LargeNumber operator +(LargeNumber a, LargeNumber b)
    {
        var multAIsLarger = a.Multiplier > b.Multiplier;
        var difference = Math.Abs(a.Multiplier - b.Multiplier);

        if (multAIsLarger)
        {
            for (var i = 0; i < difference; i++)
            {
                b.value /= 1000;
                b.multiplier++;
            }
        }
        else
        {
            for (var i = 0; i < difference; i++)
            {
                a.value /= 1000;
                a.multiplier++;
            }
        }

        return new LargeNumber(a.value + b.value, a.multiplier);
    }

    public static LargeNumber operator -(LargeNumber a, LargeNumber b)
    {
        var multAIsLarger = a.Multiplier > b.Multiplier;
        var difference = Math.Abs(a.Multiplier - b.Multiplier);

        if (multAIsLarger)
        {
            for (var i = 0; i < difference; i++)
            {
                b.value /= 1000;
                b.multiplier++;
            }
        }
        else
        {
            for (var i = 0; i < difference; i++)
            {
                a.value /= 1000;
                a.multiplier++;
            }
        }

        return new LargeNumber(a.value - b.value, a.multiplier);
    }

    public static LargeNumber operator ++(LargeNumber a)
    {
        return new LargeNumber(a.Value + 1, a.Multiplier);
    }

    public static LargeNumber operator --(LargeNumber a)
    {
        return new LargeNumber(a.Value - 1, a.Multiplier);
    }

    public static bool operator ==(LargeNumber lhs, LargeNumber rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(LargeNumber lhs, LargeNumber rhs)
    {
        return !lhs.Equals(rhs);
    }

    public static bool operator >(LargeNumber lhs, LargeNumber rhs)
    {
        return lhs.CompareTo(rhs) > 0;
    }

    public static bool operator <(LargeNumber lhs, LargeNumber rhs)
    {
        return lhs.CompareTo(rhs) < 0;
    }

    public static bool operator >=(LargeNumber lhs, LargeNumber rhs)
    {
        return lhs.CompareTo(rhs) >= 0;
    }

    public static bool operator <=(LargeNumber lhs, LargeNumber rhs)
    {
        return lhs.CompareTo(rhs) <= 0;
    }

    public int CompareTo(LargeNumber other)
    {
        if (Multiplier == other.Multiplier)
        {
            return Value.CompareTo(other.Value);
        }

        return Multiplier.CompareTo(other.Multiplier);
    }

    public override bool Equals(object? obj)
    {
        if (obj is LargeNumber largeNumber)
        {
            return Equals(largeNumber);
        }

        return false;
    }

    public bool Equals(LargeNumber other)
    {
        return Math.Abs(Value - other.Value) < float.Epsilon && Multiplier == other.Multiplier;
    }

    public override int GetHashCode()
    {
        return (Value, Multiplier).GetHashCode();
    }

    public override string ToString()
    {
        return ToString(NumberDisplayFormat.Scientific);
    }

    public string ToString(NumberDisplayFormat displayFormat, int placesToRound = 0)
    {
        placesToRound = Math.Clamp(placesToRound, 0, 15);

        var rounded = Math.Round(Value, placesToRound);

        if (Multiplier == 0)
        {
            return rounded.ToString(CultureInfo.CurrentCulture);
        }

        switch (displayFormat)
        {
            case NumberDisplayFormat.Scientific:
            {
                var extraDigits = 0;

                while (rounded >= 10) // Limit to one leading digit
                {
                    extraDigits++;
                    rounded /= 10;
                }

                while (rounded <= -10) // Limit to one leading digit
                {
                    extraDigits--;
                    rounded /= 10;
                }

                rounded = Math.Round(rounded, placesToRound); // Round the result again because the decimal place shifted in the while loop

                if (Math.Abs(Multiplier) > long.MaxValue / 3)
                {
                    var isNegative = false;

                    if (Multiplier < 0)
                    {
                        return "0";
                    }

                    if (Value < 0)
                    {
                        isNegative = true;
                    }

                    return $"{(isNegative ? "-" : string.Empty)}Infinity";
                }

                return $"{rounded.ToString($"N{placesToRound}")} E{Multiplier * 3 + extraDigits}";
            }
            case NumberDisplayFormat.Short:
            {
                if (Multiplier > EnumUtility<ShortNumberScales>.Max || Multiplier < EnumUtility<ShortNumberScales>.Min)
                {
                    return ToString(NumberDisplayFormat.Scientific);
                }

                return $"{rounded.ToString($"N{placesToRound}")} {(ShortNumberScales)Multiplier}";
            }
            case NumberDisplayFormat.Long:
            {
                if (Math.Abs(Multiplier) > EnumUtility<LongNumberScales>.Max || Math.Abs(Multiplier) < EnumUtility<LongNumberScales>.Min)
                {
                    return ToString(NumberDisplayFormat.Short);
                }

                return $"{rounded.ToString($"N{placesToRound}")} {(LongNumberScales)Math.Abs(Multiplier)}{(Multiplier < 0 ? "th" : string.Empty)}";
            }
            default:
            {
                return ExceptionUtility.ThrowNotSupported<string>(displayFormat);
            }
        }
    }
}
