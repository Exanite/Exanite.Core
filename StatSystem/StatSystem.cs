using System;
using System.Collections.Generic;
using Exanite.StatSystem.Internal;

namespace Exanite.StatSystem
{
	/// <summary>
	/// Handles everything you need for a stat system in a game
	/// </summary>
	public class StatSystem
	{
		#region Fields, Properties, and Events

		protected List<StatMod> modifiers;
		protected Dictionary<string, TrackedStat> trackedStats;

		/// <summary>
		/// List of all the modifiers
		/// </summary>
		public List<StatMod> Modifiers
		{
			get
			{
				return modifiers;
			}

			protected set
			{
				modifiers = value;
			}
		}
		/// <summary>
		/// Dictionary with all of the Tracked Stats
		/// </summary>
		public Dictionary<string, TrackedStat> TrackedStats
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

		/// <summary>
		/// Delegate used for mod events
		/// </summary>
		/// <param name="mod"></param>
		public delegate void ModEvent(StatMod mod);
		/// <summary>
		/// Called when a mod is added
		/// </summary>
		public ModEvent ModAdded;
		/// <summary>
		/// Called when a mod is removed
		/// </summary>
		public ModEvent ModRemoved;

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a new StatSystem
		/// </summary>
		public StatSystem()
		{
			Modifiers = new List<StatMod>();
			TrackedStats = new Dictionary<string, TrackedStat>();
		}

		#endregion

		#region Retrieving Mods

		#region And

		/// <summary>
		/// Returns all modifiers with all of the flags in the provided LongFlag
		/// </summary>
		/// <param name="longFlag">LongFlag to compare</param>
		/// <returns>List of all modifiers with all of the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsAnd(LongFlag longFlag)
		{
			return GetAllModsWithFlagsAnd(longFlag.GetAllTrueFlags().ToArray());
		}

		/// <summary>
		/// Returns all modifiers with all of the flags provided
		/// </summary>
		/// <param name="flags">Flags to compare</param>
		/// <returns>List of all modifiers with all of the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsAnd(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			List<StatMod> mods = new List<StatMod>();

			foreach (StatMod mod in Modifiers)
			{
				if (mod.Flags.HasFlagsAnd(flags))
				{
					mods.Add(mod);
				}
			}

			return mods;
		}

		#endregion

		#region Or

		/// <summary>
		/// Returns all modifiers with any of the flags in the provided LongFlag
		/// </summary>
		/// <param name="longFlag">LongFlag to compare</param>
		/// <returns>List of all modifiers with any of the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsOr(LongFlag longFlag)
		{
			return GetAllModsWithFlagsOr(longFlag.GetAllTrueFlags().ToArray());
		}

		/// <summary>
		/// Returns all modifiers with all of the flags provided
		/// </summary>
		/// <param name="flags">Flags to compare</param>
		/// <returns>List of all modifiers with any of the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsOr(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			List<StatMod> mods = new List<StatMod>();

			foreach (StatMod mod in Modifiers)
			{
				if (mod.Flags.HasFlagsOr(flags))
				{
					mods.Add(mod);
				}
			}

			return mods;
		}

		#endregion

		#region Equals

		/// <summary>
		/// Returns all modifiers with only the flags in the provided LongFlag
		/// </summary>
		/// <param name="longFlag">LongFlag to compare</param>
		/// <returns>List of all modifiers with only of the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsEquals(LongFlag longFlag)
		{
			return GetAllModsWithFlagsEquals(longFlag.GetAllTrueFlags().ToArray());
		}

		/// <summary>
		/// Returns all modifiers with only the flags provided
		/// </summary>
		/// <param name="flags">Flags to compare</param>
		/// <returns>List of all modifiers with only the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsEquals(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			List<StatMod> mods = new List<StatMod>();

			foreach(StatMod mod in Modifiers)
			{
				if (mod.Flags.HasFlagsEquals(flags))
				{
					mods.Add(mod);
				}
			}

			return mods;
		}

		#endregion

		#region Other

		/// <summary>
		/// Returns all modifiers that match the flags in the provided LongFlag with the provided match type
		/// </summary>
		/// <param name="matchType">How to match the flags</param>
		/// <param name="longFlag">LongFlag to compare</param>
		/// <returns>List of matched modifiers</returns>
		public virtual List<StatMod> GetAllModsWithFlags(FlagMatchType matchType, LongFlag longFlag)
		{
			return GetAllModsWithFlags(matchType, longFlag.GetAllTrueFlags().ToArray());
		}

		/// <summary>
		/// Returns all modifiers that match the flags provided with the provided match type
		/// </summary>
		/// <param name="matchType">How to match the flags</param>
		/// <param name="flags">Flags to compare</param>
		/// <returns>List of matched modifiers</returns>
		public virtual List<StatMod> GetAllModsWithFlags(FlagMatchType matchType, params Enum[] flags)
		{
			switch (matchType)
			{
				case (FlagMatchType.And):
					return GetAllModsWithFlagsAnd(flags);
				case (FlagMatchType.Or):
					return GetAllModsWithFlagsOr(flags);
				case (FlagMatchType.Equals):
					return GetAllModsWithFlagsEquals(flags);
				default:
					throw new ArgumentOutOfRangeException($"{matchType} does not have a code path");
			}
		}

		#endregion

		#endregion

		#region Adding/Removing Mods

		#region Adding Mods

		/// <summary>
		/// Adds the provided modifier to the StatSystem
		/// </summary>
		/// <param name="mod">Mod to add</param>
		public virtual void AddModifier(StatMod mod)
		{
			modifiers.Add(mod);
			ModAdded?.Invoke(mod);
		}

		/// <summary>
		/// Adds the provided modifiers to the StatSystem
		/// </summary>
		/// <param name="mods">Mods to add</param>
		public virtual void AddModifiers(params StatMod[] mods)
		{
			if (mods == null)
			{
				throw new ArgumentNullException(nameof(mods));
			}

			foreach(StatMod mod in mods)
			{
				AddModifier(mod);
			}
		}

		/// <summary>
		/// Creates a new mod, adds the mod to the StatSystem, and then returns the created mod
		/// </summary>
		/// <param name="value">Value of the mod</param>
		/// <param name="type">How the modifier is applied to existing stats</param>
		/// <param name="source">Where the mod came from, usually "this"</param>
		/// <param name="flags">What flags the modifier has</param>
		/// <returns>The created StatMod</returns>
		public virtual StatMod AddModifier(float value, StatModType type, object source, params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			StatMod mod = new StatMod(value, type, source, flags);

			AddModifier(mod);

			return mod;
		}

		#endregion

		#region Removing Mods

		/// <summary>
		/// Removes the provided mod from the StatSystem
		/// </summary>
		/// <param name="mod">Mod to remove</param>
		/// <returns>Did the mod get removed</returns>
		public virtual bool RemoveModifier(StatMod mod)
		{
			if(modifiers.Remove(mod))
			{
				ModRemoved?.Invoke(mod);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Removes the provided mods from the StatSystem
		/// </summary>
		/// <param name="mods">Mods to remove</param>
		/// <returns>Did all the mods get removed</returns>
		public virtual bool RemoveModifiers(params StatMod[] mods)
		{
			if (mods == null)
			{
				throw new ArgumentNullException(nameof(mods));
			}

			bool removedAll = true;

			foreach(StatMod mod in mods)
			{
				if (!RemoveModifier(mod)) removedAll = false;
			}

			return removedAll;
		}

		/// <summary>
		/// Removes all mods from a source
		/// </summary>
		/// <param name="source">Where the mod came from, usually "this"</param>
		/// <returns>Did a mod get removed</returns>
		public virtual bool RemoveAllModifiersFromSource(object source)
		{
			bool didRemove = false;

			for (int i = modifiers.Count - 1; i >= 0; i--)
			{
				if (modifiers[i].Source == source)
				{
					didRemove = true;
					RemoveModifier(modifiers[i]);
				}
			}

			return didRemove;
		}

		/// <summary>
		/// Removes all modifiers without flag "Base", has an optional parameter to choose the exclusion flag instead
		/// </summary>
		/// <param name="flag">Modifiers with this flag will be kept</param>
		public virtual void RemoveAllNonBaseModifiers(Enum flag = null)
		{
			if (flag == null)
			{
				flag = StatModFlag.Base;
			}

			for (int i = modifiers.Count - 1; i >= 0; i--)
			{
				if (!modifiers[i].Flags.HasFlag(flag))
				{
					RemoveModifier(modifiers[i]);
				}
			}
		}

		/// <summary>
		/// Removes all modifiers from the StatSystem
		/// </summary>
		public virtual void RemoveAllModifiers()
		{
			for (int i = modifiers.Count - 1; i >= 0; i--)
			{
				RemoveModifier(modifiers[i]);
			}
		}

		#endregion

		#endregion

		#region Tracked Stats

		#region Adding

		/// <summary>
		/// Creates a new TrackedStat that listens to new modifiers in the StatSystem
		/// </summary>
		/// <param name="name">Name of this TrackedStat</param>
		/// <param name="flags">Flags of this TrackedStat</param>
		/// <param name="matchType">How flags are matched</param>
		public virtual void AddTrackedStat(string name, Enum[] flags, FlagMatchType matchType = FlagMatchType.Equals)
		{
			if(!trackedStats.ContainsKey(name))
			{
				throw new ArgumentException($"TrackedStat of {name} already exists in the StatSystem");
			}
			else
			{
				trackedStats.Add(name, new TrackedStat(this, flags, matchType));
			}
		}

		/// <summary>
		/// Creates a new TrackedStat in the StatSystem that adds two+ other TrackedStats together and listens to new modifiers in the StatSystem
		/// </summary>
		/// <param name="name">Name of the TrackedStat</param>
		/// <param name="trackedStats">Other TrackedStats to track</param>
		/// <param name="flags">Flags of this TrackedStat</param>
		/// <param name="matchType">How flags are matched</param>
		public virtual void AddTrackedStat(string name, TrackedStat[] trackedStats, Enum[] flags, FlagMatchType matchType = FlagMatchType.Equals)
		{
			if (!this.trackedStats.ContainsKey(name))
			{
				throw new ArgumentException($"TrackedStat of {name} already exists in the StatSystem");
			}
			else
			{
				this.trackedStats.Add(name, new TrackedStat(this, trackedStats, flags, matchType));
			}
		}

		/// <summary>
		/// Creates a new TrackedStat in the StatSystem that adds two+ other TrackedStats together and listens to new modifiers in the StatSystem
		/// </summary>
		/// <param name="name">Name of the TrackedStat</param>
		/// <param name="trackedStat">Other TrackedStat to track</param>
		/// <param name="flags">Flags of this TrackedStat</param>
		/// <param name="matchType">How flags are matched</param>
		public virtual void AddTrackedStat(string name, TrackedStat trackedStat, Enum[] flags, FlagMatchType matchType = FlagMatchType.Equals)
		{
			if (!trackedStats.ContainsKey(name))
			{
				throw new ArgumentException($"TrackedStat of {name} already exists in the StatSystem");
			}
			else
			{
				trackedStats.Add(name, new TrackedStat(this, trackedStat, flags, matchType));
			}
		}

		/// <summary>
		/// Creates a new TrackedStat in the statSystem that adds two+ other TrackedStats together
		/// </summary>
		/// <param name="name">Name of the TrackedStat</param>
		/// <param name="trackedStats">Other TrackedStats to track</param>
		public virtual void AddTrackedStat(string name, params TrackedStat[] trackedStats)
		{
			if (!this.trackedStats.ContainsKey(name))
			{
				throw new ArgumentException($"TrackedStat of {name} already exists in the StatSystem");
			}
			else
			{
				this.trackedStats.Add(name, new TrackedStat(trackedStats));
			}
		}

		#endregion

		#region Retrieving

		/// <summary>
		/// Retrieves a TrackedStat by name from the StatSystem
		/// </summary>
		/// <param name="name">Name of the TrackedStat</param>
		/// <returns>Retrieved TrackedStat</returns>
		public virtual TrackedStat GetTrackedStat(string name)
		{
			if(trackedStats.ContainsKey(name))
			{
				return trackedStats[name];
			}
			else
			{
				throw new ArgumentException($"TrackedStat of {name} does not exist in the StatSystem");
			}
		}

		#endregion

		#endregion
	}
}