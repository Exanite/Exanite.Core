using System.Collections.Generic;

namespace Exanite.Core.Properties.Schemas
{
    public class PropertyCollectionSchema
    {
        // Default validators
        private readonly RequiredPropertyValidator requiredPropertyValidator = new RequiredPropertyValidator();
        private readonly PropertyTypeValidator propertyTypeValidator = new PropertyTypeValidator();
        
        private readonly List<PropertyCollectionSchemaValidationError> errorsCache = new List<PropertyCollectionSchemaValidationError>();

        public PropertyCollectionSchema(List<PropertyCollectionSchemaEntry> entries)
        {
            Entries = entries;
        }

        public List<PropertyCollectionSchemaEntry> Entries { get; }

        public bool Validate(PropertyCollection collection)
        {
            return Validate(collection, errorsCache);
        }

        public bool Validate(PropertyCollection collection, out List<PropertyCollectionSchemaValidationError> errors)
        {
            errors = new List<PropertyCollectionSchemaValidationError>();

            return Validate(collection, errors);
        }

        public bool Validate(PropertyCollection collection, List<PropertyCollectionSchemaValidationError> errors)
        {
            errors.Clear();

            foreach (var entry in Entries)
            {
                collection.PropertiesByName.TryGetValue(entry.Definition.Name, out var property);

                if (!ValidateDefault(errors, property, entry))
                {
                    continue;
                }

                foreach (var validator in entry.PropertyValidators)
                {
                    if (!validator.Validate(property))
                    {
                        errors.Add(new PropertyCollectionSchemaValidationError(entry, validator, property));

                        break;
                    }
                }
            }

            return errors.Count == 0;
        }

        private bool ValidateDefault(List<PropertyCollectionSchemaValidationError> errors, Property property, PropertyCollectionSchemaEntry entry)
        {
            if (!requiredPropertyValidator.Validate(property))
            {
                if (entry.IsRequired)
                {
                    errors.Add(new PropertyCollectionSchemaValidationError(entry, requiredPropertyValidator, property));
                }

                return false;
            }

            propertyTypeValidator.ExpectedType = entry.Definition.Type;
            if (!propertyTypeValidator.Validate(property))
            {
                errors.Add(new PropertyCollectionSchemaValidationError(entry, propertyTypeValidator, property));

                return false;
            }

            return true;
        }
    }
}