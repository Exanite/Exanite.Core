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

						trackedStatsBonus -= 1;
					}
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

		#endregion

		#region Constructors

		public TrackedStat(StatSystem statSystem = null, TrackedStat[] trackedStats = null, Enum[] flags = null, string name = "Unnamed Stat")
		{
			if (trackedStats == null && flags == null)
			{
				throw new ArgumentNullException($"{nameof(trackedStats)} and {nameof(flags)} cannot be both null");
			}

			if (flags != null)
			{
				if (statSystem != null)
				{
					UseStatSystem(statSystem, flags);
				}
				else
				{
					throw new ArgumentException($"There must be a {nameof(statSystem)} if {nameof(flags)} is not null");
				}
			}
			if (trackedStats != null)
			{
				if (flags == null && trackedStats.Length < 2)
				{
					throw new ArgumentException($"There must be more than 2 {nameof(trackedStats)} if {nameof(flags)} is null");
				}

				UseTrackedStats(trackedStats);
			}

			SetName(name);
		}

		#region Destructor

		/// <summary>
		/// Unsubscibes to the statSystem's events when destroyed
		/// </summary>
		~TrackedStat()
		{
			if(statSystem != null)
			{
				statSystem.ModAdded -= ModAdded;
				statSystem.ModRemoved -= ModRemoved;
			}
		}

		#endregion

		#region Internal

		/// <summary>
		/// Internal method used to initialize the TrackedStat
		/// </summary>
		/// <param name="statSystem">StatSystem this TrackedStat is listening to</param>
		/// <param name="flags">Flags of this TrackedStat</param>
		/// <param name="matchType">How flags are matched</param>
		protected virtual void UseStatSystem(StatSystem statSystem, Enum[] flags)
		{
			this.statSystem = statSystem;

			statSystem.ModAdded += ModAdded;
			statSystem.ModRemoved += ModRemoved;

			this.flags = new LongFlag(flags);

			foreach (StatMod mod in statSystem.GetAllModifiers())
			{
				if (CheckModMatch(mod))
				{
					ModAdded(mod);
				}
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
			bool matchSuccess;
			bool hasBaseFlag = mod.Flags.HasFlag(StatModFlag.Base);

			if(hasBaseFlag)
			{
				mod.Flags.SetFlag(false, StatModFlag.Base);
			}

			matchSuccess = flags.HasFlags(FlagMatchType.And, mod.Flags);

			if (hasBaseFlag)
			{
				mod.Flags.SetFlag(true, StatModFlag.Base);
			}

			return matchSuccess;
		}

		#endregion

		#region Other

		/// <summary>
		/// Sets the name of the TrackedStat
		/// </summary>
		/// <param name="name"></param>
		public virtual void SetName(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Adds another TrackedStat to track
		/// </summary>
		/// <param name="trackedStat">TrackedStat to track</param>
		public virtual void AddTrackedStat(TrackedStat trackedStat)
		{
			trackedStats.Add(trackedStat);
		}

		#endregion
	}
}