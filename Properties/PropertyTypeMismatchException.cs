using System;

namespace Exanite.Core.Properties
{
    public class PropertyTypeMismatchException : Exception
    {
        public Type ExistingType { get; }
        public Type RequestedType { get; }

        public PropertyTypeMismatchException(Type existingType, Type requestedType)
            : base($"Found existing property with type '{existingType}', but requested type was '{requestedType}'")
        {
            ExistingType = existingType;
            RequestedType = requestedType;
        }
    }
}