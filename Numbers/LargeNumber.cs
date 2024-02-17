using System;
using Exanite.Core.Utilities;
#if UNITY_2021_3_OR_NEWER
using UnityEngine;
#endif

namespace Exanite.Core.Numbers
{
    /// <summary>
    /// Used to store very large numbers (up to 999.999999x(10^(2^63)))
    /// <para/>
    /// Actual value = <see cref="Value"/> * (10 ^ (
    /// <see cref="Multiplier"/> * 3))
    /// </summary>
    [Serializable]
    public struct LargeNumber : IEquatable<LargeNumber>, IComparable<LargeNumber>
    {
#if UNITY_2021_3_OR_NEWER
        [SerializeField]
        [HideInInspector]
#endif
        private double value;
#if UNITY_2021_3_OR_NEWER
        [SerializeField]
        [HideInInspector]
#endif
        private long multiplier;

        /// <summary>
        /// Value of this <see cref="LargeNumber"/> Formatted as xxx.yyyyyy
        /// where x = significant digits and y = trailing digits
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
        /// Multiplier of this <see cref="LargeNumber"/>
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
        /// Creates a new <see cref="LargeNumber"/>
        /// </summary>
        public LargeNumber(double value = 0, long multiplier = 0)
        {
            this.value = value;
            this.multiplier = multiplier;

            ShiftPlaces();
        }

        /// <summary>
        /// Shifts the value and multiplier of this <see cref="LargeNumber"/>
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

        /// <summary>
        /// Converts this LargeNumber into a string
        /// </summary>
        public override string ToString()
        {
            return ToString(NumDisplayFormat.Scientific);
        }

        /// <summary>
        /// Converts this LargeNumber into a string
        /// </summary>
        public string ToString(NumDisplayFormat displayFormat, int placesToRound = 0)
        {
            placesToRound = Math.Clamp(placesToRound, 0, 15);

            var rounded = Math.Round(Value, placesToRound);

            if (Multiplier == 0)
            {
                return rounded.ToString();
            }

            switch (displayFormat)
            {
                case NumDisplayFormat.Scientific:
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
                case NumDisplayFormat.Short:
                {
                    if (Multiplier > EnumUtility<NumScalesShort>.Max || Multiplier < EnumUtility<NumScalesShort>.Min)
                    {
                        return ToString(NumDisplayFormat.Scientific);
                    }

                    return $"{rounded.ToString($"N{placesToRound}")} {(NumScalesShort)Multiplier}";
                }
                case NumDisplayFormat.Long:
                {
                    if (Math.Abs(Multiplier) > EnumUtility<NumScalesLong>.Max || Math.Abs(Multiplier) < EnumUtility<NumScalesLong>.Min)
                    {
                        return ToString(NumDisplayFormat.Short);
                    }

                    return $"{rounded.ToString($"N{placesToRound}")} {(NumScalesLong)Math.Abs(Multiplier)}{(Multiplier < 0 ? "th" : string.Empty)}";
                }
                default:
                {
                    throw ExceptionUtility.NotSupportedEnumValue(displayFormat);
                }
            }
        }

        public static implicit operator LargeNumber(double value)
        {
            return new LargeNumber(value);
        }

        public static LargeNumber operator *(LargeNumber A, LargeNumber B)
        {
            return new LargeNumber(A.Value * B.Value, A.Multiplier + B.Multiplier);
        }

        public static LargeNumber operator /(LargeNumber A, LargeNumber B)
        {
            return new LargeNumber(A.Value / B.Value, A.Multiplier - B.Multiplier);
        }

        public static LargeNumber operator +(LargeNumber A, LargeNumber B)
        {
            var multAIsLarger = A.Multiplier > B.Multiplier;
            var difference = Math.Abs(A.Multiplier - B.Multiplier);

            if (multAIsLarger)
            {
                for (var i = 0; i < difference; i++)
                {
                    B.value /= 1000;
                    B.multiplier++;
                }
            }
            else
            {
                for (var i = 0; i < difference; i++)
                {
                    A.value /= 1000;
                    A.multiplier++;
                }
            }

            return new LargeNumber(A.value + B.value, A.multiplier);
        }

        public static LargeNumber operator -(LargeNumber A, LargeNumber B)
        {
            var multAIsLarger = A.Multiplier > B.Multiplier;
            var difference = Math.Abs(A.Multiplier - B.Multiplier);

            if (multAIsLarger)
            {
                for (var i = 0; i < difference; i++)
                {
                    B.value /= 1000;
                    B.multiplier++;
                }
            }
            else
            {
                for (var i = 0; i < difference; i++)
                {
                    A.value /= 1000;
                    A.multiplier++;
                }
            }

            return new LargeNumber(A.value - B.value, A.multiplier);
        }

        public static LargeNumber operator ++(LargeNumber A)
        {
            return new LargeNumber(A.Value + 1, A.Multiplier);
        }

        public static LargeNumber operator --(LargeNumber A)
        {
            return new LargeNumber(A.Value - 1, A.Multiplier);
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

        public override bool Equals(object obj)
        {
            if (obj is LargeNumber largeNumber)
            {
                return Equals(largeNumber);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (Value, Multiplier).GetHashCode();
        }

        public bool Equals(LargeNumber other)
        {
            return Value == other.Value && Multiplier == other.Multiplier;
        }

        public int CompareTo(LargeNumber other)
        {
            if (Multiplier == other.Multiplier)
            {
                return Value.CompareTo(other.Value);
            }

            return Multiplier.CompareTo(other.Multiplier);
        }
    }
}
