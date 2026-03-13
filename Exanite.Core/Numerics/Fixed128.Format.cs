using System;
using System.Globalization;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed128
{
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var formatSpan = format.AsSpan();
        var precisionSpan = formatSpan.Length > 1 ? formatSpan[1..] : [];

        var precision = 0;
        if (precisionSpan.Length != 0)
        {
            if (!int.TryParse(precisionSpan, CultureInfo.InvariantCulture, out var requestedPrecision))
            {
                throw new FormatException($"The requested format is not supported: {formatSpan}");
            }

            precision = requestedPrecision;
        }

        var result = precision <= FractionalBitCount ? stackalloc char[64] : new char[64 + precision];
        var isSuccess = TryFormat(result, out var written, format, formatProvider);
        AssertUtility.IsTrue(isSuccess, "Internal: Failed to format Fixed128 as string");

        return result[..written].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return ((decimal)Raw / OneRaw).TryFormat(destination, out charsWritten, format, provider);
    }

    public override string ToString() => ToString(null, null);
}
