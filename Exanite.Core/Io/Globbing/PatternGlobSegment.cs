using System.Collections.Generic;
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

        currentStates[0] = true;
        if (nodes[0].Type == NodeType.WildcardStar)
        {
            // Process free move for star operator
            //
            // Patterns are expected to be optimized so that there are never two adjacent star operators
            // This means we only need to check the immediate node
            currentStates[1] = true;
        }

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

                nextStates[currentState + 1] = true;
                if (currentState + 1 < nodes.Count)
                {
                    // Process free move for star operator
                    var nextNode = nodes[currentState + 1];
                    if (nextNode.Type == NodeType.WildcardStar)
                    {
                        nextStates[currentState + 2] = true;
                    }
                }
            }

            // Swap
            (currentStates, nextStates) = (nextStates, currentStates);
            nextStates.Clear();
        }

        return currentStates[nodes.Count];
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
