using Exanite.Core.Events;
using NUnit.Framework;
using Exanite.Core.Properties;

namespace Exanite.Core.Tests.Editor.Properties
{
    [TestFixture]
    public class PropertyTests
    {
        private const string DefaultPropertyName = "Default";
        
        [Test]
        public void ValueChanged_OnValueChanged_IsRaisedWithCorrectValues()
        {
            const string previousValue = "Previous";
            const string newValue = "New";
            
            var property = new Property<string>(DefaultPropertyName, previousValue);
            var valueChangedRecorder = new EventRaisedRecorder();
            property.ValueChanged += (sender, args) =>
            {
                valueChangedRecorder.OnEventRaised();

                Assert.AreEqual(args.Property, property);
                Assert.AreEqual(args.PreviousValue, previousValue);
                Assert.AreEqual(args.NewValue, newValue);
            };
            
            property.Value = newValue;
            
            Assert.IsTrue(valueChangedRecorder.IsRaised);
        }
        
        [Test]
        public void UntypedValueChanged_OnValueChanged_IsRaisedWithCorrectValues()
        {
            const string previousValue = "Previous";
            const string newValue = "New";
            
            var property = new Property<string>(DefaultPropertyName, previousValue);
            var valueChangedRecorder = new EventRaisedRecorder();
            property.UntypedValueChanged += (sender, args) =>
            {
                valueChangedRecorder.OnEventRaised();

                Assert.AreEqual(args.Property, property);
                Assert.AreEqual(args.PreviousValue, previousValue);
                Assert.AreEqual(args.NewValue, newValue);
            };
            
            property.Value = newValue;
            
            Assert.IsTrue(valueChangedRecorder.IsRaised);
        }
    }
}