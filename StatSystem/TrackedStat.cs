using Exanite.StatSystem.Internal;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Serialization;

namespace Exanite.StatSystem
{
	/// <summary>
	/// Tracks modifiers and other tracked stats to produce a FinalValue
	/// </summary>
	[Serializable]
	public class TrackedStat
	{
		#region Fields and Properties

		[HideInInspector] [OdinSerialize] protected string name = "Unnamed Stat";
		[HideInInspector] [OdinSerialize] protected float flatValue = 0f;
		[HideInInspector] [OdinSerialize] protected float incValue = 1f;
		[HideInInspector] [OdinSerialize] protected float multValue = 1f;
		[HideInInspector] [OdinSerialize] protected LongFlag flags;
		[HideInInspector] [OdinSerialize] protected FlagMatchType matchType;

		[HideInInspector] [OdinSerialize] protected StatSystem statSystem;
		[HideInInspector] [OdinSerialize] protected List<TrackedStat> trackedStats;

		/// <summary>
		/// Name of the stat
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}

			protected set
			{
				name = value;
			}
		}
		/// <summary>
		/// Flat value of the stat
		/// </summary>
		public float FlatValue
		{
			get
			{
				float trackedStatsBonus = 0;
				if(trackedStats != null)
				{
					foreach (TrackedStat stat in trackedStats)
					{
						trackedStatsBonus += stat.FlatValue;
					}
				}
				return flatValue + trackedStatsBonus;
			}
		}
		/// <summary>
		/// Inc value of the stat
		/// </summary>
		public float IncValue
		{
			get
			{
				float trackedStatsBonus = 0;
				if (trackedStats != null)
				{
					foreach (TrackedStat stat in trackedStats)
					{
						trackedStatsBonus += stat.IncValue;
					}

					trackedStatsBonus -= 1;
				}
				return incValue + trackedStatsBonus;
			}
		}
		/// <summary>
		/// Mult value of the stat
		/// </summary>
		public float MultValue
		{
			get
			{
				float trackedStatsBonus = 1;
				if (trackedStats != null)
				{
					foreach (TrackedStat stat in trackedStats)
					{
						trackedStatsBonus *= stat.MultValue;
					}
				}
				return multValue * trackedStatsBonus;
			}
		}
		/// <summary>
		/// Final value of the stat (Flat * Inc * Mult)
		/// </summary>
		[ShowInInspector]
		public float FinalValue
		{
			get
			{
				return FlatValue * IncValue * MultValue;
			}
		}
		/// <summary>
		/// The LongFlags of this TrackedStat
		/// </summary>
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
		/// <summary>
		/// Stats tracked by this TrackedStat
		/// </summary>
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

		#region Constructors

		/// <summary>
		/// Creates a new TrackedStat that listens to new modifiers in the StatSystem
		/// </summary>
		/// <param name="statSystem">StatSystem this TrackedStat is listening to</param>
		/// <param name="flags">Flags of this TrackedStat</param>
		/// <param name="matchType">How flags are matched</param>
		public TrackedStat(StatSystem statSystem, Enum[] flags, FlagMatchType matchType = FlagMatchType.Equals)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			UseStatSystem(statSystem, flags, matchType);
		}

		/// <summary>
		/// Creates a new TrackedStat that adds two+ other TrackedStats together and listens to new modifiers in the StatSystem
		/// </summary>
		/// <param name="statSystem">StatSystem this TrackedStat is listening to</param>
		/// <param name="trackedStats">Other TrackedStats to track</param>
		/// <param name="flags">Flags of this TrackedStat</param>
		/// <param name="matchType">How flags are matched</param>
		public TrackedStat(StatSystem statSystem, TrackedStat[] trackedStats, Enum[] flags, FlagMatchType matchType = FlagMatchType.Equals)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			UseStatSystem(statSystem, flags, matchType);
			UseTrackedStats(trackedStats);
		}

		/// <summary>
		/// Creates a new TrackedStat that adds two+ other TrackedStats together and listens to new modifiers in the StatSystem
		/// </summary>
		/// <param name="statSystem">StatSystem this TrackedStat is listening to</param>
		/// <param name="trackedStat">Other TrackedStat to track</param>
		/// <param name="flags">Flags of this TrackedStat</param>
		/// <param name="matchType">How flags are matched</param>
		public TrackedStat(StatSystem statSystem, TrackedStat trackedStat, Enum[] flags, FlagMatchType matchType = FlagMatchType.Equals)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			UseStatSystem(statSystem, flags, matchType);
			UseTrackedStats(trackedStat);
		}

		/// <summary>
		/// Creates a new TrackedStat that adds two+ other TrackedStats together
		/// </summary>
		/// <param name="trackedStats">Other TrackedStats to track</param>
		public TrackedStat(params TrackedStat[] trackedStats)
		{
			if (trackedStats == null)
			{
				throw new ArgumentNullException(nameof(trackedStats));
			}

			if (trackedStats.Length < 2)
			{
				throw new ArgumentException("There must be more than 2 TrackedStats");
			}

			UseTrackedStats(trackedStats);
		}

		#region Internal

		/// <summary>
		/// Internal method used to initialize the TrackedStat
		/// </summary>
		/// <param name="statSystem">StatSystem this TrackedStat is listening to</param>
		/// <param name="flags">Flags of this TrackedStat</param>
		/// <param name="matchType">How flags are matched</param>
		protected virtual void UseStatSystem(StatSystem statSystem, Enum[] flags, FlagMatchType matchType)
		{
			this.statSystem = statSystem;

			statSystem.ModAdded += ModAdded;
			statSystem.ModRemoved += ModRemoved;

			this.flags = new LongFlag(flags);
			this.matchType = matchType;

			foreach (StatMod mod in statSystem.GetAllModsWithFlags(matchType, flags))
			{
				ModAdded(mod);
			}
		}

		/// <summary>
		/// Internal method used to add new TrackedStats
		/// </summary>
		/// <param name="trackedStats">Other TrackedStats to track</param>
		protected virtual void UseTrackedStats(params TrackedStat[] trackedStats)
		{
			this.trackedStats = new List<TrackedStat>(trackedStats);
		}

		#endregion

		#endregion

		#region StatSystem Listening

		/// <summary>
		/// Method that listens to new modifiers being added to the StatSystem
		/// </summary>
		/// <param name="mod">Added mod</param>
		protected virtual void ModAdded(StatMod mod)
		{
			if (!CheckModMatch(mod)) return;

			switch(mod.Type)
			{
				case (StatModType.Flat):
					flatValue += mod.Value;
					break;
				case (StatModType.Inc):
					incValue += mod.Value;
					break;
				case (StatModType.Mult):
					multValue *= mod.Value;
					break;
			}
		}

		/// <summary>
		/// Method that listens to modifiers being removed to the StatSystem
		/// </summary>
		/// <param name="mod">Removed mod</param>
		protected virtual void ModRemoved(StatMod mod)
		{
			if (!CheckModMatch(mod)) return;

			switch (mod.Type)
			{
				case (StatModType.Flat):
					flatValue -= mod.Value;
					break;
				case (StatModType.Inc):
					incValue -= mod.Value;
					break;
				case (StatModType.Mult):
					multValue /= mod.Value;
					break;
			}
		}

		/// <summary>
		/// Checks if the mod being added/removed matches the flags of this TrackedStat
		/// </summary>
		/// <param name="mod">Modifier to check</param>
		/// <returns></returns>
		protected virtual bool CheckModMatch(StatMod mod)
		{
			return mod.Flags.HasFlags(matchType, Flags);
		}

		#endregion

		#region Other

		public virtual void SetName(string name)
		{
			Name = name;
		}

		#endregion
	}
}