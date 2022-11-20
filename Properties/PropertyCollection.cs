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

        public T GetPropertyValue<T>(PropertyDefinition<T> definition, bool addIfDoesNotExist = false)
        {
            if (addIfDoesNotExist)
            {
                return GetOrAddProperty(definition).Value;
            }

            try
            {
                return GetProperty(definition).Value;
            }
            catch (NullReferenceException e)
            {
                throw new InvalidOperationException($"PropertyCollection does not contain Property: {definition.Name}", e);
            }
        }

        public void SetPropertyValue<T>(PropertyDefinition<T> definition, T value, bool addIfDoesNotExist = false)
        {
            var property = addIfDoesNotExist ? GetOrAddProperty(definition) : GetProperty(definition);
            property.Value = value;
        }

        public Property GetUntypedProperty(PropertyDefinition definition)
        {
            if (!properties.TryGetValue(definition.Name, out var untypedProperty))
            {
                return null;
            }

            return untypedProperty;
        }

        public Property<T> GetProperty<T>(PropertyDefinition<T> definition)
        {
            if (!properties.TryGetValue(definition.Name, out var untypedProperty))
            {
                return null;
            }

            if (untypedProperty.Type != definition.Type)
            {
                throw new PropertyTypeMismatchException(untypedProperty.Type, definition.Type);
            }

            return (Property<T>)untypedProperty;
        }

        public bool TryGetProperty<T>(PropertyDefinition<T> definition, out Property<T> property)
        {
            property = GetProperty(definition);

            return property != null;
        }

        public Property<T> GetOrAddProperty<T>(PropertyDefinition<T> definition)
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
                property = AddProperty(definition);
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

        public Property AddProperty(PropertyDefinition definition)
        {
            var property = definition.CreateProperty();
            properties.Add(property.Name, property);

            OnPropertyAdded(property);

            return property;
        }

        public Property<T> AddProperty<T>(PropertyDefinition<T> definition)
        {
            var property = definition.CreateProperty();
            properties.Add(property.Name, property);

            OnPropertyAdded(property);

            return (Property<T>)property;
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