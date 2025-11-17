using Exanite.Core.Properties;
using Xunit;

namespace Exanite.Core.Tests.Properties;

public class PropertyTests
{
    private const string DefaultPropertyName = "Default";

    [Fact]
    public void UntypedValue_IsEqualToValue()
    {
        var property = new Property<string>(DefaultPropertyName);
        property.Value = "A";

        Assert.Equal(property.UntypedValue, property.Value);

        property.Value = "B";

        Assert.Equal(property.UntypedValue, property.Value);
    }

    [Fact]
    public void ValueChanged_OnValueChanged_IsRaisedWithCorrectValues()
    {
        const string previousValue = "Previous";
        const string newValue = "New";

        var property = new Property<string>(DefaultPropertyName);
        property.Value = previousValue;

        var wasEventRaised = false;
        property.ValueChanged += (sender, args) =>
        {
            wasEventRaised = true;

            Assert.Equal(property, args.Property);
            Assert.Equal(previousValue, args.PreviousValue);
            Assert.Equal(newValue, args.NewValue);
        };

        property.Value = newValue;

        Assert.True(wasEventRaised);
    }

    [Fact]
    public void UntypedValueChanged_OnValueChanged_IsRaisedWithCorrectValues()
    {
        const string previousValue = "Previous";
        const string newValue = "New";

        var property = new Property<string>(DefaultPropertyName);
        property.Value = previousValue;

        var wasEventRaised = false;
        property.UntypedValueChanged += (sender, args) =>
        {
            wasEventRaised = true;

            Assert.Equal(property, args.Property);
            Assert.Equal(previousValue, args.PreviousValue);
            Assert.Equal(newValue, args.NewValue);
        };

        property.Value = newValue;

        Assert.True(wasEventRaised);
    }
}
