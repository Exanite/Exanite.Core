using System;
using System.Collections.Generic;
using Exanite.Core.Events;

namespace Exanite.Core.Properties
{
    public class PropertyCollection
    {
        private Dictionary<string, Property> properties = new(StringComparer.Ordinal);

        public IReadOnlyDictionary<string, Property> PropertiesByKey => properties;

        public event EventHandler<PropertyCollection, Property> PropertyAdded;
        public event EventHandler<PropertyCollection, Property> PropertyRemoved;

        public event EventHandler<PropertyCollection, PropertyValueChangedEventArgs<object>> PropertyValueChanged;

        public T GetPropertyValue<T>(PropertyDefinition<T> definition, bool addIfDoesNotExist = false)
        {
            var property = addIfDoesNotExist ? GetOrAddProperty(definition) : GetProperty(definition);

            if (property != null)
            {
                return property.Value;
            }

            throw new PropertyNotFoundException(definition);
        }

        public void SetPropertyValue<T>(PropertyDefinition<T> definition, T value, bool addIfDoesNotExist = false)
        {
            var property = addIfDoesNotExist ? GetOrAddProperty(definition) : GetProperty(definition);

            if (property != null)
            {
                property.Value = value;
            }
            else
            {
                throw new PropertyNotFoundException(definition);
            }
        }

        public Property GetProperty(string key)
        {
            if (!properties.TryGetValue(key, out var untypedProperty))
            {
                return null;
            }

            return untypedProperty;
        }

        public Property GetProperty(PropertyDefinition definition)
        {
            if (!properties.TryGetValue(definition.Key, out var untypedProperty))
            {
                return null;
            }

            if (untypedProperty.Type != definition.Type)
            {
                throw new PropertyTypeMismatchException(untypedProperty.Type, definition.Type);
            }

            return untypedProperty;
        }

        public Property<T> GetProperty<T>(PropertyDefinition<T> definition)
        {
            return (Property<T>)GetProperty((PropertyDefinition)definition);
        }

        public bool TryGetProperty<T>(PropertyDefinition<T> definition, out Property<T> property)
        {
            property = GetProperty(definition);

            return property != null;
        }

        public Property GetOrAddProperty(PropertyDefinition definition)
        {
            Property property;

            if (properties.TryGetValue(definition.Key, out var untypedProperty))
            {
                if (untypedProperty.Type != definition.Type)
                {
                    throw new PropertyTypeMismatchException(untypedProperty.Type, definition.Type);
                }

                property = untypedProperty;
            }
            else
            {
                property = AddProperty(definition);
            }

            return property;
        }

        public Property<T> GetOrAddProperty<T>(PropertyDefinition<T> definition)
        {
            Property<T> property;

            if (properties.TryGetValue(definition.Key, out var untypedProperty))
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

        public bool HasProperty(PropertyDefinition definition)
        {
            if (properties.TryGetValue(definition.Key, out var untypedProperty))
            {
                return untypedProperty.Type == definition.Type;
            }

            return false;
        }

        public Property AddProperty(PropertyDefinition definition)
        {
            var property = definition.CreateProperty();
            properties.Add(property.Key, property);

            OnPropertyAdded(property);

            return property;
        }

        public Property<T> AddProperty<T>(PropertyDefinition<T> definition)
        {
            var property = definition.CreateProperty();
            properties.Add(property.Key, property);

            OnPropertyAdded(property);

            return (Property<T>)property;
        }

        public bool RemoveProperty(string key)
        {
            if (!properties.TryGetValue(key, out var property))
            {
                return false;
            }

            properties.Remove(key);
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
