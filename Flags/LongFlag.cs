﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;
using Exanite.Utility;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Exanite.Flags
{
    /// <summary>
    /// Combines Enums into a flag system that supports more than 32/64 flags
    /// </summary>
    [Serializable]
    public class LongFlag<T> : ISerializationCallbackReceiver where T : struct, IComparable, IConvertible, IFormattable
    {
        #region Fields and Properties

        [HideInInspector]
        [OdinSerialize]
        private BitArray flags;

        /// <summary>
        /// BitArray with all the stored flags
        /// </summary>
        public BitArray Flags
        {
            get
            {
                return flags;
            }

            protected set
            {
                flags = value;
            }
        }
        /// <summary>
        /// How many flags are stored
        /// </summary>
        public int Count
        {
            get
            {
                return Flags.Count;
            }
        }
        protected static EnumData<T> EnumData
        {
            get
            {
                return EnumData<T>.Instance;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a LongFlag with the provided BitArray <para/>
        /// NOTE: Don't use unless you know what you are doing
        /// </summary>
        /// <param name="bitArray">BitArray of same length as the Enum Type</param>
        public LongFlag(BitArray bitArray)
        {
            InitiateLongFlag();

            for (int i = 0; i < Count; i++)
            {
                flags[i] = bitArray[i];
            }
        }

        /// <summary>
        /// Creates an empty LongFlag
        /// </summary>
        public LongFlag()
        {
            InitiateLongFlag();
        }

        /// <summary>
        /// Creates a LongFlag with the provided flags
        /// </summary>
        /// <param name="flags">Flags to add</param>
        public LongFlag(params T[] flags)
        {
            InitiateLongFlag();

            if (flags != null)
            {
                SetFlags(true, flags);
            }
        }

        /// <summary>
        /// Internally used to create a new LongFlag
        /// </summary>
        /// <param name="type">Enum Type used to create the LongFlag</param>
        protected virtual void InitiateLongFlag()
        {
            if (EnumData.Min < 0)
                throw new ArgumentException(string.Format("{0} must not have any negative values", typeof(T)));

            Flags = new BitArray(EnumData.Max + 1);
        }

        #endregion

        #region Flag logic

        #region Setting Flags

        /// <summary>
        /// Sets a flag to a provided state
        /// </summary>
        /// <param name="state">True or false</param>
        /// <param name="flagToSet">Enum Value of type provided in this LongFlag's constructor</param>
        public virtual void SetFlag(bool state, T flagToSet)
        {
            int index = GetFlagIndex(flagToSet);

            Flags[index] = state;
        }

        /// <summary>
        /// Sets a a number of flags to a provided state, requires at least one flag
        /// </summary>
        /// <param name="state">True or false</param>
        /// <param name="flagsToSet">Enum Values of type provided in this LongFlag's constructor</param>
        public virtual void SetFlags(bool state, params T[] flagsToSet)
        {
            if (flagsToSet == null)
            {
                throw new ArgumentNullException(nameof(flagsToSet));
            }

            foreach (T flag in flagsToSet)
            {
                SetFlag(state, flag);
            }
        }

        #endregion

        #region Has Flags

        /// <summary>
        /// Returns true if this LongFlag has the provided flag
        /// </summary>
        /// <param name="flag">Enum Value of type provided in this LongFlag's constructor</param>
        /// <returns>True or false</returns>
        public virtual bool HasFlag(T flag)
        {
            return Flags[GetFlagIndex(flag)];
        }

        /// <summary>
        /// Returns true if this LongFlag matches the flags in the provided LongFlag with the provided match type
        /// </summary>
        /// <param name="matchType">How to match the flags</param>
        /// <param name="flags">Flags to compare</param>
        /// <returns>True or false</returns>
        public virtual bool HasFlags(FlagMatchType matchType, params T[] flags)
        {
            switch (matchType)
            {
                case (FlagMatchType.And):
                {
                    return HasFlagsAnd(flags);
                }
                case (FlagMatchType.Or):
                {
                    return HasFlagsOr(flags);
                }
                case (FlagMatchType.Equals):
                {
                    return HasFlagsEquals(flags);
                }
                default:
                {
                    throw new ArgumentOutOfRangeException($"{matchType} does not have a code path");
                }
            }
        }

        /// <summary>
        /// Returns true if this LongFlag matches the flags provided with the provided match type
        /// </summary>
        /// <param name="matchType">How to match the flags</param>
        /// <param name="longFlag">LongFlag to compare</param>
        /// <returns>True or false</returns>
        public virtual bool HasFlags(FlagMatchType matchType, LongFlag<T> longFlag)
        {
            switch (matchType)
            {
                case (FlagMatchType.And):
                {
                    return HasFlagsAnd(longFlag);
                }
                case (FlagMatchType.Or):
                {
                    return HasFlagsOr(longFlag);
                }
                case (FlagMatchType.Equals):
                {
                    return HasFlagsEquals(longFlag);
                }
                default:
                {
                    throw new ArgumentOutOfRangeException($"{matchType} does not have a code path");
                }
            }
        }

        #endregion

        #region Other

        /// <summary>
        /// Returns a list of all flags with the value 'true'
        /// </summary>
        /// <returns>List of all flags with the value, true</returns>
        public virtual List<T> GetAllTrueFlags()
        {
            return GetAllFlagsOfIndex(GetAllTrueIndexes().ToArray());
        }

        /// <summary>
        /// Clears the internal BitArray of all flags
        /// </summary>
        public virtual void ClearFlags()
        {
            flags.SetAll(false);
        }

        #endregion

        #endregion

        #region Internal

        #region Flag Logic

        /// <summary>
        /// Returns true if this LongFlag has ALL of the provided flags <para/>
        /// Requires at least one flag
        /// </summary>
        /// <param name="flags">Enum Values of type provided in this LongFlag's constructor</param>
        /// <returns>True or false</returns>
        protected virtual bool HasFlagsAnd(params T[] flags)
        {
            foreach (T _flag in flags)
            {
                if (!HasFlag(_flag)) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if this LongFlag has ANY of the provided flags, false if not, requires at least one flag
        /// </summary>
        /// <param name="flags">Enum Values of type provided in this LongFlag's constructor</param>
        /// <returns>True or false</returns>
        protected virtual bool HasFlagsOr(params T[] flags)
        {
            foreach (T _flag in flags)
            {
                if (HasFlag(_flag)) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if this LongFlag has only the flags provided <para/>
        /// Requires at least one flag
        /// </summary>
        /// <param name="flags">Enum Values of type provided in this LongFlag's constructor</param>
        /// <returns>True or false</returns>
        protected virtual bool HasFlagsEquals(params T[] flags)
        {
            if (flags == null)
            {
                throw new ArgumentNullException(nameof(flags));
            }

            BitArray passedFlags = new BitArray(Count);

            foreach (T _flag in flags)
            {
                passedFlags[GetFlagIndex(_flag)] = true;
            }

            return HasFlagsEquals(passedFlags);
        }

        /// <summary>
        /// Returns true if this LongFlag has ALL of the flags in the provided LongFlag, false if not
        /// </summary>
        /// <param name="longFlag">LongFlag to compare</param>
        /// <returns>True or false</returns>
        protected virtual bool HasFlagsAnd(LongFlag<T> longFlag)
        {
            return HasFlagsAnd(longFlag.Flags);
        }

        /// <summary>
        /// Returns true if this LongFlag has ALL of the flags in the provided LongFlag, false if not
        /// </summary>
        /// <param name="longFlag">LongFlag to compare</param>
        /// <returns>True or false</returns>
        protected virtual bool HasFlagsOr(LongFlag<T> longFlag)
        {
            return HasFlagsOr(longFlag.Flags);
        }

        /// <summary>
        /// Returns true if this LongFlag has ALL of the flags in the provided LongFlag, false if not
        /// </summary>
        /// <param name="longFlag">LongFlag to compare</param>
        /// <returns>True or false</returns>
        protected virtual bool HasFlagsEquals(LongFlag<T> longFlag)
        {
            return HasFlagsEquals(longFlag.Flags);
        }

        /// <summary>
        /// Returns true if this LongFlag has all of the flags provided
        /// </summary>
        /// <param name="bitArray">BitArray of same length as this LongFlag flags BitArray</param>
        /// <returns>True or false</returns>
        protected virtual bool HasFlagsAnd(BitArray bitArray)
        {
            if (!(flags.Count == bitArray.Count)) return false;

            for (int i = 0; i < flags.Count; i++)
            {
                if (!bitArray[i]) continue;

                if (!flags[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if this LongFlag has any of the flags provided
        /// </summary>
        /// <param name="bitArray">BitArray of same length as this LongFlag flags BitArray</param>
        /// <returns>True or false</returns>
        protected virtual bool HasFlagsOr(BitArray bitArray)
        {
            if (!(flags.Count == bitArray.Count)) return false;

            for (int i = 0; i < flags.Count; i++)
            {
                if (!bitArray[i]) continue;

                if (flags[i])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if this LongFlag has only the flags provided
        /// </summary>
        /// <param name="bitArray">BitArray of same length as this LongFlag flags BitArray</param>
        /// <returns>True or false</returns>
        protected virtual bool HasFlagsEquals(BitArray bitArray)
        {
            if (!(flags.Count == bitArray.Count)) return false;

            for (int i = 0; i < flags.Count; i++)
            {
                if (!(flags[i] == bitArray[i]))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Index logic

        /// <summary>
        /// Returns a flag's index in the internal BitArray
        /// </summary>
        /// <param name="flag">Enum Value of type provided in this LongFlag's constructor</param>
        /// <returns>Index of provided flag in BitArray</returns>
        protected virtual int GetFlagIndex(T flag)
        {
            return (int)(object)flag;
        }

        /// <summary>
        /// Returns a flag based on the flag's index in the internal BitArray
        /// </summary>
        /// <param name="index">Index of the flag to retrieve</param>
        /// <returns>Retrieved flag</returns>
        protected virtual T GetFlagFromIndex(int index)
        {
            return (T)(object)index;
        }

        /// <summary>
        /// Returns a List of the indexes of all true values in the internal BitArray
        /// </summary>
        /// <returns>List of the indexes with true values in the internal BitArray</returns>
        protected virtual List<int> GetAllTrueIndexes()
        {
            List<int> indexes = new List<int>();

            for (int i = 0; i < Flags.Count; i++)
            {
                if (Flags[i] == true)
                {
                    indexes.Add(i);
                }
            }

            return indexes;
        }

        /// <summary>
        /// Returns a list of flags based on the passed array of flag indexes
        /// </summary>
        /// <param name="indexes">Indexes of the flags to retrieve</param>
        /// <returns>Retrieved list of flags</returns>
        protected virtual List<T> GetAllFlagsOfIndex(params int[] indexes)
        {
            List<T> returnEnums = new List<T>();

            foreach (int index in indexes)
            {
                returnEnums.Add(GetFlagFromIndex(index));
            }

            return returnEnums;
        }

        #endregion

        #endregion

        #region Serialization

        #region CallBacks

        #region Fields

        [HideInInspector]
        [SerializeField]
        private string bitArrayData;

        [SerializeField]
        [HideInInspector]
        private List<string> lastEnumValueData;

        [SerializeField]
#if ODIN_INSPECTOR
        [ShowIf("MissingEnumsIsNotEmptyOrNull")]
        [ReadOnly]
#endif
        private List<string> missingEnums;

        #endregion

        /// <summary>
        /// Prepares the class for serialization
        /// </summary>
        public virtual void OnBeforeSerialize()
        {
            #region BitArray

            bitArrayData = "";

            if (flags == null) return;

            for (int i = 0; i < flags.Count; i++)
            {
                bitArrayData += flags[i] ? '1' : '0';
            }

            #endregion

            #region Enum Values

            lastEnumValueData = EnumData.LastEnumValueData;

            if (missingEnums == null) missingEnums = new List<string>();

            #endregion
        }

        /// <summary>
        /// Retrieves serialized data and puts it into an usable form
        /// </summary>
        public virtual void OnAfterDeserialize()
        {
            #region BitArray

            flags = new BitArray(bitArrayData.Length);

            for (int i = 0; i < bitArrayData.Length; i++)
            {
                flags[i] = bitArrayData[i] == '1' ? true : false;
            }

            #endregion

            #region Enum Values

            if (!lastEnumValueData.SequenceEqual(EnumData.LastEnumValueData))
            {
                RepairBitArray();
            }

            #endregion
        }

        #endregion

        #region Repair BitArray

        /// <summary>
        /// Repairs the BitArray when the Enum changes after serialization
        /// </summary>
        private void RepairBitArray()
        {
            List<string> oldValues = lastEnumValueData;
            List<string> newValues = EnumData.LastEnumValueData;
            // Gets all the true values in the old BitArray
            List<int> oldIndexes = GetAllTrueIndexes();

            flags = new BitArray(EnumData.Max + 1);

            foreach (int oldIndex in oldIndexes)
            {
                // Checks if the value in the old BitArray has a corresponding bit in the new BitArray
                int newIndex = newValues.IndexOf(oldValues[oldIndex]);
                if (newIndex > -1)
                {
                    // If the BitArray has the corresponding bit, set it to true
                    flags[newIndex] = true;
                }
                else
                {
                    // Else store it in a separate list so that it can be readded once the Enum has readded it
                    missingEnums.Add($"{oldValues[oldIndex]}");
                }
            }

            // Check if missing bits can be readded
            for (int i = missingEnums.Count; i-- > 0;)
            {
                int index = newValues.IndexOf(missingEnums[i]);
                if (index > -1)
                {
                    flags[index] = true;
                    missingEnums.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Odin Inspector

#if ODIN_INSPECTOR
        private bool MissingEnumsIsNotEmptyOrNull()
        {
            if (missingEnums == null)
            {
                return false;
            }
            return missingEnums.Count > 0;
        }
#endif

        #endregion

        #endregion
    }
}