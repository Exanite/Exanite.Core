using System;
using System.Collections.Generic;

namespace Exanite.StatSystem
{
	public class TrackedStat
	{
		#region Fields and Properties

		protected float baseValue;
		protected float flatValue;
		protected float incValue;
		protected float multValue;
		protected float finalValue;

		public float BaseValue
		{
			get
			{
				return baseValue;
			}

			protected set
			{
				baseValue = value;
			}
		}
		public float FlatValue
		{
			get
			{
				return flatValue;
			}

			protected set
			{
				flatValue = value;
			}
		}
		public float IncValue
		{
			get
			{
				return incValue;
			}

			protected set
			{
				incValue = value;
			}
		}
		public float MultValue
		{
			get
			{
				return multValue;
			}

			protected set
			{
				multValue = value;
			}
		}
		public float FinalValue
		{
			get
			{
				return finalValue;
			}

			protected set
			{
				finalValue = value;
			}
		}

		#endregion
	}
}