namespace Exanite.Stats
{
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
}
