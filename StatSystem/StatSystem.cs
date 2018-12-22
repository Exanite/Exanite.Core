using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using Exanite.Flags;

namespace Exanite.StatSystem
{
	/// <summary>
	/// Handles everything you need for a stat system in a game
	/// </summary>
	[Serializable]
	public class StatSystem<T> where T : struct, IComparable, IConvertible, IFormattable
	{
		#region Fields and Events

		[OdinSerialize]
		#if ODIN_INSPECTOR
		[ReadOnly]
		[TabGroup("Modifiers")]
		#endif
		protected List<StatMod<T>> modifiers;

		[OdinSerialize]
		#if ODIN_INSPECTOR
		[ReadOnly]
		[TabGroup("Tracked Stats")]
		[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
		#endif
		protected Dictionary<string, TrackedStat<T>> trackedStats;

		protected static LongFlag<T> flagCache = new LongFlag<T>();

		/// <summary>
		/// Delegate used for mod events
		/// </summary>
		/// <param name="mod">Mod involved in the event</param>
		public delegate void ModEvent(StatMod<T> mod);
		/// <summary>
		/// Called when a mod is added
		/// </summary>
		[HideInInspector] public ModEvent ModAdded;
		/// <summary>
		/// Called when a mod is removed
		/// </summary>
		[HideInInspector] public ModEvent ModRemoved;

		protected bool? isEnum = null;

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a new StatSystem
		/// </summary>
		public StatSystem()
		{
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

			modifiers = new List<StatMod<T>>();
			trackedStats = new Dictionary<string, TrackedStat<T>>();
		}

		#endregion

		#region Retrieving Mods

		public virtual List<StatMod<T>> GetAllModifiers()
		{
			return modifiers;
		}

		#endregion

		#region Adding/Removing Mods

		#region Adding Mods

		/// <summary>
		/// Adds the provided modifier to the StatSystem
		/// </summary>
		/// <param name="mod">Mod to add</param>
		public virtual void AddModifier(StatMod<T> mod)
		{
			modifiers.Add(mod);
			ModAdded?.Invoke(mod);
		}

		/// <summary>
		/// Adds the provided modifiers to the StatSystem
		/// </summary>
		/// <param name="mods">Mods to add</param>
		public virtual void AddModifiers(params StatMod<T>[] mods)
		{
			if (mods == null)
			{
				throw new ArgumentNullException(nameof(mods));
			}

			foreach(StatMod<T> mod in mods)
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
		#if ODIN_INSPECTOR
		[Button(ButtonHeight = 25, Expanded = true)]
		[TabGroup("Utility")]
		[PropertyOrder(0)]
		#endif
		public virtual StatMod<T> AddModifier(float value, StatModType type, object source, params T[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			StatMod<T> mod = new StatMod<T>(value, type, source, flags);

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
		public virtual bool RemoveModifier(StatMod<T> mod)
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
		public virtual bool RemoveModifiers(params StatMod<T>[] mods)
		{
			if (mods == null)
			{
				throw new ArgumentNullException(nameof(mods));
			}

			bool removedAll = true;

			foreach(StatMod<T> mod in mods)
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
		#if ODIN_INSPECTOR
		[Button(ButtonHeight = 25, Expanded = true)]
		[TabGroup("Utility")]
		[PropertyOrder(1)]
		#endif
		public virtual bool RemoveAllModifiersBySource(object source)
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
		#if ODIN_INSPECTOR
		[Button(ButtonHeight = 25, Expanded = true)]
		[TabGroup("Utility")]
		[PropertyOrder(2)]
		#endif
		public virtual void RemoveAllNonBaseModifiers()
		{
			for (int i = modifiers.Count - 1; i >= 0; i--)
			{
				if (!modifiers[i].Flags.HasFlag(default(T)))
				{
					RemoveModifier(modifiers[i]);
				}
			}
		}

		/// <summary>
		/// Removes all modifiers from the StatSystem
		/// </summary>
		#if ODIN_INSPECTOR
		[Button(ButtonHeight = 25, Expanded = true)]
		[TabGroup("Utility")]
		[PropertyOrder(3)]
		#endif
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
		/// Creates and adds a new TrackedStat in the StatSystem
		/// <param name="trackedStats">TrackedStats to track</param>
		/// <param name="flags">Flags of this TrackedStat</param>
		#if ODIN_INSPECTOR
		[Button(ButtonHeight = 25, Expanded = true)]
		[TabGroup("Utility")]
		[PropertySpace(15)]
		[PropertyOrder(4)]
		#endif
		public virtual TrackedStat<T> AddTrackedStat(TrackedStat<T>[] trackedStats = null, T[] flags = null)
		{
			string name = PrepareFlagCache(trackedStats, flags);

			if (this.trackedStats.ContainsKey(name))
			{
				throw new ArgumentException($"TrackedStat of {name} already exists in the StatSystem");
			}
			else
			{
				this.trackedStats.Add(name, new TrackedStat<T>(this, trackedStats, flagCache.Flags));
				return this.trackedStats[name];
			}
		}

		#region Internal

		/// <summary>
		/// Used to prepare the FlagCache before doing actions related to TrackedStat
		/// </summary>
		/// <param name="trackedStats">TrackStats to use</param>
		/// <param name="flags">Flags to use</param>
		/// <returns>Name of the TrackedStat</returns>
		protected virtual string PrepareFlagCache(TrackedStat<T>[] trackedStats, T[] flags)
		{
			flagCache.ClearFlags();
			flagCache.SetFlags(true, flags);

			BeforeTrackedStatAdded();

			return TrackedStatFlagToString(trackedStats, flagCache.GetAllTrueFlags().ToArray());
		}

		/// <summary>
		/// Called before the TrackedStat is added to the StatSystem <para/>
		/// Example: Use this for setting mods with TrackedStatAddFlagCheck()
		/// </summary>
		protected virtual void BeforeTrackedStatAdded() { }

		/// <summary>
		/// Checks if the TrackedStat being added matches the parameters given, if true, add flagToAdd
		/// </summary>
		/// <param name="flagToAdd">Flag to add to the TrackedStat if parameters are matched</param>
		/// <param name="matchType">How to match the flags</param>
		/// <param name="flags">Flags to match with</param>
		protected virtual void TrackedStatAddFlagCheck(T flagToAdd, FlagMatchType matchType, params T[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			for (int i = 0; i < flags.Length; i++)
			{
				if(flagCache.HasFlag(flags[i]))
				{
					flagCache.SetFlag(true, flagToAdd);
					break;
				}
			}
		}

		#endregion

		#endregion

		#region Removing

		/// <summary>
		/// Removes a TrackedStat from the StatSystem
		/// </summary>
		/// <param name="trackedStats">TrackedStats tracked by the target TrackedStat</param>
		/// <param name="flags">Flags of the target TrackedStat</param>
		/// <returns>Did the TrackedStat get removed</returns>
		#if ODIN_INSPECTOR
		[Button(ButtonHeight = 25, Expanded = true)]
		[TabGroup("Utility")]
		[PropertyOrder(5)]
		#endif
		public virtual bool RemovedTrackedStat(TrackedStat<T>[] trackedStats = null, T[] flags = null)
		{
			string name = PrepareFlagCache(trackedStats, flags);

			if(this.trackedStats.ContainsKey(name))
			{
				this.trackedStats[name].UnsubscribeFromStatSystem();
				this.trackedStats.Remove(name);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Removes all TrackedStats from the StatSystem
		/// </summary>
		#if ODIN_INSPECTOR
		[Button(ButtonHeight = 25, Expanded = true)]
		[TabGroup("Utility")]
		[PropertyOrder(7)]
		#endif
		public virtual void RemoveAllTrackedStats()
		{
			foreach (var item in trackedStats)
			{
				item.Value.UnsubscribeFromStatSystem();
			}

			trackedStats.Clear();
		}

		#endregion

		#region Retrieving

		/// <summary>
		/// Retrieves a TrackedStat from the StatSystem
		/// </summary>
		/// <param name="trackedStats">TrackedStats tracked by the target TrackedStat</param>
		/// <param name="flags">Flags of the target TrackedStat</param>
		/// <returns>Retrieved TrackedStat</returns>
		#if ODIN_INSPECTOR
		[Button(ButtonHeight = 25, Expanded = true)]
		[TabGroup("Utility")]
		[PropertyOrder(6)]
		#endif
		public virtual TrackedStat<T> GetTrackedStat(TrackedStat<T>[] trackedStats = null, T[] flags = null)
		{
			string name = PrepareFlagCache(trackedStats, flags);

			if (this.trackedStats.ContainsKey(name))
			{
				return this.trackedStats[name];
			}
			else
			{
				throw new ArgumentException($"TrackedStat of name '{name}' does not exist in the StatSystem");
			}
		}

		#endregion

		#region Internal

		/// <summary>
		/// Converts an array of TrackedStats and Ts into a string
		/// </summary>
		/// <param name="trackedStats">TrackedStats to convert</param>
		/// <param name="flags">Flags to convert</param>
		/// <returns>Converted string</returns>
		protected virtual string TrackedStatFlagToString(TrackedStat<T>[] trackedStats, T[] flags)
		{
			string result = "";

			if(flags != null)
			{
				Array.Sort(flags);

				if (flags != null)
				{
					for (int i = 0; i < flags.Length; i++)
					{
						result += $"{flags[i]} ";
					}
				}
			}
			
			if(trackedStats != null)
			{
				Array.Sort(trackedStats);

				if (trackedStats != null)
				{
					for (int i = 0; i < trackedStats.Length; i++)
					{
						result += $"({trackedStats[i].Name})";
					}
				}
			}

			return result;
		}

		#endregion

		#endregion
	}
}