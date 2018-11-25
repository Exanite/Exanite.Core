using System;
using System.Collections.Generic;
using Exanite.StatSystem.Internal;

namespace Exanite.StatSystem
{
	public class StatSystem
	{
		#region Fields

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

		#region Mod/Flag Logic

		#region AND

		/// <summary>
		/// Returns all modifiers with all of the flags in the provided LongFlags
		/// </summary>
		/// <param name="longFlags">LongFlags to compare</param>
		/// <returns>List of all modifiers with all of the provided flags</returns>
		public virtual List<StatMod> GetAllModsWithFlagsAND(LongFlags longFlags)
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
		public virtual List<StatMod> GetAllModsWithFlagsOR(LongFlags longFlags)
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
		public virtual List<StatMod> GetAllModsWithFlagsEQUALS(LongFlags longFlags)
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
	}
}