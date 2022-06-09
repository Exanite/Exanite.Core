using NUnit.Framework;
using Exanite.Core.Properties;

namespace Exanite.Core.Tests.Editor.Properties
{
    [TestFixture]
    public class PropertyCollectionTests
    {
        private const string DefaultPropertyName = "Default";
        
        private static readonly PropertyDefinition<string> StringADefinition = new PropertyDefinition<string>("StringA");
        private static readonly PropertyDefinition<string> StringBDefinition = new PropertyDefinition<string>("StringB");
        
        private PropertyCollection collection;

        [SetUp]
        public void SetUp()
        {
            collection = new PropertyCollection();
        }
        
        [Test]
        public void GetProperty_WhenCalledTwice_ReturnsSameProperty()
        {
            var propertyA = collection.GetProperty(StringADefinition);
            var propertyB = collection.GetProperty(StringADefinition);
            
            Assert.AreEqual(propertyA, propertyB);
        }

        [Test]
        public void GetProperty_WhenProvidedDifferentStringDefinitions_ReturnsDifferentProperties()
        {
            var propertyA = collection.GetProperty(StringADefinition);
            var propertyB = collection.GetProperty(StringBDefinition);
            
            Assert.AreNotEqual(propertyA, propertyB);
        }

        [Test]
        public void GetProperty_WhenProvidedDefaultAndPropertyDoesNotExist_ReturnsPropertyWithProvidedDefault()
        {
            const string defaultValue = "DefaultValue";
            var property = collection.GetProperty(new PropertyDefinition<string>(DefaultPropertyName, defaultValue));
            
            Assert.AreEqual(defaultValue, property.Value);
        }
        
        [Test]
        public void GetProperty_WhenProvidedDefaultAndPropertyDoesExist_ReturnsPropertyWithExistingValue()
        {
            const string existingDefaultValue = "Existing";
            const string newDefaultValue = "New";
            
            var propertyA = collection.GetProperty(new PropertyDefinition<string>(DefaultPropertyName, existingDefaultValue));
            var propertyB = collection.GetProperty(new PropertyDefinition<string>(DefaultPropertyName, newDefaultValue));
            
            Assert.AreEqual(propertyA.Value, existingDefaultValue);
            Assert.AreEqual(propertyB.Value, existingDefaultValue);
        }

        [Test]
        public void GetProperty_WhenCalledTwiceWithDifferentTypes_ThrowsException()
        {
            Assert.Throws<PropertyTypeMismatchException>(() =>
            {
                collection.GetProperty(new PropertyDefinition<string>(DefaultPropertyName));
                collection.GetProperty(new PropertyDefinition<int>(DefaultPropertyName));
            });
        }
    }
}