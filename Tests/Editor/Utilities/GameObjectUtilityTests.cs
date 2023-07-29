using Exanite.Core.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace Exanite.Core.Tests.Editor.Utilities
{
    [TestFixture]
    public class GameObjectUtilityTests
    {
        public GameObject GameObject { get; set; }

        [SetUp]
        public void Setup()
        {
            GameObject = new GameObject();
        }

        [TearDown]
        public void TearDown()
        {
            UnityUtility.UnsafeDestroy(GameObject);
        }

        [Test]
        public void GetRequiredComponent_ComponentExists_ReturnsComponent()
        {
            GameObject.AddComponent<TestComponent>();

            var component = GameObject.GetRequiredComponent<TestComponent>();

            Assert.NotNull(component);
        }

        [Test]
        public void GetRequiredComponent_UsingInterfaceAndComponentExists_ReturnsComponent()
        {
            GameObject.AddComponent<TestComponent>();

            var component = GameObject.GetRequiredComponent<ITestComponent>();

            Assert.NotNull(component);
        }

        [Test]
        public void GetRequiredComponent_ComponentDoesNotExist_ThrowsException()
        {
            TestDelegate action = () => GameObject.GetRequiredComponent<TestComponent>();

            Assert.Throws<MissingComponentException>(action);
        }

        [Test]
        public void GetOrAddComponent_ComponentExists_ReturnsComponent()
        {
            GameObject.AddComponent<TestComponent>();

            var component = GameObject.GetOrAddComponent<TestComponent>();

            Assert.NotNull(component);
        }

        [Test]
        public void GetOrAddComponent_ComponentDoesNotExist_AddsComponent()
        {
            var component = GameObject.GetOrAddComponent<TestComponent>();

            Assert.NotNull(component);
        }

        public interface ITestComponent { }

        public class TestComponent : MonoBehaviour, ITestComponent { }
    }
}
