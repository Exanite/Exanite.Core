using System;

namespace Exanite.Core.Properties
{
    public abstract class PropertyDefinition
    {
        public string Key { get; }
        public Type Type { get; }

        protected PropertyDefinition(string key, Type type)
        {
            Key = key;
            Type = type;
        }

        public abstract Property CreateProperty();
    }

    public class PropertyDefinition<T> : PropertyDefinition
    {
        public PropertyDefinition(string key) : base(key, typeof(T)) {}

        public override Property CreateProperty()
        {
            return new Property<T>(Key);
        }
    }
}
