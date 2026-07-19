using System.Collections.Generic;
using Exanite.Core.Collections;

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

    private readonly List<Node> Nodes = new();

    /// <remarks>
    /// This expects a pre-optimized pattern.
    /// See <see cref="GlobPattern"/>'s parsing code.
    /// </remarks>
    internal PatternGlobSegment(string pattern)
    {
        Pattern = pattern;

        var isEscape = false;
        for (var i = 0; i < pattern.Length; i++)
        {
            if (isEscape)
            {
                Nodes.Add(new Node(pattern[i], isEscape));
                isEscape = false;
                continue;
            }

            if (Pattern[i] == '\\')
            {
                isEscape = true;
            }

            Nodes.Add(new Node(pattern[i], isEscape));
        }
    }

    public bool IsMatch(string name)
    {
        var maxStateCount = Nodes.Count + 1;
        var currentStates = new BitSet();
        var nextStates = new BitSet();

        // var activeStatesList = new List<ActiveState>()
        // {
        //     new(-1, default, false),
        // };
        //
        // for (var nameI = 0; nameI < name.Length; nameI++)
        // {
        //     var c = name[nameI];
        //     var isLast = nameI == name.Length - 1;
        //
        //     var activeStates = activeStatesList.AsSpan();
        //     for (var stateI = 0; stateI < activeStates.Length; stateI++)
        //     {
        //         ref var state = ref activeStates[stateI];
        //
        //         // Initialize state if necessary
        //         if (state.PatternIndex == -1 && !TryAdvanceState(ref state))
        //         {
        //             return isLast;
        //         }
        //
        //         if (!IsMatch(ref state, c))
        //         {
        //             state.IsAlive = false;
        //             continue;
        //         }
        //
        //         // TODO: I need to figure out a better way to check free moves. This is wrong.
        //         if (!state.IsEscaped && state.Operator == '*')
        //         {
        //             activeStatesList.Add(state);
        //         }
        //
        //         if (!TryAdvanceState(ref state))
        //         {
        //             return isLast;
        //         }
        //     }
        //
        //     activeStatesList.RemoveAll(static state => !state.IsAlive);
        // }
        //
        // // Check for free moves
        // foreach (var activeState in activeStatesList)
        // {
        //     if (activeState.PatternIndex == Pattern.Length - 1 && activeState is { IsEscaped: false, Operator: '*' })
        //     {
        //         return true;
        //     }
        // }

        return false;
    }

    // private bool IsMatch(ref ActiveState state, char c)
    // {
    //     if (!state.IsEscaped)
    //     {
    //         if (state.Operator == '?')
    //         {
    //             return true;
    //         }
    //
    //         if (state.Operator == '*')
    //         {
    //             return true;
    //         }
    //     }
    //
    //     if (state.Operator == c)
    //     {
    //         return true;
    //     }
    //
    //     return false;
    // }

    private record struct Node(char Operator, bool IsEscaped);
}
