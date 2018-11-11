using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exanite.Stats;

namespace Exanite.Stats
{
	[System.Serializable]
	public class StatRegenerating
	{
		public float Value;
		public StatHolder Max;
		public StatHolder Regen;

		public StatRegenerating(float max, float regen)
		{
			Value = max;
			Max = new StatHolder(max);
			Regen = new StatHolder(regen);
		}

		public void Regenerate()
		{
			Value = Mathf.Clamp(Value + (Regen.FinalValue * Time.deltaTime), -1f, Max.FinalValue);
		}

		public void SetMax()
		{
			Value = Max.FinalValue;
		}
	}
}