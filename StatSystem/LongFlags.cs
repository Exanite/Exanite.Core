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
		protected BitArray flags;
		protected Dictionary<Type, int> enums;

		/// <summary>
		/// Creates a LongFlags with the provided Enum Types
		/// </summary>
		/// <param name="flaggableEnum">Enum Type with no negative values (use typeof())</param>
		/// <param name="flaggableEnums">Enum Types with no negative values (use typeof())</param>
		public LongFlags(Type flaggableEnum, params Type[] flaggableEnums) // can pass any type right now, but preferably only types that are enums
		{
			List<Type> _flaggableEnums = new List<Type>() { flaggableEnum };
			_flaggableEnums.AddRange(flaggableEnums);

			int bitsToAdd = 0;

			enums = new Dictionary<Type, int>();

			foreach (Type enumToAdd in _flaggableEnums)
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
				else
				{
					throw new ArgumentException(string.Format("{0} is passed as a parameter more than once", enumToAdd));
				}
			}

			flags = new BitArray(bitsToAdd);
		}

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
		/// Sets a a number of flags to a provided state
		/// </summary>
		/// <param name="state">True or false</param>
		/// <param name="flagToSet">Enum Value of type provided in this LongFlags's constructor</param>
		/// <param name="flagsToSet">Enum Values of type provided in this LongFlags's constructor</param>
		public virtual void SetFlags(bool state, Enum flagToSet, params Enum[] flagsToSet)
		{
			List<Enum> _flagsToSet = new List<Enum>() { flagToSet };
			_flagsToSet.AddRange(flagsToSet);

			foreach (Enum flag in _flagsToSet)
			{
				SetFlag(state, flag);
			}
		}

		/// <summary>
		/// Returns true if this LongFlags has the provided flag, false if not
		/// </summary>
		/// <param name="flag">Enum Value of type provided in this LongFlags's constructor</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlag(Enum flag)
		{
			if (flags[GetFlagIndex(flag)]) return true;

			return false;
		}

		/// <summary>
		/// Returns true if this LongFlags has ALL of the provided flags, false if not
		/// </summary>
		/// <param name="flag">Enum Value of type provided in this LongFlags's constructor</param>
		/// <param name="flags">Enum Values of type provided in this LongFlags's constructor</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlagsAND(Enum flag, params Enum[] flags)
		{
			List<Enum> _flags = new List<Enum> { flag };
			_flags.AddRange(flags);

			foreach (Enum _flag in _flags)
			{
				if (!HasFlag(_flag)) return false;
			}

			return true;
		}

		/// <summary>
		/// Returns true if this LongFlags has ANY of the provided flags, false if not
		/// </summary>
		/// <param name="flag">Enum Value of type provided in this LongFlags's constructor</param>
		/// <param name="flags">Enum Values of type provided in this LongFlags's constructor</param>
		/// <returns>True or false</returns>
		public virtual bool HasFlagsOR(Enum flag, params Enum[] flags)
		{
			List<Enum> _flags = new List<Enum>();
			_flags.Add(flag);
			_flags.AddRange(flags);

			foreach (Enum _flag in _flags)
			{
				if (HasFlag(_flag)) return true;
			}

			return false;
		}

		/// <summary>
		/// Returns a flag's index in the internal BitArray
		/// </summary>
		/// <param name="flag">Enum Value of type provided in this LongFlags's constructor</param>
		/// <returns>Index of provided flag in BitArray</returns>
		public virtual int GetFlagIndex(Enum flag)
		{
			if (!enums.ContainsKey(flag.GetType())) throw new ArgumentException(string.Format("Enum Type {0} was not defined in {1}'s constructor", flag.GetType(), this));

			return enums[flag.GetType()] + (Convert.ToInt32(flag));
		}
	}
}