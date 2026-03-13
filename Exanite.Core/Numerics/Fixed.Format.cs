using System;

namespace Exanite.Core.Numerics;

public partial struct Fixed
{
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return ((Fixed128)this).ToString(format, formatProvider);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return ((Fixed128)this).TryFormat(destination, out charsWritten, format, provider);
    }

    public override string ToString() => ToString(null, null);
}
