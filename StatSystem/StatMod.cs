using Exanite.StatSystem.Internal;
using System;
using System.Collections.Generic;

namespace Exanite.StatSystem
{
	public class StatMod
	{
		#region Fields and Properties

		public float Value;
		public StatModType Type;
		protected object source;
		protected LongFlags flags;

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
		public LongFlags Flags
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

		public StatMod(float value, StatModType type, object source, params Enum[] flags)
		{
			Value = value;
			Type = type;
			Source = source;

			#region Initiate LongFlags

			if (flags == null) throw new ArgumentException("No arguments were passed");

			List<Type> enumFlagTypes = new List<Type>();

			foreach (Enum flag in flags)
			{
				if (!enumFlagTypes.Contains(flag.GetType()))
				{
					enumFlagTypes.Add(flag.GetType());
				}
			}

			Flags = new LongFlags(enumFlagTypes.ToArray());

			#endregion
		}

		#endregion
	}
}
