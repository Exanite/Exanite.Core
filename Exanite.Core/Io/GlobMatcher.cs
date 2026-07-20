using System;
using System.Collections.Generic;
using System.Linq;
using Exanite.Core.Io.Globbing;
using Exanite.Core.Utilities;

namespace Exanite.Core.Io;

/// <summary>
/// Finds files matching a specified set of glob patterns.
/// Patterns are case-sensitive for cross-platform consistency.
/// If a file is both included and excluded, the last matched pattern takes priority.
/// Traversal is optimized by only traversing through relevant folders.
/// <para/>
/// Supported features:
/// <list type="bullet">
///     <item><description><c>/</c> as a path segment separator.</description></item>
///     <item><description><c>\</c> as an escape character. The next character will be matched verbatim.</description></item>
///     <item><description><c>!</c> at the beginning of a pattern for excluding matches.</description></item>
///     <item><description><c>?</c> within a segment for matching exactly one character.</description></item>
///     <item><description><c>*</c> within a segment for matching zero or more characters.</description></item>
///     <item><description><c>**</c> as a segment for matching zero or more folder levels.</description></item>
/// </list>
/// Characters not used by supported features are matched directly against file and folder names.
/// <para/>
/// Unsupported features:
/// <list type="bullet">
///     <item><description><c>\</c> as a path segment separator. Use <c>/</c> instead.</description></item>
///     <item><description><c>.</c> for matching the current folder. Using these will throw an error.</description></item>
///     <item><description><c>..</c> for matching the parent folder. Using these will throw an error.</description></item>
///     <item><description><c>[abc]</c> for matching character sets.</description></item>
///     <item><description><c>[a-z]</c> for matching character ranges.</description></item>
///     <item><description><c>{a,b,c}</c> for matching expanded character sets.</description></item>
///     <item><description>
///         Matching folders in general, such as by using a trailing <c>/</c>.
///         This glob implementation only works on files. For example, use <c>folder/**</c> instead of <c>folder/</c>.
///     </description></item>
/// </list>
/// <para/>
/// </summary>
public class GlobMatcher
{
    private readonly GlobPattern[] patterns;

    public GlobMatcher(IEnumerable<string> patterns)
    {
        this.patterns = patterns.Select(pattern => new GlobPattern(pattern)).ToArray();
    }

    public GlobMatcher(IEnumerable<GlobPattern> patterns)
    {
        this.patterns = patterns.ToArray();
    }

    public IEnumerable<string> Match(AbsolutePath path)
    {
        return Match(new FileSystemFolder(path));
    }

    public IEnumerable<string> Match(IGlobFolder folder)
    {
        var context = new MatchContext(patterns);
        var activePatterns = new List<ActivePattern>();
        for (var i = 0; i < patterns.Length; i++)
        {
            activePatterns.Add(new ActivePattern(i, 0));
        }

        context.Match(folder, activePatterns);

        return context.Results;
    }

    private class MatchContext
    {
        private readonly GlobPattern[] patterns;
        private readonly List<string> results = new();

        public IEnumerable<string> Results => results;

        public MatchContext(GlobPattern[] patterns)
        {
            this.patterns = patterns;
        }

        public void Match(IGlobFolder folder, List<ActivePattern> activePatterns)
        {
            // Try match files
            // TODO: Consider flipping order of iteration
            var files = folder.GetFiles().ToHashSet();
            foreach (var file in files)
            {
                for (var i = activePatterns.Count - 1; i >= 0; i--)
                {
                    var activePattern = activePatterns[i];
                    var pattern = patterns[activePattern.PatternIndex];
                    var segment = pattern.Segments[activePattern.SegmentIndex];

                    switch (segment)
                    {
                        case DoubleStarGlobSegment:
                        {
                            ReportResult(folder, file);
                            break;
                        }
                        case LiteralGlobSegment literalSegment:
                        {
                            if (literalSegment.IsMatch(file))
                            {
                                ReportResult(folder, file);
                            }

                            break;
                        }
                        case PatternGlobSegment patternSegment:
                        {
                            if (patternSegment.IsMatch(file))
                            {
                                ReportResult(folder, file);
                            }

                            break;
                        }
                        default: throw ExceptionUtility.NotSupported(segment);
                    }
                }
            }

            // Try expand into folders
            foreach (var childFolder in folder.GetFolders())
            {
                var isRelevant = false;
                for (var i = activePatterns.Count - 1; i >= 0; i--)
                {
                    var activePattern = activePatterns[i];
                    var pattern = patterns[activePattern.PatternIndex];
                    var firstSegment = pattern.Segments[activePattern.SegmentIndex];
                    var remainingSegmentCount = pattern.Segments.Count - activePattern.SegmentIndex;

                    var maybeMatch = false;
                    switch (firstSegment)
                    {
                        case DoubleStarGlobSegment:
                        {
                            maybeMatch = true;
                            break;
                        }
                        case LiteralGlobSegment literalSegment:
                        {
                            maybeMatch = remainingSegmentCount >= 2 && literalSegment.IsMatch(childFolder);
                            break;
                        }
                        case PatternGlobSegment patternSegment:
                        {
                            maybeMatch = remainingSegmentCount >= 2 && patternSegment.IsMatch(childFolder);
                            break;
                        }
                        default: throw ExceptionUtility.NotSupported(firstSegment);
                    }

                    if (maybeMatch)
                    {
                        if (!pattern.IsExclude)
                        {
                            // Folder is relevant if an include can maybe match
                            isRelevant = true;
                            break;
                        }

                        if (remainingSegmentCount == 1 && pattern.Segments[activePattern.SegmentIndex + 1] is DoubleStarGlobSegment)
                        {
                            // Folder is not relevant if an exclude can maybe match AND the next segment is the only segment AND the next segment is a double star segment
                            isRelevant = false;
                            break;
                        }
                    }
                }

                if (!isRelevant)
                {
                    continue;
                }

                // TODO
                Console.WriteLine(childFolder);
            }

            if (nint.Size == 0) // Disable
            {
                var folders = folder.GetFolders().ToHashSet();

                var relevantFolders = new List<string>();
                var relevantFoldersSet = new HashSet<string>();

                // This block contains optimizations and is not critical to the algorithm. Ignore it for now.
                var allLiteral = true;
                for (var i = activePatterns.Count - 1; i >= 0; i--)
                {
                    var activePattern = activePatterns[i];
                    var pattern = patterns[activePattern.PatternIndex];
                    var segment = pattern.Segments[activePattern.SegmentIndex];

                    if (segment is not LiteralGlobSegment)
                    {
                        allLiteral = false;
                        break;
                    }
                }

                if (allLiteral)
                {
                    // Only attempt to expand into
                }
                else
                {
                    // If not all literal, then we potentially have to expand into all folders, unless an exclusion preempts it
                    // This is the default case, so implement this first
                }
            }
        }

        private void ReportResult(IGlobFolder folder, string file)
        {
            results.Add($"{folder.Path}/{file}");
        }
    }

    private record struct ActivePattern(int PatternIndex, int SegmentIndex);
}
