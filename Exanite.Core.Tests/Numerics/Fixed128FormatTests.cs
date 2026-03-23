using System;
using System.Globalization;
using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class Fixed128FormatTests
{
    public static TheoryData<Fixed128, string?, string> ToString_ReturnsCorrectValue_Data()
    {
        // Based on https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        return
        [
            // Supported:
            // G, F, N, R
            //
            // Lowercase variants should also work
            //
            // Not supported:
            // C, D, E, P, X

            // Default formatting (G)
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Zero, null, "0"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.One, null, "1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.NegativeOne, null, "-1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Half, null, "0.5"),

            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Zero, "G", "0"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.One, "G", "1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.NegativeOne, "G", "-1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Half, "G", "0.5"),

            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Zero, "g", "0"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.One, "g", "1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.NegativeOne, "g", "-1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Half, "g", "0.5"),

            // Fixed-point (F)
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(123, 456, 3), "F2", "123.46"), // Round for display
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(0, 5, 1), "F0", "0"), // Round to even
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(1, 5, 1), "F0", "2"), // Round to even
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(0, 501, 3), "F0", "1"), // Round to even - Ensure all remaining digits are checked
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(999, 999, 3), "F0", "1000"), // Carry rounded values

            new TheoryDataRow<Fixed128, string?, string>(Fixed128.One, "F64", "1.0000000000000000000000000000000000000000000000000000000000000000"), // Pad zeroes

            // Numeric (N)
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.One * 1000, "N", "1,000.00"),

            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(1, 25, 2), "N0", "1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(1, 25, 2), "N1", "1.2"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(1, 25, 2), "N2", "1.25"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(1, 25, 2), "N3", "1.250"),

            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(9999, 99, 2), "N0", "10,000"),

            // Precision testing (Epsilon)
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Epsilon, "G", "0.00000000023283064365386962890625"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Epsilon, "R", "0.00000000023283064365386962890625"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Epsilon, "F31", "0.0000000002328306436538696289062"),

            // Edge cases
            new TheoryDataRow<Fixed128, string?, string>(Fixed.MinValue, "G", "-140737488355327.9999847412109375"),
        ];
    }

    [Theory]
    [MemberData(nameof(ToString_ReturnsCorrectValue_Data))]
    public void ToString_ReturnsCorrectValue_InvariantCulture(Fixed128 value, string? format, string expected)
    {
        var result = value.ToString(format, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }

    public static TheoryData<Fixed128, string, string> ToString_RespectsFormatProvider_Data()
    {
        var arSaFormatInfo = NumberFormatInfo.GetInstance(new CultureInfo("ar-SA"));
        var arSaExpected = $"{arSaFormatInfo.NegativeSign}1\u066C234\u066B5"; // NumberNegativePattern=1, so the negative sign goes in front

        return
        [
            new TheoryDataRow<Fixed128, string, string>(Fixed128.FromDecimal(1234, 5, 1), "en-US", "1,234.5"),
            new TheoryDataRow<Fixed128, string, string>(Fixed128.FromDecimal(1234, 5, 1), "de-DE", "1.234,5"),
            new TheoryDataRow<Fixed128, string, string>(Fixed128.FromDecimal(1234, 5, 1), "en-US", "1,234.5"),
            new TheoryDataRow<Fixed128, string, string>(Fixed128.FromDecimal(1234, 5, 1), "fa-IR", "1٬234٫5"),
            new TheoryDataRow<Fixed128, string, string>(Fixed128.FromDecimal(-1234, 5, 1), "ar-SA", arSaExpected),
        ];
    }

    [Theory]
    [MemberData(nameof(ToString_RespectsFormatProvider_Data))]
    public void ToString_RespectsFormatProvider(Fixed128 value, string cultureName, string expected)
    {
        var culture = new CultureInfo(cultureName);
        var result = value.ToString("N1", culture);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToString_UsesGeneralFormat_ByDefault()
    {
        var value = Fixed128.FromDecimal(10, 75, 2);
        Assert.Equal("10.75", value.ToString(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void TryFormat_ReturnsTrue_IfBufferIsLargeEnough()
    {
        var value = Fixed128.One; // "1"
        Span<char> destination = stackalloc char[1];

        var isSuccess = value.TryFormat(destination, out var charsWritten, "G", CultureInfo.InvariantCulture);

        Assert.True(isSuccess);
        Assert.Equal("1", destination[..charsWritten].ToString());
    }

    [Fact]
    public void TryFormat_ReturnsFalse_IfBufferIsTooSmall()
    {
        var value = Fixed128.FromDecimal(123, 45, 2); // "123.45"
        Span<char> destination = stackalloc char[2];

        var isSuccess = value.TryFormat(destination, out var charsWritten, "G", CultureInfo.InvariantCulture);

        Assert.False(isSuccess);
        Assert.Equal(0, charsWritten);
    }

    [Fact]
    public void TryFormat_Parse_CanRoundtrip()
    {
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            var input = (Fixed128)current;
            AssertEqualRoundtrip(i, input, Fixed128.Parse(input.ToString("R", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }
    }

    private void AssertEqualRoundtrip(int i, Fixed128 input, Fixed128 expected)
    {
        Assert.True(expected == input, $"""
            Failed at i: {i}
            Input:       {input}
            Expected:    {expected}
            Actual:      {input}
            """);
    }
}
