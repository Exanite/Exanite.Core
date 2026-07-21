using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Exanite.Core.Io;
using Exanite.Core.Io.Globbing;
using Xunit;

namespace Exanite.Core.Tests.Io;

public class GlobMatcherTests
{
    private readonly TestFolder folder = new TestFolder("root", ["package.json", "tsconfig.json", "vite.config.ts", "README.md"])
    {
        new TestFolder("root/public", ["favicon.ico", "index.html", "manifest.json"]),
        new TestFolder("root/src", ["App.tsx", "App.css", "main.tsx", "vite-env.d.ts"])
        {
            new TestFolder("root/src/assets", ["logo.svg", "hero.png", "product.json", "product!.png", "product?.png", "product0.png", "product1.png", "product2.png"]),
            new TestFolder("root/src/components")
            {
                new TestFolder("root/src/components/Button", ["Button.tsx", "Button.test.tsx", "Button.css"]),
                new TestFolder("root/src/components/Navbar", ["Navbar.tsx", "Navbar.test.tsx", "Navbar.css"]),
            },
            new TestFolder("root/src/hooks", ["useAuth.ts", "useFetch.ts"]),
            new TestFolder("root/src/services", ["api.ts", "logger.ts"]),
            new TestFolder("root/src/utils", ["helpers.ts", "math.ts"]),
        },
        new TestFolder("root/tests")
        {
            new TestFolder("root/tests/e2e", ["auth.spec.ts", "home.spec.ts"]),
        },
    }.Initialize();

    [Fact]
    public void DirectReference()
    {
        RunGlobTest(
            [
                "package.json",
            ],
            [
                "root/package.json",
            ],
            [
                "root",
            ]
        );
    }

    [Fact]
    public void DirectReferenceIncludeThenExclude()
    {
        RunGlobTest(
            [
                "package.json",
                "!package.json",
            ],
            [],
            [
                "root",
            ]
        );
    }

    [Fact]
    public void DirectReferenceIncludeThenDeepExclude()
    {
        RunGlobTest(
            [
                "package.json",
                "!**",
            ],
            [],
            [
                "root",
            ]
        );
    }

    [Fact]
    public void DeepDirectReference()
    {
        RunGlobTest(
            [
                "src/components/Navbar/Navbar.test.tsx",
            ],
            [
                "root/src/components/Navbar/Navbar.test.tsx",
            ],
            [
                "root",
                "root/src",
                "root/src/components",
                "root/src/components/Navbar",
            ]
        );
    }

    [Fact]
    public void SimpleWildcard()
    {
        RunGlobTest(
            [
                "*",
            ],
            [
                "root/package.json",
                "root/tsconfig.json",
                "root/vite.config.ts",
                "root/README.md",
            ],
            [
                "root",
            ]
        );
    }

    [Fact]
    public void RootLevelFileWildcard()
    {
        RunGlobTest(
            [
                "*.json",
            ],
            [
                "root/package.json",
                "root/tsconfig.json",
            ],
            [
                "root",
            ]
        );
    }

    [Fact]
    public void DeepWildcard()
    {
        RunGlobTest(
            [
                "**/*",
            ],
            [
                "root/package.json",
                "root/tsconfig.json",
                "root/vite.config.ts",
                "root/README.md",
                "root/public/favicon.ico",
                "root/public/index.html",
                "root/public/manifest.json",
                "root/src/App.tsx",
                "root/src/App.css",
                "root/src/main.tsx",
                "root/src/vite-env.d.ts",
                "root/src/assets/logo.svg",
                "root/src/assets/hero.png",
                "root/src/assets/product.json",
                "root/src/assets/product!.png",
                "root/src/assets/product?.png",
                "root/src/assets/product0.png",
                "root/src/assets/product1.png",
                "root/src/assets/product2.png",
                "root/src/components/Button/Button.tsx",
                "root/src/components/Button/Button.test.tsx",
                "root/src/components/Button/Button.css",
                "root/src/components/Navbar/Navbar.tsx",
                "root/src/components/Navbar/Navbar.test.tsx",
                "root/src/components/Navbar/Navbar.css",
                "root/src/hooks/useAuth.ts",
                "root/src/hooks/useFetch.ts",
                "root/src/services/api.ts",
                "root/src/services/logger.ts",
                "root/src/utils/helpers.ts",
                "root/src/utils/math.ts",
                "root/tests/e2e/auth.spec.ts",
                "root/tests/e2e/home.spec.ts",
            ],
            [
                "root",
                "root/public",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/components/Navbar",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    [Fact]
    public void DeepWildcardOnly()
    {
        RunGlobTest(
            [
                "**",
            ],
            [
                "root/package.json",
                "root/tsconfig.json",
                "root/vite.config.ts",
                "root/README.md",
                "root/public/favicon.ico",
                "root/public/index.html",
                "root/public/manifest.json",
                "root/src/App.tsx",
                "root/src/App.css",
                "root/src/main.tsx",
                "root/src/vite-env.d.ts",
                "root/src/assets/logo.svg",
                "root/src/assets/hero.png",
                "root/src/assets/product.json",
                "root/src/assets/product!.png",
                "root/src/assets/product?.png",
                "root/src/assets/product0.png",
                "root/src/assets/product1.png",
                "root/src/assets/product2.png",
                "root/src/components/Button/Button.tsx",
                "root/src/components/Button/Button.test.tsx",
                "root/src/components/Button/Button.css",
                "root/src/components/Navbar/Navbar.tsx",
                "root/src/components/Navbar/Navbar.test.tsx",
                "root/src/components/Navbar/Navbar.css",
                "root/src/hooks/useAuth.ts",
                "root/src/hooks/useFetch.ts",
                "root/src/services/api.ts",
                "root/src/services/logger.ts",
                "root/src/utils/helpers.ts",
                "root/src/utils/math.ts",
                "root/tests/e2e/auth.spec.ts",
                "root/tests/e2e/home.spec.ts",
            ],
            [
                "root",
                "root/public",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/components/Navbar",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    [Fact]
    public void DeepWildcardInMiddle()
    {
        RunGlobTest(
            [
                "src/**/Button.tsx",
            ],
            [
                "root/src/components/Button/Button.tsx",
            ],
            [
                "root",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/components/Navbar",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
            ]
        );
    }

    [Fact]
    public void DeepWildcardMatchesZeroLevels()
    {
        RunGlobTest(
            [
                "src/components/Button/**/Button.tsx",
            ],
            [
                "root/src/components/Button/Button.tsx",
            ],
            [
                "root",
                "root/src",
                "root/src/components",
                "root/src/components/Button",
            ]
        );
    }

    [Fact]
    public void AllOfSpecificFileExtension()
    {
        RunGlobTest(
            [
                "**/*.spec.ts",
            ],
            [
                "root/tests/e2e/auth.spec.ts",
                "root/tests/e2e/home.spec.ts",
            ],
            [
                "root",
                "root/public",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/components/Navbar",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    [Fact]
    public void AllButtonTsxFiles()
    {
        RunGlobTest(
            [
                "**/components/Button/*.tsx",
            ],
            [
                "root/src/components/Button/Button.tsx",
                "root/src/components/Button/Button.test.tsx",
            ],
            [
                "root",
                "root/public",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/components/Navbar",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    [Fact]
    public void AllNonTestTypescriptFiles()
    {
        RunGlobTest(
            [
                "**/*.ts",
                "**/*.tsx",
                "!**/*.test.tsx",
                "!**/*.spec.ts",
            ],
            [
                "root/src/App.tsx",
                "root/src/main.tsx",
                "root/src/components/Button/Button.tsx",
                "root/src/components/Navbar/Navbar.tsx",
                "root/src/hooks/useAuth.ts",
                "root/src/hooks/useFetch.ts",
                "root/src/services/api.ts",
                "root/src/services/logger.ts",
                "root/src/utils/helpers.ts",
                "root/src/utils/math.ts",
                "root/src/vite-env.d.ts",
                "root/vite.config.ts",
            ],
            [
                "root",
                "root/public",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/components/Navbar",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    [Fact]
    public void CaseSensitivity()
    {
        RunGlobTest(
            [
                "**/button.tsx",
            ],
            [],
            [
                "root",
                "root/public",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/components/Navbar",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    [Fact]
    public void AllNumberedProductImages()
    {
        RunGlobTest(
            [
                "**/product?.png",
            ],
            [
                "root/src/assets/product!.png",
                "root/src/assets/product?.png",
                "root/src/assets/product0.png",
                "root/src/assets/product1.png",
                "root/src/assets/product2.png",
            ],
            [
                "root",
                "root/public",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/components/Navbar",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    [Fact]
    public void AllDoubleNumberedProductImages()
    {
        RunGlobTest(
            [
                "**/product??.png",
            ],
            [],
            [
                "root",
                "root/public",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/components/Navbar",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    [Fact]
    public void EscapedWildcard()
    {
        RunGlobTest(
            [
                @"**/*\?.png",
            ],
            [
                "root/src/assets/product?.png",
            ],
            [
                "root",
                "root/public",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/components/Navbar",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    [Fact]
    public void EscapedExclamation()
    {
        RunGlobTest(
            [
                @"src/assets/product\!.png",
            ],
            [
                "root/src/assets/product!.png",
            ],
            [
                "root",
                "root/src",
                "root/src/assets",
            ]
        );
    }

    [Fact]
    public void AllIncludeAllExclude()
    {
        RunGlobTest(
            [
                "**/*",
                "!**/*",
            ],
            [],
            [
                "root",
            ]
        );
    }

    [Fact]
    public void AllIncludeAllExclude_ThenIncludeOne()
    {
        RunGlobTest(
            [
                "**/*",
                "!**/*",
                "package.json",
            ],
            [
                "root/package.json",
            ],
            [
                "root",
            ]
        );
    }

    [Fact]
    public void LastPatternPriorityOverride()
    {
        RunGlobTest(
            [
                "src/**/*",
                "!src/components/**/*",
                "src/components/Button/*",
            ],
            [
                "root/src/App.tsx",
                "root/src/App.css",
                "root/src/main.tsx",
                "root/src/vite-env.d.ts",
                "root/src/assets/logo.svg",
                "root/src/assets/hero.png",
                "root/src/assets/product.json",
                "root/src/assets/product!.png",
                "root/src/assets/product?.png",
                "root/src/assets/product0.png",
                "root/src/assets/product1.png",
                "root/src/assets/product2.png",
                "root/src/components/Button/Button.tsx",
                "root/src/components/Button/Button.test.tsx",
                "root/src/components/Button/Button.css",
                "root/src/hooks/useAuth.ts",
                "root/src/hooks/useFetch.ts",
                "root/src/services/api.ts",
                "root/src/services/logger.ts",
                "root/src/utils/helpers.ts",
                "root/src/utils/math.ts",
            ],
            [
                "root",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
            ]
        );
    }

    [Fact]
    public void AllJsonNonSrc()
    {
        RunGlobTest(
            [
                "**/*.json",
                "!src/**",
            ],
            [
                "root/package.json",
                "root/tsconfig.json",
                "root/public/manifest.json",
            ],
            [
                "root",
                "root/public",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    [Fact]
    public void AllJsonNonSrc_MoreSpecific()
    {
        RunGlobTest(
            [
                "**/*.json",
                "!src/**/*.json",
            ],
            [
                "root/package.json",
                "root/tsconfig.json",
                "root/public/manifest.json",
            ],
            [
                // This notably can be optimized, but I can't be bothered to right now
                "root",
                "root/public",
                "root/src",
                "root/src/assets",
                "root/src/components",
                "root/src/components/Button",
                "root/src/components/Navbar",
                "root/src/hooks",
                "root/src/services",
                "root/src/utils",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    [Fact]
    public void ExcludeFolderDeepWildcardOptimization()
    {
        RunGlobTest(
            [
                "**/*",
                "!src/**/*",
            ],
            [
                "root/package.json",
                "root/tsconfig.json",
                "root/vite.config.ts",
                "root/README.md",
                "root/public/favicon.ico",
                "root/public/index.html",
                "root/public/manifest.json",
                "root/tests/e2e/auth.spec.ts",
                "root/tests/e2e/home.spec.ts",
            ],
            [
                "root",
                "root/public",
                "root/tests",
                "root/tests/e2e",
            ]
        );
    }

    private void RunGlobTest(string[] patterns, string[] expectedMatches, string[] expectedAccesses)
    {
        var matcher = new GlobMatcher(patterns);
        var results = matcher.Match(folder).ToArray();

        var sortedExpectedFiles = expectedMatches.OrderBy(x => x).ToArray();
        var sortedActualFiles = results.OrderBy(x => x).ToArray();
        Assert.Equal(sortedExpectedFiles, sortedActualFiles);

        var sortedExpectedAccesses = expectedAccesses.OrderBy(x => x).ToArray();
        var sortedActualAccesses = folder.AccessedFolders.OrderBy(x => x).ToArray();
        Assert.Equal(sortedExpectedAccesses, sortedActualAccesses);
    }

    private class TestFolder : IGlobFolder, IEnumerable<string>
    {
        private readonly Dictionary<string, TestFolder> folders = [];
        private readonly string[] files;

        public string Name { get; }
        public string Path { get; }

        public HashSet<string> AccessedFolders { get; private set; } = null!;

        public TestFolder(string path, string[]? files = null)
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

        public IGlobFolder GetFolder(string name)
        {
            return folders[name];
        }

        public IEnumerable<string> GetFolders()
        {
            AccessedFolders.Add(Path);
            return folders.Keys;
        }

        public IEnumerable<string> GetFiles()
        {
            AccessedFolders.Add(Path);
            return files;
        }

        /// <summary>
        /// Call once on root.
        /// </summary>
        public TestFolder Initialize()
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            AccessedFolders ??= [];

            foreach (var folder in folders.Values)
            {
                folder.AccessedFolders = AccessedFolders;
                folder.Initialize();
            }

            return this;
        }

        // For collection initializer syntax
        public void Add(TestFolder testFolder)
        {
            folders.Add(testFolder.Name, testFolder);
        }

        // For collection initializer syntax
        public IEnumerator<string> GetEnumerator()
        {
            foreach (var folder in folders.Keys)
            {
                yield return folder;
            }

            foreach (var file in files)
            {
                yield return file;
            }
        }

        // For collection initializer syntax
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return Path;
        }
    }
}
