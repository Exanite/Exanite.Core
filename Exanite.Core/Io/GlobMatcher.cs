using System.Collections.Generic;
using System.Linq;
using Exanite.Core.Io.Globbing;

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
        return [];
    }
}
