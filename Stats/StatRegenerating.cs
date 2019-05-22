using UnityEngine;

namespace Exanite.Core.Stats
{
    [System.Serializable]
    public class StatRegenerating
    {
        protected float _value;
        public StatHolder Max;
        public StatHolder Regen;

        public float Value
        {
            get
            {
                return _value;
            }

            protected set
            {
                this._value = value;
            }
        }

        public StatRegenerating(float max, float regen)
        {
            Value = max;
            Max = new StatHolder(max);
            Regen = new StatHolder(regen);
        }

        public virtual void Regenerate()
        {
            Value = Mathf.Clamp(Value + (Regen.FinalValue * Time.deltaTime), -1f, Max.FinalValue);
        }

        public virtual void SetMax()
        {
            Value = Max.FinalValue;
        }

        // Negative values increase
        public virtual void Decrease(float value) 
        {
            Value = Mathf.Clamp(Value - value, -1f, Max.FinalValue);
        }
    }
}