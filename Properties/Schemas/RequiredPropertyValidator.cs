namespace Exanite.Core.Properties.Schemas
{
    public class RequiredPropertyValidator : IPropertyValidator
    {
        public bool Validate(Property property)
        {
            return property != null;
        }
    }
}