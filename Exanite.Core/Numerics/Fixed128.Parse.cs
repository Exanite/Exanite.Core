using System;
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
            var explicitSignHandled = false;
            if ((style & NumberStyles.AllowLeadingSign) != 0)
            {
                if (s.StartsWith(formatInfo.NegativeSign))
                {
                    isNegative = true;
                    s = s[formatInfo.NegativeSign.Length..];

                    if (formatInfo.NegativeSign.Length > 0)
                    {
                        explicitSignHandled = true;
                    }
                }
                else if (s.StartsWith(formatInfo.PositiveSign))
                {
                    s = s[formatInfo.PositiveSign.Length..];

                    if (formatInfo.PositiveSign.Length > 0)
                    {
                        explicitSignHandled = true;
                    }
                }
            }

            if (!explicitSignHandled && (style & NumberStyles.AllowTrailingSign) != 0)
            {
                if (s.EndsWith(formatInfo.NegativeSign))
                {
                    isNegative = true;
                    s = s[..formatInfo.NegativeSign.Length];
                }
                else if (s.EndsWith(formatInfo.PositiveSign))
                {
                    s = s[..formatInfo.PositiveSign.Length];
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
