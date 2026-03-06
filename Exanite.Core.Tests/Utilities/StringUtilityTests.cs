using Exanite.Core.Utilities;
using Xunit;

namespace Exanite.Core.Tests.Utilities;

public class StringUtilityTests
{
    [Theory]
    [InlineData("\r\nabc\nabcdefg\n\r", "\nabc\nabcdefg\n\r", "\n")]
    [InlineData("\r\nabc\nabcdefg\n\r", "\r\nabc\r\nabcdefg\r\n\r", "\r\n")]
    [InlineData("", "", "\r\n")]
    [InlineData("\r", "\r", "\r\n")]
    [InlineData("\r\n", "\r\n", "\r\n")]
    [InlineData("abc", "abc", "\r\n")]
    public void UpdateNewLines_ReturnsCorrectResult(string input, string expected, string newLine)
    {
        var output = StringUtility.UpdateNewLines(input, newLine);
        Assert.Equal(expected, output);
    }
}
