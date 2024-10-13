using System;
using System.Diagnostics;
using Exanite.Core.Pooling;

namespace Exanite.Core.Utilities
{
    public static class StringUtility
    {
        /// <summary>
        /// Replaces line endings in the current text with the new line string for the current environment. See <see cref="Environment.NewLine">Environment.NewLine</see>.
        /// </summary>
        public static string UpdateNewLines(string text)
        {
            return UpdateNewLines(text, Environment.NewLine);
        }

        /// <summary>
        /// Replaces line endings in the current text with the specified new line string.
        /// </summary>
        public static string UpdateNewLines(string text, string newLine)
        {
            Debug.Assert(newLine == "\n" || newLine == "\r\n");

            using var _ = StringBuilderPool.Acquire(out var builder);

            var lastLineEnding = -1;
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    var currentIndex = i - 1;
                    if (i - 1 >= 0 && text[i - 1] == '\r')
                    {
                        currentIndex -= 2;
                    }

                    var length = currentIndex - lastLineEnding;
                    if (length > 0)
                    {
                        builder.Append(text.AsSpan().Slice(lastLineEnding + 1, length));
                    }

                    builder.Append(newLine);

                    lastLineEnding = i;
                }
            }

            if (lastLineEnding != text.Length - 1)
            {
                builder.Append(text.AsSpan(lastLineEnding + 1));
            }

            return builder.ToString();
        }
    }
}
