using System;
using Exanite.Core.Events;

namespace Exanite.Core.Properties
{
    public abstract class Property
    {
        private readonly PropertyValueChangedEventArgs<object> propertyValueChangedEventArgs;

        public string Key { get; }
        public Type Type { get; }

        public abstract object UntypedValue { get; set; }

        public event EventHandler<Property, PropertyValueChangedEventArgs<object>> UntypedValueChanged;

        protected Property(string key, Type type)
        {
            propertyValueChangedEventArgs = new PropertyValueChangedEventArgs<object>(this);

            Key = key;
            Type = type;
        }

        protected void OnValueChangedUntyped(object previousValue, object newValue)
        {
            propertyValueChangedEventArgs.PreviousValue = previousValue;
            propertyValueChangedEventArgs.NewValue = newValue;

            UntypedValueChanged?.Invoke(this, propertyValueChangedEventArgs);
        }
    }

    public class Property<T> : Property
    {
        private T value;

        private readonly PropertyValueChangedEventArgs<T> propertyValueChangedEventArgs;

        public T Value
        {
            get => value;
            set
            {
                if (Equals(this.value, value))
                {
                    return;
                }

                var previousValue = this.value;
                this.value = value;
                OnValueChanged(previousValue, value);
            }
        }

        public override object UntypedValue
        {
            get => value;
            set => this.value = (T)value;
        }

        public event EventHandler<Property, PropertyValueChangedEventArgs<T>> ValueChanged;

        public Property(string key) : base(key, typeof(T))
        {
            propertyValueChangedEventArgs = new PropertyValueChangedEventArgs<T>(this);
        }

        private void OnValueChanged(T previousValue, T newValue)
        {
            propertyValueChangedEventArgs.PreviousValue = previousValue;
            propertyValueChangedEventArgs.NewValue = newValue;

            ValueChanged?.Invoke(this, propertyValueChangedEventArgs);
            OnValueChangedUntyped(previousValue, newValue);
        }
    }
}
