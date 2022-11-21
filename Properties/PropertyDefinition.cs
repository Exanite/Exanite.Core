using System;

namespace Exanite.Core.Properties
{
    public abstract class PropertyDefinition
    {
        public string Name { get; }
        public Type Type { get; }

        protected PropertyDefinition(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public abstract Property CreateProperty();
    }

    public class PropertyDefinition<T> : PropertyDefinition
    {
        public PropertyDefinition(string name) : base(name, typeof(T)) {}

        public override Property CreateProperty()
        {
            return new Property<T>(Name);
        }
    }
}