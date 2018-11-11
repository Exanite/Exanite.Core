using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Exanite.Stats
{
	[Serializable]
	public class StatHolder
	{
		public float BaseValue;
		public float FlatValue;
		public float IncValue;
		public float MultValue;
		public float _FinalValue;

		public readonly List<StatModifier> statModifiers;
		public readonly ReadOnlyCollection<StatModifier> StatModifiers;

		#region Initialization
		public StatHolder()
		{
			statModifiers = new List<StatModifier>();
			StatModifiers = statModifiers.AsReadOnly();
		}

		public StatHolder(float baseValue) : this()
		{
			BaseValue = baseValue;
		}
		#endregion

		#region Adding/Removing Modifiers
		public virtual void AddModifier(StatModifier mod)
		{
			isDirty = true;
			statModifiers.Add(mod);
		}

		public virtual void AddModifiers(params StatModifier[] mods)
		{
			foreach(StatModifier mod in mods)
			{
				AddModifier(mod);
			}
		}

		public virtual bool RemoveModifier(StatModifier mod)
		{
			if(statModifiers.Remove(mod))
			{
				isDirty = true;
				return true;
			}

			return false;
		}

		public virtual bool RemoveAllModifiersFromSource(object source)
		{
			bool didRemove = false;

			for(int i = statModifiers.Count - 1; i >= 0; i--)
			{
				if(statModifiers[i].Source == source)
				{
					isDirty = true;
					didRemove = true;
					statModifiers.RemoveAt(i);
				}
			}
			
			return didRemove;
		}

		public virtual void RemoveAllModifiers()
		{
			isDirty = true;
			statModifiers.Clear();
		}
		#endregion

		#region Calculating FinalValue
		
		protected bool isDirty = true;
		protected float lastBaseValue;

		public virtual float FinalValue
		{
			get
			{
				if (isDirty || BaseValue != lastBaseValue)
				{
					lastBaseValue = BaseValue;
					_FinalValue =  CalculateFinalValue();
					isDirty = false;
				}
				return _FinalValue;
			}
		}

		protected virtual float CalculateFinalValue()
		{
			FlatValue = 0f;
			IncValue = 1f;
			MultValue = 1f;

			for (int i = 0; i < statModifiers.Count; i++)
			{
				StatModifier mod = statModifiers[i];

				if (mod.Type == StatModifierType.Flat)
				{
					FlatValue += statModifiers[i].Value;
				}
				else if (mod.Type ==StatModifierType.Inc)
				{
					IncValue += mod.Value;
				}
				else if (mod.Type == StatModifierType.Mult)
				{
				MultValue *= statModifiers[i].Value;
				}
			}

			return (float)Math.Round((BaseValue + FlatValue) * IncValue * MultValue, 4);
		}
		#endregion

		public virtual List<StatModifier> GetCombinedModifiers()
		{
			List<StatModifier> mods = new List<StatModifier>();

			CalculateFinalValue();

			mods.Add(new StatModifier(BaseValue + FlatValue, StatModifierType.Flat, this));
			mods.Add(new StatModifier(Mathf.Clamp(IncValue - 1f, 0f, Mathf.Infinity), StatModifierType.Inc, this));
			mods.Add(new StatModifier(MultValue, StatModifierType.Mult, this));

			return mods;
		}
	}

	#region StatMod
	public enum StatModifierType
	{
		Flat,
		Inc,
		Mult,
	}

	public class StatModifier 
	{
		public readonly float Value;
		public readonly StatModifierType Type;
		public readonly object Source;

		public StatModifier(float value, StatModifierType type, object source)
		{
			Value = value;
			Type = type;
			Source = source;
		}
	}
	#endregion
}