using System.Collections.Generic;

namespace Exanite.Core.Properties.Schemas
{
    public class PropertyCollectionSchemaEntryBuilder
    {
        private readonly PropertyDefinition definition;
        private bool isRequired = true;

        private readonly List<IPropertyValidator> propertyValidators = new();

        public PropertyCollectionSchemaEntryBuilder(PropertyDefinition definition)
        {
            this.definition = definition;
        }

        public PropertyCollectionSchemaEntryBuilder AsOptional()
        {
            isRequired = false;

            return this;
        }

        public PropertyCollectionSchemaEntryBuilder WithValueNotNull()
        {
            return WithValidator(new PropertyValueNotNullValidator());
        }

        public PropertyCollectionSchemaEntryBuilder WithValidator(IPropertyValidator validator)
        {
            propertyValidators.Add(validator);

            return this;
        }

        public PropertyCollectionSchemaEntry Build()
        {
            return new PropertyCollectionSchemaEntry(definition, isRequired, propertyValidators);
        }
    }
}