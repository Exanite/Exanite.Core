using System;

namespace Exanite.Core.Properties.Schemas
{
    public class PropertyTypeValidator : IPropertyValidator
    {
        public Type? ExpectedType;

        public PropertyTypeValidator() {}

        public PropertyTypeValidator(Type expectedType)
        {
            ExpectedType = expectedType;
        }

        public bool Validate(Property? property)
        {
            return property!.Type == ExpectedType;
        }
    }
}
