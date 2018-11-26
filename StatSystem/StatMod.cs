using Exanite.StatSystem.Internal;
using System;
using System.Collections.Generic;

namespace Exanite.StatSystem
{
	public class StatMod
	{
		#region Fields and Properties

		/// <summary>
		/// Value of the mod
		/// </summary>
		public float Value;
		/// <summary>
		/// How the modifier is applied to existing stats
		/// </summary>
		public StatModType Type;
		protected object source;
		protected LongFlag flags;

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

		#endregion
	}
}