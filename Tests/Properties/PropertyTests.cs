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

            Assert.AreEqual(property.UntypedValue, property.Value);

            property.Value = "B";

            Assert.AreEqual(property.UntypedValue, property.Value);
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

                Assert.AreEqual(property, args.Property);
                Assert.AreEqual(previousValue, args.PreviousValue);
                Assert.AreEqual(newValue, args.NewValue);
            };

            property.Value = newValue;

            Assert.IsTrue(wasEventRaised);
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

                Assert.AreEqual(property, args.Property);
                Assert.AreEqual(previousValue, args.PreviousValue);
                Assert.AreEqual(newValue, args.NewValue);
            };

            property.Value = newValue;

            Assert.IsTrue(wasEventRaised);
        }
    }
}
