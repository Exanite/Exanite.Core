#nullable enable

namespace Exanite.Core.Properties.Schemas
{
    public class PropertyCollectionSchemaValidationError
    {
        public PropertyCollectionSchemaValidationError(PropertyCollectionSchemaEntry entry, IPropertyValidator failedValidator, Property? property)
        {
            Entry = entry;
            FailedValidator = failedValidator;
            Property = property;
        }

        public PropertyCollectionSchemaEntry Entry { get; }
        public IPropertyValidator FailedValidator { get; }
        public Property? Property { get; }

        public PropertyDefinition Definition => Entry.Definition;
    }
}