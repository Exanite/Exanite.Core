using System.Collections.Generic;
using Exanite.Core.Utilities;

namespace Exanite.Core.Io.Globbing;

/// <summary>
/// Matches a file or folder name by pattern.
/// Supports <c>?</c> for matching exactly one character,
/// <c>*</c> for matching zero or more characters,
/// and <c>\</c> for escape characters.
/// </summary>
public sealed class PatternGlobSegment : GlobSegment
{
    public readonly string Pattern;

    internal PatternGlobSegment(string pattern)
    {
        Pattern = pattern;
    }

    public bool IsMatch(string name)
    {
        var activeStates = new List<ActiveState>()
        {
            new(-1, default, false),
        };

        for (var nameI = 0; nameI < name.Length; nameI++)
        {
            var c = name[nameI];
            var isLast = nameI == name.Length - 1;

            var span = activeStates.AsSpan();
            for (var stateI = 0; stateI < span.Length; stateI++)
            {
                ref var state = ref span[stateI];

                // Initialize state if necessary
                if (state.PatternIndex == -1 && !TryAdvanceState(ref state))
                {
                    return isLast;
                }

                if (!state.IsEscaped)
                {
                    if (state.ActiveSelector == '?')
                    {
                        if (!TryAdvanceState(ref state))
                        {
                            return isLast;
                        }

                        continue;
                    }

                    if (state.ActiveSelector == '*')
                    {
                        // TODO
                        if (!TryAdvanceState(ref state))
                        {
                            return isLast;
                        }

                        continue;
                    }
                }

                if (state.ActiveSelector == c)
                {
                    if (!TryAdvanceState(ref state))
                    {
                        return isLast;
                    }

                    continue;
                }

                state.IsAlive = false;
            }

            activeStates.RemoveAll(static state => !state.IsAlive);
        }

        return false;
    }

    private bool TryAdvanceState(ref ActiveState state)
    {
        var next = state.PatternIndex + 1;
        if (next >= Pattern.Length)
        {
            return false;
        }

        var isEscape = Pattern[next] == '\\';
        if (!isEscape)
        {
            state = new ActiveState(next, Pattern[next], isEscape);
            return true;
        }

        next++;
        if (next >= Pattern.Length)
        {
            return false;
        }

        state = new ActiveState(next, Pattern[next], isEscape);
        return true;
    }

    private record struct ActiveState(int PatternIndex, char ActiveSelector, bool IsEscaped, bool IsAlive = true);
}
