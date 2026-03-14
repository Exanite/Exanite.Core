using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Exanite.Core.Numerics;

public partial struct Fixed128
{
    public const NumberStyles SupportedNumberStyles = 0
        | NumberStyles.AllowLeadingWhite
        | NumberStyles.AllowTrailingWhite
        | NumberStyles.AllowLeadingSign
        | NumberStyles.AllowTrailingSign
        | NumberStyles.AllowParentheses
        | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowThousands
        | NumberStyles.AllowHexSpecifier
        | NumberStyles.AllowBinarySpecifier
        // Explicitly specify that these are not supported
        & ~(NumberStyles.AllowExponent | NumberStyles.AllowCurrencySymbol);

    private static readonly ImmutableArray<char> BidiCharacters = [
        '\u061C', // ALM - Arabic letter mark
        '\u200E', // LRM - Left-to-right mark
        '\u200F', // RLM - Right-to-left mark
        '\u202A', // LRE - Left-to-right embedding
        '\u202B', // RLE - Right-to-left embedding
        '\u202C', // PDF - Pop directional formatting
        '\u202D', // LRO - Left-to-right override
        '\u202E', // RLO - Right-to-left override
    ];

    public static Fixed128 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, SupportedNumberStyles, provider);
    public static Fixed128 Parse(string s, IFormatProvider? provider) => Parse((ReadOnlySpan<char>)s, provider);
    public static Fixed128 Parse(string s, NumberStyles style, IFormatProvider? provider) => Parse((ReadOnlySpan<char>)s, style, provider);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Fixed128 result) => TryParse(s, SupportedNumberStyles, provider, out result);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Fixed128 result) => TryParse((ReadOnlySpan<char>)s, provider, out result);
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Fixed128 result) => TryParse((ReadOnlySpan<char>)s, style, provider, out result);

    public static Fixed128 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        if (!TryParse(s, style, provider, out var result))
        {
            throw new FormatException($"The input string is in an invalid format: {s}");
        }

        return result;
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Fixed128 result)
    {
        if ((style & ~SupportedNumberStyles) != 0)
        {
            result = default;
            return false;
        }

        // If hex or binary is requested, then just parse it as an Int128 and shift up
        if ((style & (NumberStyles.AllowHexSpecifier | NumberStyles.AllowBinarySpecifier)) != 0)
        {
            if (Int128.TryParse(s, style, provider, out var value))
            {
                if (value > (Int128)MaxValue || value < (Int128)MinValue)
                {
                    result = default;
                    return false;
                }

                result = new Fixed128(value << Shift);
                return true;
            }
        }

        // Trim Bidi characters
        s = s.Trim(BidiCharacters.AsSpan());

        // Handle whitespace
        if ((style & NumberStyles.AllowLeadingWhite) != 0)
        {
            s = s.TrimStart();
        }

        if ((style & NumberStyles.AllowTrailingWhite) != 0)
        {
            s = s.TrimEnd();
        }

        if (s.IsEmpty)
        {
            result = default;
            return false;
        }

        // Get format info
        var formatInfo = NumberFormatInfo.GetInstance(provider);

        // Handle signs
        var isNegative = false;
        if ((style & NumberStyles.AllowParentheses) != 0 && s.StartsWith("(") && s.EndsWith(")"))
        {
            isNegative = true;
            s = s[1..^1];
        }
        else
        {
            var explicitLeadingSignHandled = false;
            var negativeSign = formatInfo.NegativeSign.Trim(BidiCharacters.AsSpan());
            var positiveSign = formatInfo.PositiveSign.Trim(BidiCharacters.AsSpan());

            if ((style & NumberStyles.AllowLeadingSign) != 0)
            {
                if (s.StartsWith(negativeSign))
                {
                    isNegative = true;
                    s = s[negativeSign.Length..];

                    if (negativeSign.Length > 0)
                    {
                        explicitLeadingSignHandled = true;
                    }
                }
                else if (s.StartsWith(positiveSign))
                {
                    s = s[positiveSign.Length..];

                    if (positiveSign.Length > 0)
                    {
                        explicitLeadingSignHandled = true;
                    }
                }
            }

            if (!explicitLeadingSignHandled && (style & NumberStyles.AllowTrailingSign) != 0)
            {
                if (s.EndsWith(negativeSign))
                {
                    isNegative = true;
                    s = s[..^negativeSign.Length];
                }
                else if (s.EndsWith(positiveSign))
                {
                    s = s[..^positiveSign.Length];
                }
            }
        }

        // Split into integral and fractional parts
        ReadOnlySpan<char> integralText;
        ReadOnlySpan<char> fractionalText;

        var decimalSeparator = formatInfo.NumberDecimalSeparator;
        var decimalIndex = s.IndexOf(decimalSeparator);
        if (decimalIndex >= 0)
        {
            if ((style & NumberStyles.AllowDecimalPoint) == 0)
            {
                result = default;
                return false;
            }

            integralText = s[..decimalIndex];
            fractionalText = s[(decimalIndex + decimalSeparator.Length)..];
        }
        else
        {
            integralText = s;
            fractionalText = ReadOnlySpan<char>.Empty;
        }

        // Parse integral portion
        // Group separators can appear in the integral portion, but not the fractional portion
        var integralStyle = style & NumberStyles.AllowThousands;
        if (!Int128.TryParse(integralText, integralStyle, provider, out var integralValue))
        {
            result = default;
            return false;
        }

        var resultRaw = integralValue << Shift;

        // Parse fractional portion
        {
            Int128 numerator = 0;
            Int128 denominator = 1;
            var digitsProcessed = 0;

            const int maxDigits = 28; // log10(2^(127-Shift))
            foreach (var c in fractionalText)
            {
                if (!char.IsDigit(c))
                {
                    result = default;
                    return false;
                }

                if (digitsProcessed < maxDigits)
                {
                    var digit = c - '0';
                    numerator = (numerator * 10) + digit;
                    denominator *= 10;
                    digitsProcessed++;
                }
            }

            if (numerator > 0)
            {
                var scaledNumerator = numerator * OneRaw;
                var fraction = scaledNumerator / denominator;
                var remainder = scaledNumerator % denominator;

                // Banker's rounding
                var twiceRemainder = remainder * 2;
                if (twiceRemainder > denominator)
                {
                    // Remainder is greater than 0.5
                    fraction++;
                }
                else if (twiceRemainder == denominator && (fraction & 1) != 0)
                {
                    // Remainder is exactly 0.5
                    // Only add if fraction is odd
                    fraction++;
                }

                resultRaw += fraction;
            }
        }

        // Apply negative sign
        if (isNegative)
        {
            resultRaw = -resultRaw;
        }

        result = new Fixed128(resultRaw);
        return true;
    }
}
