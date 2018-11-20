using System;
using System.Collections;
using System.Linq;

namespace Exanite.Stats.Internal
{
	public class LongFlags<T> where T : struct, IConvertible
	{
		private BitArray flags;

		public LongFlags()
		{
			if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

			flags = new BitArray(Enum.GetValues(typeof(T)).Cast<int>().Max() + 1);
		}

		public void SetFlag(T flag, bool state)
		{
			flags[GetFlagIndex(flag)] = state;
		}

		public bool HasFlag(T flag)
		{
			if (flags[GetFlagIndex(flag)]) return true;

			return false;
		}

		public bool HasFlags(params T[] flags)
		{
			foreach (T flag in flags)
			{
				if (!HasFlag(flag)) return false;
			}

			return true;
		}

		private int GetFlagIndex(T flag)
		{
			return (int)(object)flag;
		}
	}
}
