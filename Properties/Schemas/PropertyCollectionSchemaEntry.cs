using System.Collections.Generic;

namespace Exanite.Core.Properties.Schemas
{
    public class PropertyCollectionSchemaEntry
    {
        public PropertyCollectionSchemaEntry(PropertyDefinition definition, bool isRequired, List<IPropertyValidator> propertyValidators)
        {
            Definition = definition;
            IsRequired = isRequired;
            PropertyValidators = new List<IPropertyValidator>(propertyValidators);
        }

        public PropertyDefinition Definition { get; }
        public bool IsRequired { get; }

        public List<IPropertyValidator> PropertyValidators { get; }
    }
}