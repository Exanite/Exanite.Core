using Exanite.Core.Properties;
using NUnit.Framework;
#if !UNITY_2021_3_OR_NEWER
using Assert = NUnit.Framework.Legacy.ClassicAssert;
#endif

namespace Exanite.Core.Tests.Properties
{
    [TestFixture]
    public class PropertyTests
    {
        private const string DefaultPropertyName = "Default";

        [Test]
        public void UntypedValue_IsEqualToValue()
        {
            var property = new Property<string>(DefaultPropertyName);
            property.Value = "A";

            Assert.That(property.Value, Is.EqualTo(property.UntypedValue));

            property.Value = "B";

            Assert.That(property.Value, Is.EqualTo(property.UntypedValue));
        }

        [Test]
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

                Assert.That(args.Property, Is.EqualTo(property));
                Assert.That(args.PreviousValue, Is.EqualTo(previousValue));
                Assert.That(args.NewValue, Is.EqualTo(newValue));
            };

            property.Value = newValue;

            Assert.That(wasEventRaised, Is.True);
        }

        [Test]
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

                Assert.That(args.Property, Is.EqualTo(property));
                Assert.That(args.PreviousValue, Is.EqualTo(previousValue));
                Assert.That(args.NewValue, Is.EqualTo(newValue));
            };

            property.Value = newValue;

            Assert.That(wasEventRaised, Is.True);
        }
    }
}
