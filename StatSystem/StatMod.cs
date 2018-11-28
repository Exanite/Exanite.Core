using Exanite.StatSystem.Internal;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace Exanite.StatSystem
{
	/// <summary>
	/// Class used in the StatSystem to modify existing stats
	/// </summary>
	public class StatMod
	{
		#region Fields and Properties

		[HideInInspector] [OdinSerialize] protected string name;
		[HideInInspector] [OdinSerialize] protected float value;
		[HideInInspector] [OdinSerialize] protected StatModType type;
		[HideInInspector] [OdinSerialize] protected object source;
		[HideInInspector] [OdinSerialize] protected LongFlag flags;

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
		/// Where the mod came from
		/// </summary>
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
		/// <summary>
		/// What flags the modifier has
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

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a new StatMod
		/// </summary>
		/// <param name="value">Value of the mod</param>
		/// <param name="type">How the modifier is applied to existing stats</param>
		/// <param name="source">Where the mod came from, usually "this"</param>
		/// <param name="flags">What flags the modifier has</param>
		public StatMod(float value, StatModType type, object source, params Enum[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			Value = value;
			Type = type;
			Source = source;
			Flags = new LongFlag(flags);
		}

		public StatMod(string name, float value, StatModType type, object source, params Enum[] flags) : this(value, type, source, flags)
		{
			this.name = name;
		}

		#endregion
	}
}