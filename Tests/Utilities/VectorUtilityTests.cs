using System;
using System.Numerics;
using Exanite.Core.Numerics;
using NUnit.Framework;

namespace Exanite.Core.Tests.Utilities
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
