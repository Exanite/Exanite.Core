using System;
using UnityEngine;
using Exanite.Utility;

namespace Exanite.Numbers
{
    /// <summary>
    /// Used to store very large numbers (up to 999.999999x(10^(3*2^63))) <para/>
    /// Actual value = <see cref="Value"/> * (10 ^ (<see cref="Multiplier"/> * 3))
    /// </summary>
    public struct LargeNumber
    {
        #region Fields and Properties

        /// <summary>
        /// The way <see cref="ToString"/> will format the string by default <para/>
        /// If <see cref="ToString"/> cannot format the string in the way specified, it will use the next lowest NumDisplayFormat to format the string
        /// </summary>
        public NumDisplayFormat DisplayFormat;

        [SerializeField]
        private double value;
        [SerializeField]
        private long multiplier;
        [SerializeField]
        [Range(0, 15)]
        private int placesToRound;

        private static EnumData<NumScalesShort> shortEnumData;
        private static EnumData<NumScalesLong> longEnumData;

        /// <summary>
        /// Readonly value of this <see cref="LargeNumber"/>
        /// Formatted as xxx.yyyyyy where x = significant digits and y = trailing digits
        /// </summary>
        public double Value
        {
            get
            {
                AutoShift();
                return value;
            }

            set
            {
                this.value = value;
                AutoShift();
            }
        }
        /// <summary>
        /// Multiplier of this <see cref="LargeNumber"/>
        /// </summary>
        public long Multiplier
        {
            get
            {
                AutoShift();
                return multiplier;
            }

            set
            {
                multiplier = value;
            }
        }
        /// <summary>
        /// Places to round to when <see cref="ToString"/> is called
        /// </summary>
        public int PlacesToRound
        {
            get
            {
                return placesToRound;
            }

            set
            {
                placesToRound = Mathf.Clamp(value, 0, 15);
            }
        }

        private static EnumData<NumScalesShort> ShortEnumData
        {
            get
            {
                if(shortEnumData == null)
                {
                    shortEnumData = new EnumData<NumScalesShort>();
                }
                return shortEnumData;
            }
        }
        private static EnumData<NumScalesLong> LongEnumData
        {
            get
            {
                if (longEnumData == null)
                {
                    longEnumData = new EnumData<NumScalesLong>();
                }
                return longEnumData;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="LargeNumber"/> with the provided parameters
        /// </summary>
        /// <param name="value">
        /// Value of the <see cref="LargeNumber"/><para/>
        /// In format of: xxx.yyyyyy where x = significant digits and y = trailing digits
        /// </param>
        /// <param name="multiplier">
        /// Multiplier of the <see cref="LargeNumber"/>
        /// <para/>See: <see cref="NumScalesLong"/> for what multipliers correspond to what values
        /// </param>
        /// <param name="placesToRound">Places to round to when <see cref="ToString"/> is called</param>
        /// <param name="displayFormat">How to format the string when <see cref="ToString"/> is called</param>
        public LargeNumber(double value = 0, long multiplier = 0, int placesToRound = 2, NumDisplayFormat displayFormat = NumDisplayFormat.Scientific)
        {
            this.value = value;
            this.multiplier = multiplier;
            this.placesToRound = placesToRound;
            DisplayFormat = displayFormat;

            AutoShift();
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return ToString(DisplayFormat);
        }

        public string ToString(NumDisplayFormat displayFormat)
        {
            double rounded = Math.Round(Value, PlacesToRound);

            if(Multiplier == 0)
            {
                return rounded.ToString();
            }

            switch (displayFormat)
            {
                case (NumDisplayFormat.Scientific):
                {
                    int extraDigits = 0;
                    while(rounded > 10) // Limit to one leading digit
                    {
                        rounded /= 10;
                        extraDigits += 1;
                    }
                    rounded = Math.Round(rounded, PlacesToRound); // Round the result again because the decimal place shifted in the while loop

                    return $"{rounded} E{(Multiplier * 3) + extraDigits}";
                }
                case (NumDisplayFormat.Short):
                {
                    if (Multiplier > ShortEnumData.max || Multiplier < ShortEnumData.min)
                    {
                        return ToString(NumDisplayFormat.Scientific);
                    }
                    return $"{rounded} {(NumScalesShort)Multiplier}";
                }
                case (NumDisplayFormat.Long):
                {
                    if(Math.Abs(Multiplier) > LongEnumData.max || Math.Abs(Multiplier) < LongEnumData.min)
                    {
                        return ToString(NumDisplayFormat.Short);
                    }
                    return (Multiplier < 0) ? $"{rounded} {(NumScalesLong)Math.Abs(Multiplier)}th" : $"{rounded} {(NumScalesLong)Math.Abs(Multiplier)}";
                }
                default:
                {
                    throw new ArgumentOutOfRangeException($"DisplayFormat {DisplayFormat} is not an implemented display format");
                }
            }
        }

        #endregion

        #region Operators

        #region Assignment

        public static implicit operator LargeNumber(double value)
        {
            return new LargeNumber(value);
        }

        #endregion

        #region Basic

        public static LargeNumber operator *(LargeNumber A, LargeNumber B)
        {
            return new LargeNumber(A.Value * B.Value, A.Multiplier + B.Multiplier, A.PlacesToRound, A.DisplayFormat);
        }

        public static LargeNumber operator /(LargeNumber A, LargeNumber B)
        {
            return new LargeNumber(A.Value / B.Value, A.Multiplier - B.Multiplier, A.PlacesToRound, A.DisplayFormat);
        }

        public static LargeNumber operator +(LargeNumber A, LargeNumber B)
        {
            bool AMultIsLarger = (A.Multiplier > B.Multiplier);
            long difference = Math.Abs(A.Multiplier - B.Multiplier);

            if (AMultIsLarger)
            {
                for (int i = 0; i < difference; i++)
                {
                    B.value /= 1000;
                    B.multiplier++;
                }
            }
            else
            {
                for (int i = 0; i < difference; i++)
                {
                    A.value /= 1000;
                    A.multiplier++;
                }
            }

            return new LargeNumber(A.value + B.value, A.multiplier, A.PlacesToRound, A.DisplayFormat);
        }

        public static LargeNumber operator -(LargeNumber A, LargeNumber B)
        {
            bool AMultIsLarger = (A.Multiplier > B.Multiplier);
            long difference = Math.Abs(A.Multiplier - B.Multiplier);

            if (AMultIsLarger)
            {
                for (int i = 0; i < difference; i++)
                {
                    B.value /= 1000;
                    B.multiplier++;
                }
            }
            else
            {
                for (int i = 0; i < difference; i++)
                {
                    A.value /= 1000;
                    A.multiplier++;
                }
            }

            return new LargeNumber(A.value - B.value, A.multiplier, A.PlacesToRound, A.DisplayFormat);
        }

        public static LargeNumber operator ++(LargeNumber A)
        {
            return new LargeNumber(A.Value + 1, A.Multiplier, A.PlacesToRound, A.DisplayFormat);
        }

        public static LargeNumber operator --(LargeNumber A)
        {
            return new LargeNumber(A.Value - 1, A.Multiplier, A.PlacesToRound, A.DisplayFormat);
        }

        #endregion

        #region Comparison

        public static bool operator ==(LargeNumber A, LargeNumber B)
        {
            return (A.Value == B.Value && A.Multiplier == A.Multiplier);
        }

        public static bool operator !=(LargeNumber A, LargeNumber B)
        {
            return !(A == B);
        }

        public static bool operator >(LargeNumber A, LargeNumber B)
        {
            if(A.Multiplier == B.multiplier)
            {
                return A.Value > B.Value;
            }
            else
            {
                return A.Multiplier > B.Multiplier;
            }
        }

        public static bool operator <(LargeNumber A, LargeNumber B)
        {
            if (A.Multiplier == B.multiplier)
            {
                return A.Value < B.Value;
            }
            else
            {
                return A.Multiplier < B.Multiplier;
            }
        }

        public static bool operator >=(LargeNumber A, LargeNumber B)
        {
            return (A > B || A == B);
        }

        public static bool operator <=(LargeNumber A, LargeNumber B)
        {
            return (A < B || A == B);
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #endregion

        #region Internal

        private void AutoShift()
        {
            while (Math.Abs(value) >= 1000) // More than 1000
            {
                value /= 1000;
                multiplier++;
            }
            while (Math.Abs(value) > 0 && Math.Abs(value) < 1) // Between 0 and 1
            {
                value *= 1000;
                multiplier--;
            }
            if(value == 0)
            {
                multiplier = 0;
            }
        }

        #endregion
    }
}