using Exanite.StatSystem.Internal;
using System;
using System.Collections.Generic;

namespace Exanite.StatSystem
{
	public class TrackedStat
	{
		#region Fields and Properties

		protected float flatValue;
		protected float incValue;
		protected float multValue;
		protected float finalValue;
		protected LongFlag flags;
		protected List<TrackedStat> trackedStats;

		public float FlatValue
		{
			get
			{
				return flatValue;
			}

			protected set
			{
				flatValue = value;
			}
		}
		public float IncValue
		{
			get
			{
				return incValue;
			}

			protected set
			{
				incValue = value;
			}
		}
		public float MultValue
		{
			get
			{
				return multValue;
			}

			protected set
			{
				multValue = value;
			}
		}
		public float FinalValue
		{
			get
			{
				return finalValue;
			}

			protected set
			{
				finalValue = value;
			}
		}
		public LongFlag Flags
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
		public List<TrackedStat> TrackedStats
		{
			get
			{
				return trackedStats;
			}

			protected set
			{
				trackedStats = value;
			}
		}

		#endregion

		#region Constructor

		public TrackedStat(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}


		}

		public TrackedStat(TrackedStat trackedStat, params Enum[] flags)
		{

		}

		#endregion
	}
}