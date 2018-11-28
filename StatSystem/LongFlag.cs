using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.StatSystem.Internal
{
	/// <summary>
	/// Combines Enums into a flag system that supports more than 32/64 flags
	/// </summary>
	public class LongFlag
	{
		#region Fields and Properties

		protected BitArray flags;
		protected Dictionary<Type, int> enums;

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
		/// Dictionary of all the Enum Types and their starting indexes
		/// </summary>
		public Dictionary<Type, int> Enums
		{
			get
			{
				return enums;
			}

			protected set
			{
				enums = value;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Creates an empty LongFlag with the provided Enum Types, requires at least one type
		/// </summary>
		/// <param name="flaggableEnums">Enum Types with no negative values (use typeof())</param>
		public LongFlag(params Type[] flaggableEnums) // can pass any type right now, but preferably only types that are enums
		{
			if (flaggableEnums == null)
			{
				throw new ArgumentNullException(nameof(flaggableEnums));
			}

			AddEnumTypes(flaggableEnums);
		}

		/// <summary>
		/// Creates a LongFlag with the provided flags, automatically adding the required Enum Types <para />
		/// Requires at least one flag. Enum Types cannot be changed after initialization
		/// </summary>
		/// <param name="flags">Flags to add after creation</param>
		public LongFlag(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			List<Type> enumFlagTypes = new List<Type>();

			foreach (Enum flag in flags)
			{
				if (!enumFlagTypes.Contains(flag.GetType()))
				{
					enumFlagTypes.Add(flag.GetType());
				}
			}

			AddEnumTypes(enumFlagTypes.ToArray());

			SetFlags(true, flags);
		}

		/// <summary>
		/// Adds new Enum Types to LongFlag allowing it to set new flags
		/// </summary>
		/// <param name="types">Enum Types to add</param>
		public virtual void AddEnumTypes(params Type[] types)
		{
			if (types == null)
			{
				throw new ArgumentNullException(nameof(types));
			}

			foreach(Type type in types)
			{
				AddEnumType(type);
			}
		}

		/// <summary>
		/// Adds a new Enum Type to LongFlag allowing it to set new flags
		/// </summary>
		/// <param name="type">Enum Type to add</param>
		public virtual void AddEnumType(Type type)
		{
			if (!type.IsEnum) throw new ArgumentException(string.Format("Passed parameter {0} is not an Enum Type", type));
			if (Enums == null) Enums = new Dictionary<Type, int>();

			if (!Enums.ContainsKey(type))
			{
				int enumMax = Enum.GetValues(type).Cast<int>().Max();
				int enumMin = Enum.GetValues(type).Cast<int>().Min();

				if (enumMin < 0)
					throw new ArgumentException(string.Format("{0} must not have any negative values", type));

				Enums.Add(type, Count);

				Byte[] bits = new Byte[Count + enumMax + 1];
				if (Flags != null) Flags.CopyTo(bits, 0);
				Flags = new BitArray(bits);
			}
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

			if (index == -1) throw new ArgumentException(string.Format("{0} does not exist in {1}", flagToSet, this));

			Flags[index] = state;
		}

		/// <summary>
		/// Sets a a number of flags to a provided state, requires at least one flag
		/// </summary>
		/// <param name="state">True or false</param>
		/// <param name="flagsToSet">Enum Values of type provided in this LongFlag's constructor</param>
		public virtual void SetFlags(bool state, params Enum[] flagsToSet)
		{
			if (flagsToSet == null) throw new ArgumentException("No arguments were passed");

			foreach (Enum flag in flagsToSet)
			{
				SetFlag(state, flag);
			}
		}

		#endregion

		#region And

		/// <summary>
		/// Returns true if this LongFlag has the provided flag, false if not, requires at least one flag
		/// </summary>
		/// <param name="flag">Enum Value of type provided in this LongFlag's constructor</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlag(Enum flag)
		{
			int index = GetFlagIndex(flag);

			if (index == -1) return false;
			if (Flags[index]) return true;

			return false;
		}

		/// <summary>
		/// Returns true if this LongFlag has ALL of the provided flags, false if not, requires at least one flag
		/// </summary>
		/// <param name="flags">Enum Values of type provided in this LongFlag's constructor</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlagsAnd(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			foreach (Enum _flag in flags)
			{
				if (!HasFlag(_flag)) return false;
			}

			return true;
		}

		/// <summary>
		/// Returns true if this LongFlag has ALL of the flags in the provided LongFlag, false if not
		/// </summary>
		/// <param name="longFlag">LongFlag to compare</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlagsAnd(LongFlag longFlag)
		{
			return HasFlagsAnd(longFlag.GetAllTrueFlags().ToArray());
		}

		#endregion

		#region Or

		/// <summary>
		/// Returns true if this LongFlag has ANY of the provided flags, false if not, requires at least one flag
		/// </summary>
		/// <param name="flags">Enum Values of type provided in this LongFlag's constructor</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlagsOr(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			foreach (Enum _flag in flags)
			{
				if (HasFlag(_flag)) return true;
			}

			return false;
		}

		/// <summary>
		/// Returns true if this LongFlag has ANY of the flags in the provided LongFlag, false if not
		/// </summary>
		/// <param name="longFlag">LongFlag to compare</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlagsOr(LongFlag longFlag)
		{
			return HasFlagsOr(longFlag.GetAllTrueFlags().ToArray());
		}

		#endregion

		#region Equals

		/// <summary>
		/// Returns true if this LongFlag has only the flags provided, requires at least one flag
		/// </summary>
		/// <param name="flags">Enum Values of type provided in this LongFlag's constructor</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlagsEquals(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			BitArray passedFlags = new BitArray(Count);

			foreach(Enum _flag in flags)
			{
				passedFlags[GetFlagIndex(_flag)] = true;
			}

			return HasFlagsEquals(passedFlags);
		}

		/// <summary>
		/// Returns true if this LongFlag has only the flags provided
		/// </summary>
		/// <param name="longFlag">LongFlag to compare</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlagsEquals(LongFlag longFlag)
		{
			Type[] arrayA = longFlag.enums.Keys.ToArray();
			Type[] arrayB = enums.Keys.ToArray();
			bool firstCheckFailed = false;

			for (int i = 0; i < arrayA.Count(); i++)
			{
				if(!(arrayA[i] == arrayB[i]))
				{
					firstCheckFailed = true;
					break;
				}
			}

			if(firstCheckFailed)
			{
				List<Enum> arrayC = GetAllTrueFlags();
				List<Enum> arrayD = longFlag.GetAllTrueFlags();
				arrayC.Sort(new EnumComparer());
				arrayD.Sort(new EnumComparer());

				if (arrayC.Count() != arrayD.Count()) return false;

				for (int i = 0; i < arrayC.Count(); i++)
				{
					if ((int)(object)arrayC[i] != (int)(object)arrayD[i]) return false;
				}

				return true;
			}

			return HasFlagsEquals(longFlag.flags);
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
		public virtual void ClearBitArray()
		{
			flags.SetAll(false);
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
			return HasFlags(matchType, longFlag.GetAllTrueFlags().ToArray());
		}

		#endregion

		#endregion

		#region Internal

		#region Index logic

		/// <summary>
		/// Returns a flag's index in the internal BitArray
		/// </summary>
		/// <param name="flag">Enum Value of type provided in this LongFlag's constructor</param>
		/// <returns>Index of provided flag in BitArray, -1 if it does not exist</returns>
		protected virtual int GetFlagIndex(Enum flag)
		{
			if (!Enums.ContainsKey(flag.GetType())) return -1;

			return Enums[flag.GetType()] + (Convert.ToInt32(flag));
		}

		/// <summary>
		/// Returns a flag based on the flag's index in the internal BitArray
		/// </summary>
		/// <param name="index">Index of the flag to retrieve</param>
		/// <returns>Retrieved flag</returns>
		protected virtual Enum GetFlagFromIndex(int index)
		{
			Type type = GetTypeFromIndex(index);
			int value = (index - Enums[type]);

			return (Enum)Enum.Parse(type, value.ToString());
		}

		/// <summary>
		/// Returns the type of flag based on the flag's index
		/// </summary>
		/// <param name="index">Index of the flag to retrieve from</param>
		/// <returns>Retrieved type from flag</returns>
		protected virtual Type GetTypeFromIndex(int index)
		{
			List<KeyValuePair<Type, int>> pairs = new List<KeyValuePair<Type, int>>();

			foreach (KeyValuePair<Type, int> entry in Enums)
			{
				if (entry.Value <= index)
				{
					pairs.Add(entry);
				}
			}

			return pairs.OrderByDescending(entry => entry.Value).First().Key;
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

		#region Equals

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
				if(!(flags[i] == bitArray[i]))
				{
					return false;
				}
			}

			return true;
		}

		#endregion

		#region IComparer

		internal class EnumComparer : IComparer<Enum>
		{
			public int Compare(Enum a, Enum b)
			{
				int typeCompare = a.GetType().Name.CompareTo(b.GetType().Name);

				if (typeCompare == 0)
				{
					return a.CompareTo(b);
				}
				else
				{
					return typeCompare;
				}
			}
		}

		#endregion

		#endregion
	}
}