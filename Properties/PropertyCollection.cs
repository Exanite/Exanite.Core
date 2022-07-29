using System;
using System.Collections.Generic;
using Exanite.Core.Events;

namespace Exanite.Core.Properties
{
    public class PropertyCollection
    {
        private Dictionary<string, Property> properties = new Dictionary<string, Property>(StringComparer.Ordinal);

        public IReadOnlyDictionary<string, Property> PropertiesByName => properties;

        public event EventHandler<PropertyCollection, Property> PropertyAdded;
        public event EventHandler<PropertyCollection, Property> PropertyRemoved;

        public event EventHandler<PropertyCollection, PropertyValueChangedEventArgs<object>> PropertyValueChanged;

        public T GetPropertyValue<T>(PropertyDefinition<T> definition)
        {
            return GetProperty(definition).Value;
        }
        
        public void SetPropertyValue<T>(PropertyDefinition<T> definition, T value)
        {
            GetProperty(definition).Value = value;
        }
        
        public Property<T> GetProperty<T>(PropertyDefinition<T> definition)
        {
            Property<T> property;

            if (properties.TryGetValue(definition.Name, out var untypedProperty))
            {
                if (untypedProperty.Type != definition.Type)
                {
                    throw new PropertyTypeMismatchException(untypedProperty.Type, definition.Type);
                }

                property = (Property<T>)untypedProperty;
            }
            else
            {
                property = CreateProperty(definition);
            }

            return property;
        }

        public bool HasProperty<T>(PropertyDefinition<T> definition)
        {
            if (properties.TryGetValue(definition.Name, out var untypedProperty))
            {
                return untypedProperty.Type == definition.Type;
            }

            return false;
        }

        public bool RemoveProperty(string name)
        {
            if (!properties.TryGetValue(name, out var property))
            {
                return false;
            }

            properties.Remove(name);
            OnPropertyRemoved(property);

            return true;
        }

        public void Clear()
        {
            var oldProperties = properties;
            properties = new Dictionary<string, Property>();

            foreach (var kvp in oldProperties)
            {
                OnPropertyRemoved(kvp.Value);
            }
        }

        private Property<T> CreateProperty<T>(PropertyDefinition<T> definition)
        {
            var property = new Property<T>(definition.Name, definition.InitialValue);
            properties.Add(property.Name, property);

            OnPropertyAdded(property);

            return property;
        }
        
        private void OnPropertyAdded(Property property)
        {
            property.UntypedValueChanged += Property_OnUntypedValueChanged;
            PropertyAdded?.Invoke(this, property);
        }

        private void OnPropertyRemoved(Property property)
        {
            PropertyRemoved?.Invoke(this, property);
            property.UntypedValueChanged -= Property_OnUntypedValueChanged;
        }
        
        private void Property_OnUntypedValueChanged(Property sender, PropertyValueChangedEventArgs<object> e)
        {
            PropertyValueChanged?.Invoke(this, e);
        }
    }
}