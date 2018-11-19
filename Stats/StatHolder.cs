using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Exanite.Stats
{
	[Serializable]
	public class StatHolder
	{
		#region Fields and Properties

		public float BaseValue;
		protected float _flatValue;
		protected float _incValue;
		protected float _multValue;
		[SerializeField] protected float _finalValue;

		public readonly List<StatModifier> _statModifiers;
		public readonly ReadOnlyCollection<StatModifier> _StatModifiers;

		protected bool isDirty = true;
		protected float lastBaseValue;

		public virtual float FinalValue
		{
			get
			{
				if (isDirty || BaseValue != lastBaseValue)
				{
					lastBaseValue = BaseValue;
					_finalValue = CalculateFinalValue();
					isDirty = false;
				}
				return _finalValue;
			}
		}

		#endregion

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
			foreach (StatModifier mod in mods)
			{
				AddModifier(mod);
			}
		}

		public virtual bool RemoveModifier(StatModifier mod)
		{
			if (_statModifiers.Remove(mod))
			{
				isDirty = true;
				return true;
			}

			return false;
		}

		public virtual bool RemoveAllModifiersFromSource(object source)
		{
			bool didRemove = false;

			for (int i = _statModifiers.Count - 1; i >= 0; i--)
			{
				if (_statModifiers[i].Source == source)
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

		public virtual float CalculateFinalValue()
		{
			_flatValue = 0f;
			_incValue = 1f;
			_multValue = 1f;

			for (int i = 0; i < _statModifiers.Count; i++)
			{
				StatModifier mod = _statModifiers[i];

				if (mod.Type == StatModifierType.Flat)
				{
					_flatValue += _statModifiers[i].Value;
				}
				else if (mod.Type == StatModifierType.Inc)
				{
					_incValue += mod.Value;
				}
				else if (mod.Type == StatModifierType.Mult)
				{
					_multValue *= _statModifiers[i].Value;
				}
			}

			isDirty = false;

			StatRecalculated?.Invoke();

			return (float)Math.Round((BaseValue + _flatValue) * _incValue * _multValue, 4);
		}

		#endregion

		#region Extras

		public delegate void StatEvent();
		public StatEvent StatRecalculated;

		public virtual List<StatModifier> GetCombinedModifiers()
		{
			List<StatModifier> mods = new List<StatModifier>();

			CalculateFinalValue();

			mods.Add(new StatModifier(BaseValue + _flatValue, StatModifierType.Flat, this));
			mods.Add(new StatModifier(Mathf.Clamp(_incValue - 1f, 0f, Mathf.Infinity), StatModifierType.Inc, this));
			mods.Add(new StatModifier(_multValue, StatModifierType.Mult, this));

			return mods;
		}

		public virtual StatHolder CombineStatHolders(params StatHolder[] statsToCombine)
		{
			StatHolder combined = new StatHolder();

			foreach(StatHolder stat in statsToCombine)
			{
				combined.AddModifiers(stat.GetCombinedModifiers().ToArray());
			}

			return combined;
		}

		public virtual void Clear()
		{
			BaseValue = 0f;
			_statModifiers.Clear();
			CalculateFinalValue();
		}

		#endregion
	}
}