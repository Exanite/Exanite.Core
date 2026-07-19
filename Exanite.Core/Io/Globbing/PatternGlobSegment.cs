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
        var currentStates = new BitSet();
        var nextStates = new BitSet();

        currentStates[0] = true;
        if (!Nodes[0].IsEscaped && Nodes[0].Operator == '*')
        {
            currentStates[1] = true;
        }

        foreach (var c in name)
        {
            foreach (var currentState in currentStates)
            {
                if (currentState >= Nodes.Count)
                {
                    return true;
                }

                var node = Nodes[currentState];
                if (!IsMatch(node, c))
                {
                    continue;
                }

                if (node is { IsEscaped: false, Operator: '*' })
                {
                    nextStates[currentState] = true;
                }

                nextStates[currentState + 1] = true;
            }

            // Swap
            (currentStates, nextStates) = (nextStates, currentStates);
            nextStates.Clear();
        }

        return currentStates[Nodes.Count];
    }

    private bool IsMatch(Node node, char c)
    {
        if (!node.IsEscaped)
        {
            if (node.Operator == '?')
            {
                return true;
            }

            if (node.Operator == '*')
            {
                return true;
            }
        }

        if (node.Operator == c)
        {
            return true;
        }

        return false;
    }

    private record struct Node(char Operator, bool IsEscaped);
}
