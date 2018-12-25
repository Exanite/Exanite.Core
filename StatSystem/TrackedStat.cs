using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using Sirenix.Serialization;
using Exanite.Flags;
using Exanite.Extensions;

namespace Exanite.StatSystem
{
	/// <summary>
	/// Tracks modifiers and other tracked stats to produce a FinalValue
	/// </summary>
	[Serializable]
	public class TrackedStat<T> where T : struct, IComparable, IConvertible, IFormattable
	{
		#region Fields and Properties

		protected string name;
		[HideInInspector] [OdinSerialize] protected float flatValue = 0f;
		[HideInInspector] [OdinSerialize] protected float incValue = 1f;
		[HideInInspector] [OdinSerialize] protected float multValue = 1f;
		[HideInInspector] [OdinSerialize] protected LongFlag<T> flags;

		[HideInInspector] [OdinSerialize] protected StatSystem<T> statSystem;
		[HideInInspector] [OdinSerialize] protected List<TrackedStat<T>> trackedStats;

		protected bool? isEnum = null;

		/// <summary>
		/// Name of the stat
		/// </summary>
#if ODIN_INSPECTOR
		[ShowInInspector]
#endif
		public string Name
		{
			get
			{
				if (string.IsNullOrEmpty(name))
				{
					foreach (T flag in flags.GetAllTrueFlags())
					{
						name += $"{flag} ";
						name.Trim();
					}
				}
				return name;
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
					foreach (TrackedStat<T> stat in trackedStats)
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
					foreach (TrackedStat<T> stat in trackedStats)
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
					foreach (TrackedStat<T> stat in trackedStats)
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
#if ODIN_INSPECTOR
		[ShowInInspector]
#endif
		public float FinalValue
		{
			get
			{
				return FlatValue * IncValue * MultValue;
			}
		}

		#endregion

		#region Constructors

		public TrackedStat(StatSystem<T> statSystem, TrackedStat<T>[] trackedStats = null, T[] flags = null)
		{
			if (trackedStats.IsNullOrEmpty() && flags.IsNullOrEmpty())
			{
				throw new ArgumentNullException($"{nameof(trackedStats)} and {nameof(flags)} cannot be both null");
			}

			switch (isEnum)
			{
				case (null):
					isEnum = typeof(T).IsEnum;
					if (isEnum == false)
					{
						throw new ArgumentException(string.Format("{0} is not an Enum Type", typeof(T)));
					}
					break;
				case (true):
					break;
				case (false):
					throw new ArgumentException(string.Format("{0} is not an Enum Type", typeof(T)));
			}

			if (!flags.IsNullOrEmpty())
			{
				UseStatSystem(statSystem, flags);
			}
			if (!trackedStats.IsNullOrEmpty())
			{
				if (flags.IsNullOrEmpty() && trackedStats.Length < 2)
				{
					throw new ArgumentException($"There must be more than 2 {nameof(trackedStats)} if {nameof(flags)} is null");
				}

				UseTrackedStats(trackedStats);
			}
		}

		public TrackedStat(StatSystem<T> statSystem, TrackedStat<T>[] trackedStats = null, BitArray bitArray = null)
		{
			if (trackedStats.IsNullOrEmpty() && bitArray == null)
			{
				throw new ArgumentNullException($"{nameof(trackedStats)} and {nameof(flags)} cannot be both null");
			}

			switch (isEnum)
			{
				case (null):
					isEnum = typeof(T).IsEnum;
					if (isEnum == false)
					{
						throw new ArgumentException(string.Format("{0} is not an Enum Type", typeof(T)));
					}
					break;
				case (true):
					break;
				case (false):
					throw new ArgumentException(string.Format("{0} is not an Enum Type", typeof(T)));
			}

			if (bitArray != null)
			{
				UseStatSystem(statSystem, bitArray);
			}
			if (!trackedStats.IsNullOrEmpty())
			{
				if (bitArray == null && trackedStats.Length < 2)
				{
					throw new ArgumentException($"There must be more than 2 {nameof(trackedStats)} if {nameof(bitArray)} is null");
				}

				UseTrackedStats(trackedStats);
			}
		}

		#region Destructor

		/// <summary>
		/// Unsubscibes to the statSystem's events when destroyed
		/// </summary>
		~TrackedStat()
		{
			UnsubscribeFromStatSystem();
		}

		#endregion

		#region Internal

		/// <summary>
		/// Internal method used to initialize the TrackedStat
		/// </summary>
		/// <param name="statSystem">StatSystem this TrackedStat is listening to</param>
		/// <param name="flags">Flags of this TrackedStat</param>
		/// <param name="matchType">How flags are matched</param>
		protected virtual void UseStatSystem(StatSystem<T> statSystem, T[] flags)
		{
			this.statSystem = statSystem;

			statSystem.ModAdded += ModAdded;
			statSystem.ModRemoved += ModRemoved;

			this.flags = new LongFlag<T>(flags);

			foreach (StatMod<T> mod in statSystem.GetAllModifiers())
			{
				if (CheckModMatch(mod))
				{
					ModAdded(mod);
				}
			}
		}

		/// <summary>
		/// Internal method used to initialize the TrackedStat
		/// </summary>
		/// <param name="statSystem">StatSystem this TrackedStat is listening to</param>
		/// <param name="flags">Flags of this TrackedStat</param>
		/// <param name="matchType">How flags are matched</param>
		protected virtual void UseStatSystem(StatSystem<T> statSystem, BitArray bitArray)
		{
			this.statSystem = statSystem;

			statSystem.ModAdded += ModAdded;
			statSystem.ModRemoved += ModRemoved;

			flags = new LongFlag<T>(bitArray);

			foreach (StatMod<T> mod in statSystem.GetAllModifiers())
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
		protected virtual void UseTrackedStats(params TrackedStat<T>[] trackedStats)
		{
			this.trackedStats = new List<TrackedStat<T>>(trackedStats);
		}

		#endregion

		#endregion

		#region StatSystem Listening

		/// <summary>
		/// Unsubscribes from the StatSystem <para/>
		/// Do not use unless you know what you are doing
		/// </summary>
		public virtual void UnsubscribeFromStatSystem()
		{
			if (statSystem != null)
			{
				statSystem.ModAdded -= ModAdded;
				statSystem.ModRemoved -= ModRemoved;
			}
		}

		/// <summary>
		/// Method that listens to new modifiers being added to the StatSystem
		/// </summary>
		/// <param name="mod">Added mod</param>
		protected virtual void ModAdded(StatMod<T> mod)
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
		protected virtual void ModRemoved(StatMod<T> mod)
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
		/// <returns>True or false</returns>
		protected virtual bool CheckModMatch(StatMod<T> mod)
		{
			return mod.IsMatch(flags);
		}

		#endregion

		#region Other

		/// <summary>
		/// Adds another TrackedStat to track
		/// </summary>
		/// <param name="trackedStat">TrackedStat to track</param>
		public virtual void AddTrackedStat(TrackedStat<T> trackedStat)
		{
			trackedStats.Add(trackedStat);
		}

		#endregion
	}
}