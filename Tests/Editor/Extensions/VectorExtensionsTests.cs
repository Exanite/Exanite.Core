using Exanite.Core.Extensions;
using Exanite.Core.Numbers;
using NUnit.Framework;
using UnityEngine;

namespace Exanite.Core.Tests.Editor
{
    public class VectorExtensionsTests
    {
        [Test]
        public void InverseSwizzleVector3_ReversesSwizzleVector3([Values] Vector3Swizzle swizzle)
        {
            Vector3 original = new Vector3(Random.value, Random.value, Random.value);
            Vector3 result;

            result = original.Swizzle(swizzle).InverseSwizzle(swizzle);
            Assert.AreEqual(original, result);
        }
    }
}
