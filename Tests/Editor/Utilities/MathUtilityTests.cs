using Exanite.Core.Utilities;
using NUnit.Framework;

namespace Exanite.Core.Tests.Editor.Utilities
{
    [TestFixture]
    public class MathUtilityTests
    {
        [TestCase(5, 0, 10, 0, 100, ExpectedResult = 50)]
        public float RemapFloat_ReturnsExpectedValue(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return MathUtility.Remap(value, fromMin, fromMax, toMin, toMax);
        }

        [TestCase(10, 0, 3, ExpectedResult = 1)]
        [TestCase(2, 0, 3, ExpectedResult = 2)]
        public float WrapFloat_ReturnsExpectedValue(float value, float min, float max)
        {
            return MathUtility.Wrap(value, min, max);
        }

        [TestCase(5, 2, ExpectedResult = 1)]
        [TestCase(6, 2, ExpectedResult = 0)]
        [TestCase(-5, 2, ExpectedResult = 1)]
        public float ModuloFloat_ReturnsExpectedValue(float value, float divisor)
        {
            return MathUtility.Modulo(value, divisor);
        }

        [TestCase(45, 11, ExpectedResult = 44)]
        public int GetNearestMultiple_ReturnsExpectedValue(int value, int multiple)
        {
            return MathUtility.GetNearestMultiple(value, multiple);
        }

        [TestCase(0, ExpectedResult = 2)]
        [TestCase(2, ExpectedResult = 2)]
        [TestCase(5, ExpectedResult = 8)]
        [TestCase(16, ExpectedResult = 16)]
        [TestCase(123, ExpectedResult = 128)]
        public int GetNextPowerOfTwo_ReturnsExpectedValue(int value)
        {
            return MathUtility.GetNextPowerOfTwo(value);
        }
    }
}
