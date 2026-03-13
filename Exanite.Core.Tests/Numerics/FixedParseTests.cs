using System.Globalization;
using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedParseTests
{
    public static TheoryData<string, Fixed> TryParse_ReturnsCorrectValue_CommonInputs_Data()
    {
        return
        [
            new TheoryDataRow<string, Fixed>("0", 0),
            new TheoryDataRow<string, Fixed>("1", 1),
            new TheoryDataRow<string, Fixed>("-1", -1),
            new TheoryDataRow<string, Fixed>("0.5", Fixed.FromDecimal(0, 5, 1)),
            new TheoryDataRow<string, Fixed>("2", 2),
            new TheoryDataRow<string, Fixed>("0.0000152587890625", Fixed.Epsilon), // Input is 1.0 * Epsilon
            new TheoryDataRow<string, Fixed>("0.00000762939453125", Fixed.Epsilon), // Input is 0.5 * Epsilon
            new TheoryDataRow<string, Fixed>("0.00002288818359375", Fixed.Epsilon * 2), // Input is 1.5 * Epsilon
        ];
    }

    [Theory]
    [MemberData(nameof(TryParse_ReturnsCorrectValue_CommonInputs_Data))]
    public void TryParse_ReturnsCorrectValue_CommonInputs(string input, Fixed expected)
    {
        var isSuccess = Fixed.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var result);

        Assert.True(isSuccess);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TryParse_ParsesAsInteger_ForHexStyle()
    {
        // 0x10 is 16
        var input = "10";
        var expected = Fixed.One * 16;

        var isSuccess = Fixed.TryParse(input, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var result);

        Assert.True(isSuccess);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TryParse_ReturnsNegative_Parentheses_ForParentheses()
    {
        var input = "(1)";
        var style = NumberStyles.AllowParentheses;

        var isSuccess = Fixed.TryParse(input, style, CultureInfo.InvariantCulture, out var result);

        Assert.True(isSuccess);
        Assert.Equal(Fixed.NegativeOne, result);
    }

    public static TheoryData<string, string, bool, Fixed> TryParse_RespectsFormatProvider_Data()
    {
        return
        [
            new TheoryDataRow<string, string, bool, Fixed>("1,234.5", "en-US", true, Fixed.FromDecimal(1234, 5, 1)), // Comma is group separator
            new TheoryDataRow<string, string, bool, Fixed>("1.234,5", "de-DE", true, Fixed.FromDecimal(1234, 5, 1)), // Dot is group separator, comma is decimal
            new TheoryDataRow<string, string, bool, Fixed>("1,234.5", "de-DE", false, 0), // Wrong format for US
        ];
    }

    [Theory]
    [MemberData(nameof(TryParse_RespectsFormatProvider_Data))]
    public void TryParse_RespectsFormatProvider(string input, string cultureName, bool shouldPass, Fixed expected)
    {
        var culture = new CultureInfo(cultureName);
        var style = NumberStyles.Number;
        var isSuccess = Fixed.TryParse(input, style, culture, out var result);

        Assert.Equal(shouldPass, isSuccess);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("   123   ", NumberStyles.None, false)]
    [InlineData("   123   ", NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, true)]
    public void TryParse_RespectsWhitespaceFlags(string input, NumberStyles style, bool expectedSuccess)
    {
        var isSuccess = Fixed.TryParse(input, style, CultureInfo.InvariantCulture, out var result);
        Assert.Equal(expectedSuccess, isSuccess);
    }

    [Fact]
    public void TryParse_ReturnsFalse_OnOverflow()
    {
        // Value significantly larger than MaxValue
        var input = "1" + new string('0', 30);
        var isSuccess = Fixed.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result);

        Assert.False(isSuccess);
        Assert.Equal(Fixed.Zero, result);
    }

    [Theory]
    [InlineData("not-a-number")]
    [InlineData("1.2.3")]
    [InlineData("")]
    public void TryParse_ReturnsFalse_ForInvalidInput(string input)
    {
        var isSuccess = Fixed.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result);

        Assert.False(isSuccess);
        Assert.Equal(default, result);
    }
}
