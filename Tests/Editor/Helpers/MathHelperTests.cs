using Exanite.Core.Helpers;
using NUnit.Framework;

namespace Exanite.Core.Tests.Editor.Helpers
{
    public class MathHelperTests
    {
        [TestCase(5, 0, 10, 0, 100, ExpectedResult = 50)]
        public float RemapFloat_ReturnsExpectedResult(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return MathHelper.Remap(value, fromMin, fromMax, toMin, toMax);
        }

        [TestCase(10, 0, 3, ExpectedResult = 1)]
        [TestCase(2, 0, 3, ExpectedResult = 2)]
        public float WrapFloat_ReturnsExpectedResult(float value, float min, float max)
        {
            return MathHelper.Wrap(value, min, max);
        }

        [TestCase(5, 2, ExpectedResult = 1)]
        [TestCase(6, 2, ExpectedResult = 0)]
        [TestCase(-5, 2, ExpectedResult = 1)]
        public float ModuloFloat_ReturnsExpectedResult(float value, float divisor)
        {
            return MathHelper.Modulo(value, divisor);
        }

        [TestCase(5, 0, 10, ExpectedResult = 5)]
        [TestCase(-1, 0, 10, ExpectedResult = 0)]
        [TestCase(15, 0, 10, ExpectedResult = 10)]
        [TestCase(50, 10, 0, ExpectedResult = 0)] // min and max are swapped
        [TestCase(-10, 10, 0, ExpectedResult = 10)] // min and max are swapped
        public float ClampFloat_ReturnsExpectedResult(float value, float min, float max)
        {
            return MathHelper.Clamp(value, min, max);
        }

        [TestCase(45, 11, ExpectedResult = 44)]
        public int GetNearestMultiple_ReturnsExpectedResult(int value, int multiple)
        {
            return MathHelper.GetNearestMultiple(value, multiple);
        }
    }
}
