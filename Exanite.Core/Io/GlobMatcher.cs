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

        // TODO: Optimize
        public void Match(IGlobFolder folder, List<ActivePattern> activePatterns)
        {
            // Handle free moves
            for (var i = activePatterns.Count - 1; i >= 0; i--)
            {
                var activePattern = activePatterns[i];
                var pattern = patterns[activePattern.PatternIndex];
                var segment = pattern.Segments[activePattern.SegmentIndex];
                var remainingSegmentCount = pattern.Segments.Count - activePattern.SegmentIndex;
                if (remainingSegmentCount >= 2 && segment is DoubleStarGlobSegment)
                {
                    activePatterns.Insert(i + 1, new ActivePattern(activePattern.PatternIndex, activePattern.SegmentIndex + 1));
                }
            }

            // Dedupe active patterns
            var rawActivePatterns = activePatterns;
            activePatterns = new List<ActivePattern>();
            var alreadyAddedActivePatterns = new HashSet<ActivePattern>();
            for (var i = rawActivePatterns.Count - 1; i >= 0; i--)
            {
                var activePattern = rawActivePatterns[i];
                if (!alreadyAddedActivePatterns.Add(activePattern))
                {
                    continue;
                }

                activePatterns.Add(activePattern);
            }
            activePatterns.Reverse();

            // Try to match files
            var files = folder.GetFiles().ToHashSet();
            foreach (var file in files)
            {
                for (var i = activePatterns.Count - 1; i >= 0; i--)
                {
                    var activePattern = activePatterns[i];
                    var pattern = patterns[activePattern.PatternIndex];
                    var segment = pattern.Segments[activePattern.SegmentIndex];
                    var remainingSegmentCount = pattern.Segments.Count - activePattern.SegmentIndex;
                    if (remainingSegmentCount != 1)
                    {
                        continue;
                    }

                    if (IsSegmentMatch(segment, file))
                    {
                        if (pattern.IsInclude)
                        {
                            ReportResult(folder, file);
                        }

                        break;
                    }
                }
            }

            // Try to expand into folders
            foreach (var childFolder in folder.GetFolders())
            {
                var isRelevant = false;
                for (var i = activePatterns.Count - 1; i >= 0; i--)
                {
                    var activePattern = activePatterns[i];
                    var pattern = patterns[activePattern.PatternIndex];
                    var firstSegment = pattern.Segments[activePattern.SegmentIndex];
                    var remainingSegmentCount = pattern.Segments.Count - activePattern.SegmentIndex;

                    // For includes, we have to open the folder if it is even potentially relevant
                    // For excludes, we can skip the folder once it is known to be definitely not relevant
                    if (pattern.IsInclude)
                    {
                        // Include: ** -> Definitely relevant
                        // Include: match/.. -> Potentially relevant

                        // **
                        if (firstSegment is DoubleStarGlobSegment)
                        {
                            isRelevant = true;
                            break;
                        }

                        // match/..
                        if (remainingSegmentCount >= 2 && IsSegmentMatch(firstSegment, childFolder))
                        {
                            isRelevant = true;
                            break;
                        }
                    }
                    else
                    {
                        // Exclude: ** -> Definitely not relevant
                        // Exclude: **/* -> Definitely not relevant
                        // Exclude: match/** -> Definitely not relevant
                        // Exclude: match/**/* -> Definitely not relevant
                        // Exclude: match/**/*/* -> Indeterminate
                        // Exclude: match/*/** -> Indeterminate

                        // **
                        if (remainingSegmentCount == 1 && pattern.Segments[activePattern.SegmentIndex] is DoubleStarGlobSegment)
                        {
                            break;
                        }

                        if (remainingSegmentCount == 2)
                        {
                            // **/*
                            if (pattern.Segments[activePattern.SegmentIndex] is DoubleStarGlobSegment
                                && pattern.Segments[activePattern.SegmentIndex + 1] is PatternGlobSegment { Pattern: "*" })
                            {
                                break;
                            }

                            // match/**
                            if (pattern.Segments[activePattern.SegmentIndex + 1] is DoubleStarGlobSegment
                                && IsSegmentMatch(firstSegment, childFolder))
                            {
                                break;
                            }
                        }

                        // match/**/*
                        if (remainingSegmentCount == 3
                            && pattern.Segments[activePattern.SegmentIndex + 1] is DoubleStarGlobSegment
                            && pattern.Segments[activePattern.SegmentIndex + 2] is PatternGlobSegment { Pattern: "*" }
                            && IsSegmentMatch(firstSegment, childFolder))
                        {
                            break;
                        }
                    }
                }

                if (!isRelevant)
                {
                    continue;
                }

                var nextActivePatterns = new List<ActivePattern>();
                for (var i = activePatterns.Count - 1; i >= 0; i--)
                {
                    var activePattern = activePatterns[i];
                    TryAdvancePattern(activePattern, childFolder, nextActivePatterns);
                }

                Match(folder.GetFolder(childFolder), nextActivePatterns);
            }
        }

        private bool IsSegmentMatch(GlobSegment segment, string name)
        {
            return segment switch
            {
                DoubleStarGlobSegment => true,
                LiteralGlobSegment literalSegment => literalSegment.IsMatch(name),
                PatternGlobSegment patternSegment => patternSegment.IsMatch(name),
                _ => throw ExceptionUtility.NotSupported(segment),
            };
        }

        private void TryAdvancePattern(ActivePattern activePattern, string folder, List<ActivePattern> next)
        {
            var pattern = patterns[activePattern.PatternIndex];
            var segment = pattern.Segments[activePattern.SegmentIndex];
            var remainingSegmentCount = pattern.Segments.Count - activePattern.SegmentIndex;

            // Case: ** -> Output as is
            // Case: **/.. -> Output ** and ..
            // Case: match -> Output nothing
            // Case: match/.. -> Output ..
            if (segment is DoubleStarGlobSegment)
            {
                next.Add(activePattern);

                if (remainingSegmentCount >= 2)
                {
                    next.Add(new ActivePattern(activePattern.PatternIndex, activePattern.SegmentIndex + 1));
                }

                return;
            }

            if (remainingSegmentCount >= 2 && IsSegmentMatch(segment, folder))
            {
                next.Add(new ActivePattern(activePattern.PatternIndex, activePattern.SegmentIndex + 1));
            }
        }

        private void ReportResult(IGlobFolder folder, string file)
        {
            results.Add($"{folder.Path}/{file}");
        }
    }

    private record struct ActivePattern(int PatternIndex, int SegmentIndex);
}
