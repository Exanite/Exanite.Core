using System;

namespace Exanite.Core.Properties.Schemas
{
    public class PropertyTypeValidator : IPropertyValidator
    {
        public Type ExpectedType;

        public bool Validate(Property property)
        {
            return property.Type == ExpectedType;
        }
    }
}