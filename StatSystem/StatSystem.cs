using System;
using System.Collections.Generic;
using Exanite.StatSystem.Internal;

namespace Exanite.StatSystem
{
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

		public delegate void ModEvent(StatMod mod);
		public ModEvent ModAdded;
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

		#region AND

		/// <summary>
		/// Returns all modifiers with all of the flags in the provided LongFlags
		/// </summary>
		/// <param name="longFlags">LongFlags to compare</param>
		/// <returns>List of all modifiers with all of the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsAND(LongFlag longFlags)
		{
			return GetAllModsWithFlagsAND(longFlags.GetAllTrueFlags().ToArray());
		}

		/// <summary>
		/// Returns all modifiers with all of the flags provided
		/// </summary>
		/// <param name="flags">Flags to compare</param>
		/// <returns>List of all modifiers with all of the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsAND(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			List<StatMod> mods = new List<StatMod>();

			foreach (StatMod mod in Modifiers)
			{
				if (mod.Flags.HasFlagsAND(flags))
				{
					mods.Add(mod);
				}
			}

			return mods;
		}

		#endregion

		#region OR

		/// <summary>
		/// Returns all modifiers with any of the flags in the provided LongFlags
		/// </summary>
		/// <param name="longFlags">LongFlags to compare</param>
		/// <returns>List of all modifiers with any of the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsOR(LongFlag longFlags)
		{
			return GetAllModsWithFlagsOR(longFlags.GetAllTrueFlags().ToArray());
		}

		/// <summary>
		/// Returns all modifiers with all of the flags provided
		/// </summary>
		/// <param name="flags">Flags to compare</param>
		/// <returns>List of all modifiers with any of the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsOR(params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			List<StatMod> mods = new List<StatMod>();

			foreach (StatMod mod in Modifiers)
			{
				if (mod.Flags.HasFlagsOR(flags))
				{
					mods.Add(mod);
				}
			}

			return mods;
		}

		#endregion

		#region EQUALS

		/// <summary>
		/// Returns all modifiers with only the flags in the provided LongFlags
		/// </summary>
		/// <param name="longFlags">LongFlags to compare</param>
		/// <returns>List of all modifiers with only of the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsEQUALS(LongFlag longFlags)
		{
			return GetAllModsWithFlagsEQUALS(longFlags.GetAllTrueFlags().ToArray());
		}

		/// <summary>
		/// Returns all modifiers with only the flags provided
		/// </summary>
		/// <param name="flags">Flags to compare</param>
		/// <returns>List of all modifiers with only the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsEQUALS(params Enum[] flags)
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
	}
}