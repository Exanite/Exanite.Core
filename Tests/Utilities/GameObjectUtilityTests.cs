﻿using NUnit.Framework;

namespace Exanite.Core.Tests.Utilities
{
    [TestFixture]
    public class GameObjectUtilityTests
    {
        private GameObject gameObject = null!;

        [SetUp]
        public void Setup()
        {
            gameObject = new GameObject();
        }

        [TearDown]
        public void TearDown()
        {
            UnityUtility.UnsafeDestroy(gameObject);
        }

        [Test]
        public void GetRequiredComponent_ComponentExists_ReturnsComponent()
        {
            gameObject.AddComponent<TestComponent>();

            var component = gameObject.GetRequiredComponent<TestComponent>();

            Assert.NotNull(component);
        }

        [Test]
        public void GetRequiredComponent_UsingInterfaceAndComponentExists_ReturnsComponent()
        {
            gameObject.AddComponent<TestComponent>();

            var component = gameObject.GetRequiredComponent<ITestComponent>();

            Assert.NotNull(component);
        }

        [Test]
        public void GetRequiredComponent_ComponentDoesNotExist_ThrowsException()
        {
            TestDelegate action = () => gameObject.GetRequiredComponent<TestComponent>();

            Assert.Throws<MissingComponentException>(action);
        }

        [Test]
        public void GetOrAddComponent_ComponentExists_ReturnsComponent()
        {
            gameObject.AddComponent<TestComponent>();

            var component = gameObject.GetOrAddComponent<TestComponent>();

            Assert.NotNull(component);
        }

        [Test]
        public void GetOrAddComponent_ComponentDoesNotExist_AddsComponent()
        {
            var component = gameObject.GetOrAddComponent<TestComponent>();

            Assert.NotNull(component);
        }

        public interface ITestComponent { }

        public class TestComponent : MonoBehaviour, ITestComponent { }
    }
}