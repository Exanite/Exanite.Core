using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exanite.Core.Io;
using Exanite.Core.Io.Globbing;
using Xunit;

namespace Exanite.Core.Tests.Io;

public class GlobMatcherTests
{
    private static readonly Folder WebDev = new Folder("root",
    [
        new Folder("public", [], ["favicon.ico", "index.html", "manifest.json"]),
        new Folder("src",
        [
            new Folder("assets", [], ["logo.svg", "hero.png"]),
            new Folder("components",
            [
                new Folder("Button", [], ["Button.tsx", "Button.test.tsx", "Button.css"]),
                new Folder("Navbar", [], ["Navbar.tsx", "Navbar.test.tsx", "Navbar.css"]),
            ], []),
            new Folder("hooks", [], ["useAuth.ts", "useFetch.ts"]),
            new Folder("services", [], ["api.ts", "logger.ts"]),
            new Folder("utils", [], ["helpers.ts", "math.ts"]),
        ],
        ["App.tsx", "App.css", "main.tsx", "vite-env.d.ts"]),
        new Folder("tests",
        [
            new Folder("e2e", [], ["auth.spec.ts", "home.spec.ts"]),
        ], []),
    ],
    ["package.json", "tsconfig.json", "vite.config.ts", "README.md"]).PropagatePaths();

    [Fact]
    public void DirectReference()
    {
        var matcher = new GlobMatcher(["package.json"]);
        var rawResults = matcher.Match(WebDev).ToList();
        var results = new HashSet<string>(rawResults);

        Assert.Equal(results.Count, rawResults.Count);

        var expected = new HashSet<string>()
        {
            "package.json",
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
            "src/components/Navbar/Navbar.test.tsx",
        };

        Assert.Equal(expected, results);
    }

    private class Folder : IGlobFolder
    {
        public string Name { get; }
        public string Path { get; private set; }

        private readonly Dictionary<string, Folder> folders;
        private readonly string[] files;

        public Folder(string name, Folder[] folders, string[] files)
        {
            Name = name;
            Path = name;

            this.folders = folders.ToDictionary(x => x.Name, x => x);
            this.files = files;
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

        public Folder PropagatePaths()
        {
            foreach (var folder in folders.Values)
            {
                folder.Path = $"{Path}/{folder.Path}";
                folder.PropagatePaths();
            }

            return this;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            BuildTree(builder, this);

            return builder.ToString();
        }

        private void BuildTree(StringBuilder builder, Folder current, int depth = 0)
        {
            builder.AppendLine($"{new string(' ', depth * 2)}{current.Path}");
            foreach (var folderName in current.GetFolders())
            {
                var folder = (Folder)current.GetFolder(folderName);
                BuildTree(builder, folder, depth + 1);
            }
        }
    }
}
