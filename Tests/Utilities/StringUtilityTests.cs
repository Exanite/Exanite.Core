using Exanite.Core.Utilities;
using NUnit.Framework;

#if !UNITY_2021_3_OR_NEWER
using Assert = NUnit.Framework.Legacy.ClassicAssert;
#endif

namespace Exanite.Core.Tests.Utilities
{
    [TestFixture]
    public class StringUtilityTests
    {
        [Test]
        [TestCase("\r\nabc\nabcdefg\n\r", "\nabc\nabcdefg\n\r", "\n")]
        [TestCase("\r\nabc\nabcdefg\n\r", "\r\nabc\r\nabcdefg\r\n\r", "\r\n")]
        [TestCase("", "", "\r\n")]
        [TestCase("\r", "\r", "\r\n")]
        [TestCase("\r\n", "\r\n", "\r\n")]
        [TestCase("abc", "abc", "\r\n")]
        public void UpdateNewLines_ReturnsCorrectResult(string input, string expected, string newLine)
        {
            var output = StringUtility.UpdateNewLines(input, newLine);
            Assert.AreEqual(expected, output);
        }
    }
}
