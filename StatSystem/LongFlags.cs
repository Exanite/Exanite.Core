using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.StatSystem.Internal
{
	/// <summary>
	/// Combines Enums into a flag system that supports more than 32/64 flags
	/// </summary>
	public class LongFlags
	{
		#region Fields and Properties

		protected BitArray flags;
		protected Dictionary<Type, int> enums;

		public int Count
		{
			get
			{
				return flags.Count;
			}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a LongFlags with the provided Enum Types, requires at least one type
		/// </summary>
		/// <param name="flaggableEnums">Enum Types with no negative values (use typeof())</param>
		public LongFlags(params Type[] flaggableEnums) // can pass any type right now, but preferably only types that are enums
		{
			if (flaggableEnums == null) throw new ArgumentException("No arguments were passed");

			int bitsToAdd = 0;

			enums = new Dictionary<Type, int>();

			foreach (Type enumToAdd in flaggableEnums)
			{
				if (!enumToAdd.IsEnum) throw new ArgumentException(string.Format("Passed parameter {0} is not an Enum", enumToAdd));

				if (!enums.ContainsKey(enumToAdd))
				{
					int enumMax = Enum.GetValues(enumToAdd).Cast<int>().Max();
					int enumMin = Enum.GetValues(enumToAdd).Cast<int>().Min();

					if (enumMin < 0)
						throw new ArgumentException(string.Format("{0} must not have any negative values", enumToAdd));

					enums.Add(enumToAdd, bitsToAdd);
					bitsToAdd += enumMax + 1;
				}
			}

			flags = new BitArray(bitsToAdd);
		}

		#endregion

		#region Flag logic

		/// <summary>
		/// Sets a flag to a provided state
		/// </summary>
		/// <param name="state">True or false</param>
		/// <param name="flagToSet">Enum Value of type provided in this LongFlags's constructor</param>
		public virtual void SetFlag(bool state, Enum flagToSet)
		{
			flags[GetFlagIndex(flagToSet)] = state;
		}

		/// <summary>
		/// Sets a a number of flags to a provided state, requires at least one flag
		/// </summary>
		/// <param name="state">True or false</param>
		/// <param name="flagsToSet">Enum Values of type provided in this LongFlags's constructor</param>
		public virtual void SetFlags(bool state, params Enum[] flagsToSet)
		{
			if (flagsToSet == null) throw new ArgumentException("No arguments were passed");

			foreach (Enum flag in flagsToSet)
			{
				SetFlag(state, flag);
			}
		}

		/// <summary>
		/// Returns true if this LongFlags has the provided flag, false if not, requires at least one flag
		/// </summary>
		/// <param name="flag">Enum Value of type provided in this LongFlags's constructor</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlag(Enum flag)
		{
			if (flags[GetFlagIndex(flag)]) return true;

			return false;
		}

		/// <summary>
		/// Returns true if this LongFlags has ALL of the provided flags, false if not, requires at least one flag
		/// </summary>
		/// <param name="flags">Enum Values of type provided in this LongFlags's constructor</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlagsAND(params Enum[] flags)
		{
			if (flags == null) throw new ArgumentException("No arguments were passed");

			foreach (Enum _flag in flags)
			{
				if (!HasFlag(_flag)) return false;
			}

			return true;
		}

		/// <summary>
		/// Returns true if this LongFlags has ANY of the provided flags, false if not, requires at least one flag
		/// </summary>
		/// <param name="flags">Enum Values of type provided in this LongFlags's constructor</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlagsOR(params Enum[] flags)
		{
			if (flags == null) throw new ArgumentException("No arguments were passed");

			foreach (Enum _flag in flags)
			{
				if (HasFlag(_flag)) return true;
			}

			return false;
		}

		public virtual List<Enum> GetAllTrueFlags()
		{
			return GetAllFlagsOfIndex(GetAllTrueIndexes().ToArray());
		}

		#endregion

		#region Internal

		/// <summary>
		/// Returns a flag's index in the internal BitArray
		/// </summary>
		/// <param name="flag">Enum Value of type provided in this LongFlags's constructor</param>
		/// <returns>Index of provided flag in BitArray</returns>
		protected virtual int GetFlagIndex(Enum flag)
		{
			if (!enums.ContainsKey(flag.GetType())) throw new ArgumentException(string.Format("Enum Type {0} was not defined in {1}'s constructor", flag.GetType(), this));

			return enums[flag.GetType()] + (Convert.ToInt32(flag));
		}

		/// <summary>
		/// Returns a flag based on the flag's index in the internal BitArray
		/// </summary>
		/// <param name="index">Index of the flag to retrieve</param>
		/// <returns>Retrieved flag</returns>
		protected virtual Enum GetFlagFromIndex(int index)
		{
			Type type = GetTypeFromIndex(index);
			int value = (index - enums[type]);

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

			foreach (KeyValuePair<Type, int> entry in enums)
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

			for (int i = 0; i < flags.Count; i++)
			{
				if (flags[i] == true)
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
	}
}