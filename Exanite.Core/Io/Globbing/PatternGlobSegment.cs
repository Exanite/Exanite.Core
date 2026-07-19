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
        var activeStatesList = new List<ActiveState>()
        {
            new(-1, default, false),
        };

        for (var nameI = 0; nameI < name.Length; nameI++)
        {
            var c = name[nameI];
            var isLast = nameI == name.Length - 1;

            var activeStates = activeStatesList.AsSpan();
            for (var stateI = 0; stateI < activeStates.Length; stateI++)
            {
                ref var state = ref activeStates[stateI];

                // Initialize state if necessary
                if (state.PatternIndex == -1 && !TryAdvanceState(ref state))
                {
                    return isLast;
                }

                if (!IsMatch(ref state, c))
                {
                    state.IsAlive = false;
                    continue;
                }

                // TODO: I need to figure out a better way to check free moves. This is wrong.
                if (!state.IsEscaped && state.Operator == '*')
                {
                    activeStatesList.Add(state);
                }

                if (!TryAdvanceState(ref state))
                {
                    return isLast;
                }
            }

            activeStatesList.RemoveAll(static state => !state.IsAlive);
        }

        // Check for free moves
        foreach (var activeState in activeStatesList)
        {
            if (activeState.PatternIndex == Pattern.Length - 1 && activeState is { IsEscaped: false, Operator: '*' })
            {
                return true;
            }
        }

        return false;
    }

    private bool IsMatch(ref ActiveState state, char c)
    {
        if (!state.IsEscaped)
        {
            if (state.Operator == '?')
            {
                return true;
            }

            if (state.Operator == '*')
            {
                return true;
            }
        }

        if (state.Operator == c)
        {
            return true;
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

    private record struct ActiveState(int PatternIndex, char Operator, bool IsEscaped, bool IsAlive = true);
}
