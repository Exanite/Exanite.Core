using System;

namespace Exanite.Core.Properties
{
    public class PropertyValueChangedEventArgs<T> : EventArgs
    {
        public PropertyValueChangedEventArgs()
        {
            // Intentionally blank
        }

        public PropertyValueChangedEventArgs(Property property)
        {
            Property = property;
        }

        public Property Property { get; set; }
        public T PreviousValue { get; set; }
        public T NewValue { get; set; }
    }
}