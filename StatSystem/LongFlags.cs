using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Exanite.StatSystem.Internal
{
	public class LongFlags
	{
		protected BitArray flags;
		protected Dictionary<Type, int> enums;

		public LongFlags(params Type[] flaggableEnums) // can pass any type right now, but preferably only types that are enums
		{
			int bitsToAdd = 0;

			enums = new Dictionary<Type, int>();

			for (int i = 0; i < flaggableEnums.Count(); i++)
			{
				if (!flaggableEnums[i].IsEnum) throw new ArgumentException(string.Format("Passed parameter {0} is not an Enum", flaggableEnums[i]));

				if (!enums.ContainsKey(flaggableEnums[i]))
				{
					int enumMax = Enum.GetValues(flaggableEnums[i]).Cast<int>().Max();
					int enumMin = Enum.GetValues(flaggableEnums[i]).Cast<int>().Min();

					if (enumMin < 0)
						throw new ArgumentException(string.Format("{0} must not have any negative values", flaggableEnums[i]));

					enums.Add(flaggableEnums[i], bitsToAdd);
					bitsToAdd += enumMax + 1;
				}
				else
				{
					throw new ArgumentException(string.Format("{0} is passed as a parameter more than once", flaggableEnums[i]));
				}
			}

			flags = new BitArray(bitsToAdd);
		}

		public virtual void SetFlag(bool state, Enum flagToSet)
		{
			flags[GetFlagIndex(flagToSet)] = state;
		}

		public virtual void SetFlags(bool state, params Enum[] flagsToSet)
		{
			foreach (Enum flag in flagsToSet)
			{
				flags[GetFlagIndex(flag)] = state;
			}
		}

		public virtual bool HasFlag(Enum flag)
		{
			if (flags[GetFlagIndex(flag)]) return true;

			return false;
		}

		public virtual bool HasFlags(params Enum[] flags)
		{
			foreach (Enum flag in flags)
			{
				if (!HasFlag(flag)) return false;
			}

			return true;
		}

		public virtual int GetFlagIndex(Enum flag)
		{
			if (!enums.ContainsKey(flag.GetType())) throw new ArgumentException(string.Format("Enum Type {0} was not defined in {1}'s constructor", flag.GetType(), this));

			return enums[flag.GetType()] + (Convert.ToInt32(flag));
		}
	}
}
