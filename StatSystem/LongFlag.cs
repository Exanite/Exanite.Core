using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Exanite.StatSystem.Internal
{
	/// <summary>
	/// Combines Enums into a flag system that supports more than 32/64 flags
	/// </summary>
	[Serializable]
	public class LongFlag
	{
		#region Fields and Properties

		[HideInInspector] [OdinSerialize] protected BitArray flags;
		[HideInInspector] [OdinSerialize] protected Type enumType;

		protected static Dictionary<Type, IEnumerable<int>> enumValues;

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
				if (Flags == null) return 0;

				return Flags.Count;
			}
		}
		/// <summary>
		/// Enum Type this LongFlag supports
		/// </summary>
		public Type EnumType
		{
			get
			{
				return enumType;
			}

			protected set
			{
				enumType = value;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Creates an empty LongFlag with the provided Enum Type
		/// </summary>
		/// <param name="flaggableEnum">Enum Type with no negative values (use typeof())</param>
		public LongFlag(Type flaggableEnum)
		{
			if (flaggableEnum == null)
			{
				throw new ArgumentNullException(nameof(flaggableEnum));
			}

			AddEnumType(flaggableEnum);
		}

		/// <summary>
		/// Creates a LongFlag with the provided flags <para/>
		/// Requires at least one flag.
		/// </summary>
		/// <param name="flags">Flags to add after creation</param>
		public LongFlag(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			Type enumType = flags[0].GetType();

			foreach (Enum flag in flags)
			{
				if (flag.GetType() != enumType)
				{
					throw new ArgumentException("More than one Enum Type was passed");
				}
			}

			AddEnumType(enumType);

			SetFlags(true, flags);
		}

		/// <summary>
		/// Internally used to create a new LongFlag
		/// </summary>
		/// <param name="type">Enum Type used to create the LongFlag</param>
		protected virtual void AddEnumType(Type type)
		{
			if (!type.IsEnum) throw new ArgumentException(string.Format("Passed parameter {0} is not an Enum Type", type));
			if(enumValues == null)
			{
				enumValues = new Dictionary<Type, IEnumerable<int>>();
			}

			if(!enumValues.ContainsKey(type))
			{
				enumValues.Add(type, Enum.GetValues(type).Cast<int>());
			}

			int enumMax = enumValues[type].Max();
			int enumMin = enumValues[type].Min();

			if (enumMin < 0)
				throw new ArgumentException(string.Format("{0} must not have any negative values", type));

			Flags = new BitArray(enumMax + 1);

			EnumType = type;
		}

		#endregion

		#region Flag logic

		#region Setting Flags

		/// <summary>
		/// Sets a flag to a provided state
		/// </summary>
		/// <param name="state">True or false</param>
		/// <param name="flagToSet">Enum Value of type provided in this LongFlag's constructor</param>
		public virtual void SetFlag(bool state, Enum flagToSet)
		{
			int index = GetFlagIndex(flagToSet);

			Flags[index] = state;
		}

		/// <summary>
		/// Sets a a number of flags to a provided state, requires at least one flag
		/// </summary>
		/// <param name="state">True or false</param>
		/// <param name="flagsToSet">Enum Values of type provided in this LongFlag's constructor</param>
		public virtual void SetFlags(bool state, params Enum[] flagsToSet)
		{
			if (flagsToSet == null)
			{
				throw new ArgumentNullException(nameof(flagsToSet));
			}

			foreach (Enum flag in flagsToSet)
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
		public virtual bool HasFlag(Enum flag)
		{
			return Flags[GetFlagIndex(flag)];
		}

		/// <summary>
		/// Returns true if this LongFlag matches the flags in the provided LongFlag with the provided match type
		/// </summary>
		/// <param name="matchType">How to match the flags</param>
		/// <param name="flags">Flags to compare</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlags(FlagMatchType matchType, params Enum[] flags)
		{
			switch (matchType)
			{
				case (FlagMatchType.And):
					return HasFlagsAnd(flags);
				case (FlagMatchType.Or):
					return HasFlagsOr(flags);
				case (FlagMatchType.Equals):
					return HasFlagsEquals(flags);
				default:
					throw new ArgumentOutOfRangeException($"{matchType} does not have a code path");
			}
		}

		/// <summary>
		/// Returns true if this LongFlag matches the flags provided with the provided match type
		/// </summary>
		/// <param name="matchType">How to match the flags</param>
		/// <param name="longFlag">LongFlag to compare</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlags(FlagMatchType matchType, LongFlag longFlag)
		{
			switch (matchType)
			{
				case (FlagMatchType.And):
					return HasFlagsAnd(longFlag);
				case (FlagMatchType.Or):
					return HasFlagsOr(longFlag);
				case (FlagMatchType.Equals):
					return HasFlagsEquals(longFlag);
				default:
					throw new ArgumentOutOfRangeException($"{matchType} does not have a code path");
			}
		}

		#endregion

		#region Other

		/// <summary>
		/// Returns a list of all flags with the value 'true'
		/// </summary>
		/// <returns>List of all flags with the value, true</returns>
		public virtual List<Enum> GetAllTrueFlags()
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

		/// <summary>
		/// Returns true if both LongFlags have the same EnumType
		/// </summary>
		/// <param name="flag">LongFlag to compare</param>
		/// <returns>True or false</returns>
		public virtual bool IsSameType(LongFlag flag)
		{
			return flag.EnumType == EnumType;
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
		protected virtual bool HasFlagsAnd(params Enum[] flags)
		{
			foreach (Enum _flag in flags)
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
		protected virtual bool HasFlagsOr(params Enum[] flags)
		{
			foreach (Enum _flag in flags)
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
		protected virtual bool HasFlagsEquals(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			BitArray passedFlags = new BitArray(Count);

			foreach (Enum _flag in flags)
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
		protected virtual bool HasFlagsAnd(LongFlag longFlag)
		{
			return HasFlagsAnd(longFlag.Flags);
		}

		/// <summary>
		/// Returns true if this LongFlag has ALL of the flags in the provided LongFlag, false if not
		/// </summary>
		/// <param name="longFlag">LongFlag to compare</param>
		/// <returns>True or false</returns>
		protected virtual bool HasFlagsOr(LongFlag longFlag)
		{
			return HasFlagsOr(longFlag.Flags);
		}

		/// <summary>
		/// Returns true if this LongFlag has ALL of the flags in the provided LongFlag, false if not
		/// </summary>
		/// <param name="longFlag">LongFlag to compare</param>
		/// <returns>True or false</returns>
		protected virtual bool HasFlagsEquals(LongFlag longFlag)
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
		protected virtual int GetFlagIndex(Enum flag)
		{
			if (flag.GetType() != EnumType)
			{
				throw new ArgumentException("Provided flag is not supported by this LongFlag because its type was not defined in the constructor");
			}

			return (int)(object)flag;
		}

		/// <summary>
		/// Returns a flag based on the flag's index in the internal BitArray
		/// </summary>
		/// <param name="index">Index of the flag to retrieve</param>
		/// <returns>Retrieved flag</returns>
		protected virtual Enum GetFlagFromIndex(int index)
		{
			//return (Enum)Enum.Parse(EnumType, index.ToString()); // Slower because of garbage alloc
			return (Enum)Enum.ToObject(EnumType, index);
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
		protected virtual List<Enum> GetAllFlagsOfIndex(params int[] indexes)
		{
			List<Enum> returnEnums = new List<Enum>();
			
			foreach(int index in indexes)
			{
				returnEnums.Add(GetFlagFromIndex(index));
			}

			return returnEnums;
		}

		#endregion

		#endregion
	}
}