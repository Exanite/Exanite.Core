using System.Collections.Generic;
using Exanite.Core.Collections;
using Exanite.Core.Pooling;

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

    private readonly List<Node> nodes = new();

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
                nodes.Add(new Node(NodeType.Literal, pattern[i]));
                isEscape = false;
                continue;
            }

            if (Pattern[i] == '\\')
            {
                isEscape = true;
                continue;
            }

            var type = pattern[i] switch
            {
                '*' => NodeType.WildcardStar,
                '?' => NodeType.WildcardQuestion,
                _ => NodeType.Literal,
            };

            nodes.Add(new Node(type, pattern[i]));
        }
    }

    public bool IsMatch(string name)
    {
        using var _ = BitSetPool.Acquire(out var currentStates);
        using var __ = BitSetPool.Acquire(out var nextStates);

        EnableStateWithFreeMove(currentStates, 0);

        foreach (var c in name)
        {
            foreach (var currentState in currentStates)
            {
                if (currentState >= nodes.Count)
                {
                    continue;
                }

                var node = nodes[currentState];
                if (!IsMatch(node, c))
                {
                    continue;
                }

                if (node.Type == NodeType.WildcardStar)
                {
                    nextStates[currentState] = true;
                }

                EnableStateWithFreeMove(nextStates, currentState + 1);
            }

            // Swap
            (currentStates, nextStates) = (nextStates, currentStates);
            nextStates.Clear();
        }

        return currentStates[nodes.Count];
    }

    /// <summary>
    /// Sets the state at the specified index to true and process free move for star operator.
    /// </summary>
    /// <remarks>
    /// Patterns are expected to be optimized so that there are never two adjacent star operators
    /// This means we only need to check the immediate node
    /// </remarks>
    private void EnableStateWithFreeMove(BitSet states, int index)
    {
        states[index] = true;

        // Process free move for star operator
        if (index < nodes.Count)
        {
            var nextNode = nodes[index];
            if (nextNode.Type == NodeType.WildcardStar)
            {
                states[index + 1] = true;
            }
        }
    }

    private bool IsMatch(Node node, char c)
    {
        switch (node.Type)
        {
            case NodeType.WildcardStar:
            case NodeType.WildcardQuestion:
            {
                return true;
            }

            case NodeType.Literal:
            default:
            {
                return node.Value == c;
            }
        }
    }

    private record struct Node(NodeType Type, char Value);

    private enum NodeType
    {
        Literal,
        WildcardStar,
        WildcardQuestion,
    }
}
