using Exanite.Core.Properties;
using Xunit;

namespace Exanite.Core.Tests.Properties;

public class PropertyCollectionTests
{
    private const string DefaultPropertyName = "Default";

    private static readonly PropertyDefinition<string> StringADefinition = new("StringA");
    private static readonly PropertyDefinition<string> StringBDefinition = new("StringB");

    private readonly PropertyCollection collection = new();

    [Fact]
    public void GetProperty_WhenPropertyDoesNotExist_ReturnsNull()
    {
        var property = collection.GetProperty(StringADefinition);

        Assert.Null(property);
    }

    [Fact]
    public void GetPropertyValue_WhenPropertyDoesNotExist_ThrowsException()
    {
        Assert.Throws<PropertyNotFoundException>(() =>
        {
            collection.GetPropertyValue(StringADefinition);
        });
    }

    [Fact]
    public void SetPropertyValue_WhenPropertyDoesNotExist_ThrowsException()
    {
        Assert.Throws<PropertyNotFoundException>(() =>
        {
            collection.SetPropertyValue(StringADefinition, "");
        });
    }

    [Fact]
    public void GetOrAddProperty_WhenCalledTwice_ReturnsSameProperty()
    {
        var propertyA = collection.GetOrAddProperty(StringADefinition);
        var propertyB = collection.GetOrAddProperty(StringADefinition);

        Assert.Equal(propertyA, propertyB);
    }

    [Fact]
    public void GetOrAddProperty_WhenProvidedDifferentStringDefinitions_ReturnsDifferentProperties()
    {
        var propertyA = collection.GetOrAddProperty(StringADefinition);
        var propertyB = collection.GetOrAddProperty(StringBDefinition);

        Assert.NotEqual(propertyA, propertyB);
    }

    [Fact]
    public void GetOrAddProperty_WhenCalledTwiceWithDifferentTypes_ThrowsException()
    {
        Assert.Throws<PropertyTypeMismatchException>(() =>
        {
            collection.GetOrAddProperty(new PropertyDefinition<string>(DefaultPropertyName));
            collection.GetOrAddProperty(new PropertyDefinition<int>(DefaultPropertyName));
        });
    }
}
