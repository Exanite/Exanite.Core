using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Exanite.Core.Io;
using Exanite.Core.Io.Globbing;
using Xunit;

namespace Exanite.Core.Tests.Io;

public class GlobMatcherTests
{
    private static readonly Folder WebDev = new("root", ["package.json", "tsconfig.json", "vite.config.ts", "README.md"])
    {
        new Folder("root/public", ["favicon.ico", "index.html", "manifest.json"]),
        new Folder("root/src", ["App.tsx", "App.css", "main.tsx", "vite-env.d.ts"])
        {
            new Folder("root/src/assets", ["logo.svg", "hero.png", "product.json", "product0.png", "product1.png", "product2.png"]),
            new Folder("root/src/components")
            {
                new Folder("root/src/components/Button", ["Button.tsx", "Button.test.tsx", "Button.css"]),
                new Folder("root/src/components/Navbar", ["Navbar.tsx", "Navbar.test.tsx", "Navbar.css"]),
            },
            new Folder("root/src/hooks", ["useAuth.ts", "useFetch.ts"]),
            new Folder("root/src/services", ["api.ts", "logger.ts"]),
            new Folder("root/src/utils", ["helpers.ts", "math.ts"]),
        },
        new Folder("root/tests")
        {
            new Folder("root/tests/e2e", ["auth.spec.ts", "home.spec.ts"]),
        },
    };

    [Fact]
    public void DirectReference()
    {
        var matcher = new GlobMatcher(["package.json"]);
        var rawResults = matcher.Match(WebDev).ToList();
        var results = new HashSet<string>(rawResults);

        Assert.Equal(results.Count, rawResults.Count);

        var expected = new HashSet<string>()
        {
            "root/package.json",
        };

        Assert.Equal(expected, results);
    }

    [Fact]
    public void DeepDirectReference()
    {
        var matcher = new GlobMatcher(["src/components/Navbar/Navbar.test.tsx"]);
        var rawResults = matcher.Match(WebDev).ToList();
        var results = new HashSet<string>(rawResults);

        Assert.Equal(results.Count, rawResults.Count);

        var expected = new HashSet<string>()
        {
            "root/src/components/Navbar/Navbar.test.tsx",
        };

        Assert.Equal(expected, results);
    }

    [Fact]
    public void SimpleWildcardSegment()
    {
        var matcher = new GlobMatcher(["*"]);
        var rawResults = matcher.Match(WebDev).ToList();
        var results = new HashSet<string>(rawResults);

        Assert.Equal(results.Count, rawResults.Count);

        var expected = new HashSet<string>()
        {
            "root/package.json",
            "root/tsconfig.json",
            "root/vite.config.ts",
            "root/README.md",
        };

        Assert.Equal(expected, results);
    }

    [Fact]
    public void AllOfSpecificFileExtension()
    {
        var matcher = new GlobMatcher(["**/*.spec.ts"]);
        var rawResults = matcher.Match(WebDev).ToList();
        var results = new HashSet<string>(rawResults);

        Assert.Equal(results.Count, rawResults.Count);

        var expected = new HashSet<string>()
        {
            "root/tests/e2e/auth.spec.ts",
            "root/tests/e2e/home.spec.ts",
        };

        Assert.Equal(expected, results);
    }

    [Fact]
    public void AllButtonTsxFiles()
    {
        var matcher = new GlobMatcher(["**/components/Button/*.tsx"]);
        var rawResults = matcher.Match(WebDev).ToList();
        var results = new HashSet<string>(rawResults);

        Assert.Equal(results.Count, rawResults.Count);

        var expected = new HashSet<string>()
        {
            "root/src/components/Button/Button.tsx",
            "root/src/components/Button/Button.test.tsx",
        };

        Assert.Equal(expected, results);
    }

    [Fact]
    public void AllNumberedProductImages()
    {
        var matcher = new GlobMatcher(["**/product?.png"]);
        var rawResults = matcher.Match(WebDev).ToList();
        var results = new HashSet<string>(rawResults);

        Assert.Equal(results.Count, rawResults.Count);

        var expected = new HashSet<string>()
        {
            "root/src/assets/product0.png",
            "root/src/assets/product1.png",
            "root/src/assets/product2.png",
        };

        Assert.Equal(expected, results);
    }

    [Fact]
    public void AllIncludeAllExclude()
    {
        var matcher = new GlobMatcher(["**/*", "!**/*"]);
        var rawResults = matcher.Match(WebDev).ToList();
        var results = new HashSet<string>(rawResults);

        Assert.Equal(results.Count, rawResults.Count);

        var expected = new HashSet<string>();

        Assert.Equal(expected, results);
    }

    [Fact]
    public void AllJsonNonSrc()
    {
        var matcher = new GlobMatcher(["**/*.json", "!src/**"]);
        var rawResults = matcher.Match(WebDev).ToList();
        var results = new HashSet<string>(rawResults);

        Assert.Equal(results.Count, rawResults.Count);

        var expected = new HashSet<string>()
        {
            "root/package.json",
            "root/tsconfig.json",
            "root/public/manifest.json",
        };

        Assert.Equal(expected, results);
    }

    [Fact]
    public void AllJsonNonSrc_MoreSpecific()
    {
        var matcher = new GlobMatcher(["**/*.json", "!src/**/*.json"]);
        var rawResults = matcher.Match(WebDev).ToList();
        var results = new HashSet<string>(rawResults);

        Assert.Equal(results.Count, rawResults.Count);

        var expected = new HashSet<string>()
        {
            "root/package.json",
            "root/tsconfig.json",
            "root/public/manifest.json",
        };

        Assert.Equal(expected, results);
    }

    private class Folder : IGlobFolder, IEnumerable
    {
        private readonly Dictionary<string, Folder> folders = [];
        private readonly string[] files;

        public string Name { get; }
        public string Path { get; }

        public Folder(string path, string[]? files = null)
        {
            Name = path;
            Path = path;

            var index = Name.LastIndexOf('/');
            if (index >= 0)
            {
                Name = Name[(index + 1)..];
            }

            this.files = files ?? [];
        }

        // Strictly for collection initializer syntax
        public void Add(Folder folder)
        {
            folders.Add(folder.Name, folder);
        }

        // Strictly for collection initializer syntax
        IEnumerator IEnumerable.GetEnumerator()
        {
            return folders.Values.Cast<object>().Concat(files).GetEnumerator();
        }

        public IGlobFolder GetFolder(string name)
        {
            return folders[name];
        }

        public IEnumerable<string> GetFolders()
        {
            return folders.Keys;
        }

        public IEnumerable<string> GetFiles()
        {
            return files;
        }
    }
}
