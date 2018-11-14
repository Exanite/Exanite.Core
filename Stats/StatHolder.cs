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
		[SerializeField]protected float _FinalValue;

		public readonly List<StatModifier> _statModifiers;
		public readonly ReadOnlyCollection<StatModifier> _StatModifiers;

		#region Initialization
		
		public StatHolder()
		{
			_statModifiers = new List<StatModifier>();
			_StatModifiers = _statModifiers.AsReadOnly();
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
			_statModifiers.Add(mod);
		}

		public virtual StatModifier AddModifier(float value, StatModifierType type, object source)
		{
			isDirty = true;
			StatModifier mod = new StatModifier(value, type, source);
			AddModifier(mod);
			return mod;
		}

		public virtual void AddModifiers(params StatModifier[] mods)
		{
			isDirty = true;
			foreach(StatModifier mod in mods)
			{
				AddModifier(mod);
			}
		}

		public virtual bool RemoveModifier(StatModifier mod)
		{
			if(_statModifiers.Remove(mod))
			{
				isDirty = true;
				return true;
			}

			return false;
		}

		public virtual bool RemoveAllModifiersFromSource(object source)
		{
			bool didRemove = false;

			for(int i = _statModifiers.Count - 1; i >= 0; i--)
			{
				if(_statModifiers[i].Source == source)
				{
					isDirty = true;
					didRemove = true;
					_statModifiers.RemoveAt(i);
				}
			}
			
			return didRemove;
		}

		public virtual void RemoveAllModifiers()
		{
			isDirty = true;
			_statModifiers.Clear();
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

		public virtual float CalculateFinalValue()
		{
			FlatValue = 0f;
			IncValue = 1f;
			MultValue = 1f;

			for (int i = 0; i < _statModifiers.Count; i++)
			{
				StatModifier mod = _statModifiers[i];

				if (mod.Type == StatModifierType.Flat)
				{
					FlatValue += _statModifiers[i].Value;
				}
				else if (mod.Type ==StatModifierType.Inc)
				{
					IncValue += mod.Value;
				}
				else if (mod.Type == StatModifierType.Mult)
				{
				MultValue *= _statModifiers[i].Value;
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
	
		public virtual void Clear()
		{
			BaseValue = 0f;
			_statModifiers.Clear();
			CalculateFinalValue();
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