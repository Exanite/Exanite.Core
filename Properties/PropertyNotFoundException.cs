using System;

namespace Exanite.Core.Properties
{
    public class PropertyNotFoundException : Exception
    {
        public PropertyDefinition Definition { get; }

        public PropertyNotFoundException(PropertyDefinition definition)
            : base($"PropertyCollection does not contain Property: {definition.Key}")
        {
            Definition = definition;
        }
    }
}