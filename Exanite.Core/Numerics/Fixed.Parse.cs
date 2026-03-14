using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Exanite.Core.Numerics;

public partial struct Fixed
{
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Fixed result)
    {
        if (Fixed128.TryParse(s, style, provider, out var value))
        {
            if (value < MinValue || value > MaxValue)
            {
                result = default;
                return false;
            }

            result = (Fixed)value;
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Fixed result) => TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Fixed result) => TryParse((ReadOnlySpan<char>)s, provider, out result);
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Fixed result) => TryParse((ReadOnlySpan<char>)s, style, provider, out result);

    public static Fixed Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        if (!TryParse(s, style, provider, out var result))
        {
            throw new FormatException($"The input string is in an invalid format: {s}");
        }

        return result;
    }

    public static Fixed Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider);
    public static Fixed Parse(string s, IFormatProvider? provider) => Parse((ReadOnlySpan<char>)s, provider);
    public static Fixed Parse(string s, NumberStyles style, IFormatProvider? provider) => Parse((ReadOnlySpan<char>)s, style, provider);
}
