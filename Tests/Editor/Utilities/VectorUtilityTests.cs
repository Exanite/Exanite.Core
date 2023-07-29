using Exanite.Core.Numbers;
using Exanite.Core.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace Exanite.Core.Tests.Editor.Utilities
{
    [TestFixture]
    public class VectorUtilityTests
    {
        [Test]
        public void InverseSwizzleVector3_ReversesSwizzleVector3([Values] Vector3Swizzle swizzle)
        {
            var original = new Vector3(Random.value, Random.value, Random.value);
            Vector3 result;

            result = original.Swizzle(swizzle).InverseSwizzle(swizzle);
            Assert.AreEqual(original, result);
        }
    }
}
