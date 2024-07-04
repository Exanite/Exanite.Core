#if UNITY_2021_3_OR_NEWER
using Exanite.Core.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace Exanite.Core.Tests.Utilities
{
    public partial class MathUtilityTests
    {
        [Test]
        public void InverseSwizzleVector3_ReversesSwizzleVector3([Values] Vector3Swizzle swizzle)
        {
            var original = new Vector3(1, 2, 3);
            Vector3 result;

            result = original.Swizzle(swizzle).InverseSwizzle(swizzle);
            Assert.AreEqual(original, result);
        }

        [TestCase]
        public void CreateUnityPlane_ReturnsExpectedResult1()
        {
            var plane = new Plane(Vector3.right, Vector3.zero);

            Assert.IsTrue(MathUtility.IsApproximatelyEqual(0, plane.distance));
            Assert.IsTrue(MathUtility.IsApproximatelyEqual(Vector3.right, plane.normal));
        }

        [TestCase]
        public void CreateUnityPlane_ReturnsExpectedResult2()
        {
            var plane = new Plane(Vector3.right, Vector3.right);

            Assert.IsTrue(MathUtility.IsApproximatelyEqual(-1, plane.distance));
            Assert.IsTrue(MathUtility.IsApproximatelyEqual(Vector3.right, plane.normal));
        }
    }
}
#endif
