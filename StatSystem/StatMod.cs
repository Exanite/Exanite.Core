using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Exanite.Extensions;
using Exanite.Flags;

namespace Exanite.StatSystem
{
	/// <summary>
	/// Class used in the StatSystem to modify existing stats
	/// </summary>
	public class StatMod
	{
		#region Fields and Properties

		protected string name;

		[HideInInspector] [OdinSerialize] protected float value;
		[HideInInspector] [OdinSerialize] protected StatModType type;
		[HideInInspector] [OdinSerialize] protected object source;
		[HideInInspector] [OdinSerialize] protected LongFlag<StatModFlag> flags;

		/// <summary>
		/// Automatically generated name for this modifier
		/// </summary>
		[ShowInInspector]
		public string Name
		{
			get
			{
				if(string.IsNullOrEmpty(name))
				{
					foreach(Enum flag in Flags.GetAllTrueFlags())
					{
						name += $"{flag} ";
						name.Trim();
					}
				}
				return name;
			}

			protected set
			{
				name = value;
			}
		}

		/// <summary>
		/// What flags the modifier has
		/// </summary>
		public LongFlag<StatModFlag> Flags
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
		/// Value of the mod
		/// </summary>
		[ShowInInspector]
		public float Value
		{
			get
			{
				return value;
			}

			protected set
			{
				this.value = value;
			}
		}
		/// <summary>
		/// How the modifier is applied to existing stats
		/// </summary>
		[ShowInInspector]
		public StatModType Type
		{
			get
			{
				return type;
			}

			protected set
			{
				type = value;
			}
		}
		/// <summary>
		/// Where the mod came from
		/// </summary>
		[ShowInInspector]
		public object Source
		{
			get
			{
				return source;
			}

			protected set
			{
				source = value;
			}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a new StatMod
		/// </summary>
		/// <param name="value">Value of the mod</param>
		/// <param name="type">How the modifier is applied to existing stats</param>
		/// <param name="source">Where the mod came from, usually "this"</param>
		/// <param name="flags">What flags the modifier has</param>
		public StatMod(float value, StatModType type, object source, params StatModFlag[] flags)
		{
			if (flags.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(flags));
			}

			Value = value;
			Type = type;
			Source = source;
			Flags = new LongFlag<StatModFlag>(flags);
		}

		#endregion

		#region Matching

		/// <summary>
		/// Returns true if the LongFlag provided matches the flags of this modifier
		/// </summary>
		/// <param name="mod">LongFlag to compare</param>
		/// <returns>True or false</returns>
		public virtual bool IsMatch(LongFlag<StatModFlag> flags)
		{
			bool hasBaseFlag = Flags.HasFlag(StatModFlag.Base);

			if (hasBaseFlag)
			{
				Flags.SetFlag(false, StatModFlag.Base);
			}

			bool matchSuccess = flags.HasFlags(FlagMatchType.And, Flags);

			if (hasBaseFlag)
			{
				Flags.SetFlag(true, StatModFlag.Base);
			}

			return matchSuccess;
		}

		#endregion
	}
}