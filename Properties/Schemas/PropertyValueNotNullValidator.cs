namespace Exanite.Core.Properties.Schemas
{
    public class PropertyValueNotNullValidator : IPropertyValidator
    {
        public bool Validate(Property property)
        {
            return property.UntypedValue != null;
        }
    }
}