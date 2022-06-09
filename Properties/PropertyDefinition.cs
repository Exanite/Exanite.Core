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
    }
    
    public class PropertyDefinition<T> : PropertyDefinition
    {
        public PropertyDefinition(string name, T initialValue = default) : base(name, typeof(T))
        {
            InitialValue = initialValue;
        }
        
        public T InitialValue { get; }
    }
}