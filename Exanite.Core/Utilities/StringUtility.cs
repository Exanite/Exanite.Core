using System.Buffers;
using System.Collections.Immutable;

namespace Exanite.Core.Utilities;

public static class StringUtility
{
    public static readonly ImmutableArray<char> BidiCharacters = [
        '\u061C', // ALM - Arabic letter mark
        '\u200E', // LRM - Left-to-right mark
        '\u200F', // RLM - Right-to-left mark
        '\u202A', // LRE - Left-to-right embedding
        '\u202B', // RLE - Right-to-left embedding
        '\u202C', // PDF - Pop directional formatting
        '\u202D', // LRO - Left-to-right override
        '\u202E', // RLO - Right-to-left override
    ];

    public static readonly SearchValues<char> BidiCharacterSearch = SearchValues.Create(BidiCharacters.AsSpan());
}
