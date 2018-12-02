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
	public class StatMod : ISerializationCallbackReceiver
	{
		#region Fields and Properties

		[HideInInspector] [OdinSerialize] protected string name;
		[HideInInspector] [OdinSerialize] protected float value;
		[HideInInspector] [OdinSerialize] protected StatModType type;
		[HideInInspector] [OdinSerialize] protected object source;
		[OdinSerialize] protected LongFlag<StatModFlag> flags;

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
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			Value = value;
			Type = type;
			Source = source;
			Flags = new LongFlag<StatModFlag>(flags);
		}

		/// <summary>
		/// Creates a new StatMod with the passed name
		/// </summary>
		/// <param name="name">Name of the mod</param>
		/// <param name="value">Value of the mod</param>
		/// <param name="type">How the modifier is applied to existing stats</param>
		/// <param name="source">Where the mod came from, usually "this"</param>
		/// <param name="flags">What flags the modifier has</param>
		public StatMod(string name, float value, StatModType type, object source, params StatModFlag[] flags) : this(value, type, source, flags)
		{
			this.name = name;
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

		#region Serialization

		/// <summary>
		/// Prepares the class for Serialization
		/// </summary>
		public virtual void OnBeforeSerialize() { }

		/// <summary>
		/// Recalculates the field 'name' after deserialization
		/// </summary>
		public void OnAfterDeserialize()
		{
			name = "";
			foreach (Enum flag in Flags.GetAllTrueFlags())
			{
				name += $"{flag} ";
				name.Trim();
			}
		}

		#endregion
	}
}