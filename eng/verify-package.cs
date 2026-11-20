#:package NuGet.Versioning@7.9.0
#:property PublishAot=false
#:property ManagePackageVersionsCentrally=false
// Pack-and-inspect check for the packages in this repository, adapted from Resultful's eng/verify-package.cs.
//
// Usage: dotnet run eng/verify-package.cs -- [--plan <release-plan.txt>] <nupkg> [<nupkg> ...]
//   --plan   the release plan's output ("<id> <version>" per line); a package in it is about to be
//            published, so the checks that only matter for a release become errors instead of warnings
//
// Fails a package that:
//   - has a .NETStandard1.x or .NETFramework dependency group, or a NETStandard.Library/System.ValueTuple dependency
//   - is missing lib/netstandard2.0/<id>.dll or lib/net10.0/<id>.dll
//   - has a OneOf or OneOf.Extended floor below 3.0.271 (older OneOf pulls NETStandard.Library 1.6.1, NU1903)
//   - has a FluentAssertions floor of 8 or above (8+ is commercially licensed; these packages target 7.x)
//   - lacks the MIT licence expression, the README, the repository url and commit (SourceLink), or a .snupkg
// Warns (an error if the package is in the plan) when the Resultful floor is below 0.2.0-beta06: beta05
// carries NU1903 for consumers (docs/roadmap.md, known issues).

using System.IO.Compression;
using System.Xml.Linq;
using NuGet.Versioning;

var planIndex = Array.IndexOf(args, "--plan");
var planned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
if (planIndex >= 0)
{
    if (planIndex + 1 >= args.Length || !File.Exists(args[planIndex + 1]))
    {
        Console.Error.WriteLine("error: --plan needs an existing file");
        return 2;
    }

    foreach (var line in File.ReadAllLines(args[planIndex + 1]).Where(l => l.Trim().Length > 0))
    {
        planned.Add(line.Trim().Split(' ')[0]);
    }
}

var packages = args.Where((a, i) => planIndex < 0 || (i != planIndex && i != planIndex + 1)).ToList();
if (packages.Count == 0)
{
    Console.Error.WriteLine("usage: dotnet run eng/verify-package.cs -- [--plan <file>] <nupkg> [<nupkg> ...]");
    return 2;
}

var minOneOf = NuGetVersion.Parse("3.0.271");
var minResultful = NuGetVersion.Parse("0.2.0-beta06");
string[] banned = ["NETStandard.Library", "System.ValueTuple"];
string[] frameworks = ["netstandard2.0", "net10.0"];
var exit = 0;

foreach (var path in packages)
{
    if (!File.Exists(path))
    {
        Console.Error.WriteLine($"error: {path} not found");
        return 2;
    }

    var failures = new List<string>();
    var warnings = new List<string>();
    using var zip = ZipFile.OpenRead(path);
    var files = zip.Entries.Select(e => e.FullName).ToList();
    var nuspecEntry = zip.Entries.Single(e => e.FullName.EndsWith(".nuspec", StringComparison.Ordinal) && !e.FullName.Contains('/'));
    XDocument nuspec;
    using (var stream = nuspecEntry.Open())
    {
        nuspec = XDocument.Load(stream);
    }

    var ns = nuspec.Root!.Name.Namespace;
    var metadata = nuspec.Root.Element(ns + "metadata")!;
    var id = metadata.Element(ns + "id")!.Value;
    var inPlan = planned.Contains(id);
    var groups = metadata.Element(ns + "dependencies")?.Elements(ns + "group").ToList() ?? [];
    var dependencies = groups.SelectMany(g => g.Elements(ns + "dependency")).ToList();

    if (groups.Count == 0)
    {
        failures.Add("nuspec has no dependency groups");
    }

    foreach (var group in groups)
    {
        var tfm = (string?)group.Attribute("targetFramework") ?? "";
        if (tfm.StartsWith(".NETStandard1.", StringComparison.Ordinal) || tfm.StartsWith(".NETFramework", StringComparison.Ordinal))
        {
            failures.Add($"nuspec has a {tfm} dependency group");
        }
    }

    foreach (var dependency in dependencies)
    {
        var depId = (string)dependency.Attribute("id")!;
        var text = (string?)dependency.Attribute("version") ?? "";
        var floor = VersionRange.TryParse(text, out var range) ? range.MinVersion : null;
        if (banned.Contains(depId, StringComparer.OrdinalIgnoreCase))
        {
            failures.Add($"nuspec depends on {depId}");
        }

        if ((depId.Equals("OneOf", StringComparison.OrdinalIgnoreCase) || depId.Equals("OneOf.Extended", StringComparison.OrdinalIgnoreCase))
            && (floor is null || floor < minOneOf))
        {
            failures.Add($"{depId} floor {text} is below {minOneOf}");
        }

        if (depId.Equals("FluentAssertions", StringComparison.OrdinalIgnoreCase) && (floor is null || floor.Major >= 8))
        {
            failures.Add($"FluentAssertions floor {text} is 8 or above (commercially licensed)");
        }

        if (depId.Equals("Resultful", StringComparison.OrdinalIgnoreCase) && (floor is null || floor < minResultful))
        {
            (inPlan ? failures : warnings).Add($"Resultful floor {text} is below {minResultful} (NU1903 for consumers)");
        }
    }

    foreach (var framework in frameworks.Where(f => !files.Contains($"lib/{f}/{id}.dll")))
    {
        failures.Add($"missing lib/{framework}/{id}.dll");
    }

    if (files.Any(f => f.StartsWith("lib/netstandard1.", StringComparison.Ordinal)))
    {
        failures.Add("package ships a lib/netstandard1.x assembly");
    }

    var license = metadata.Element(ns + "license");
    if ((string?)license?.Attribute("type") != "expression" || license?.Value != "MIT")
    {
        failures.Add("missing MIT license expression");
    }

    if (metadata.Element(ns + "readme")?.Value != "README.md" || !files.Contains("README.md"))
    {
        failures.Add("README.md missing from metadata or package");
    }

    var repository = metadata.Element(ns + "repository");
    if ((string?)repository?.Attribute("url") != "https://github.com/Resultful/Resultful.Assertions.git")
    {
        failures.Add("missing or wrong repository url");
    }

    var commit = (string?)repository?.Attribute("commit") ?? "";
    if (commit.Length != 40 || !commit.All(Uri.IsHexDigit))
    {
        failures.Add("repository metadata has no commit (SourceLink)");
    }

    var snupkg = Path.ChangeExtension(path, ".snupkg");
    if (!File.Exists(snupkg))
    {
        failures.Add($"missing symbols package {Path.GetFileName(snupkg)}");
    }

    foreach (var warning in warnings.Distinct())
    {
        Console.WriteLine($"warning: {Path.GetFileName(path)}: {warning}");
    }

    if (failures.Count > 0)
    {
        Console.Error.WriteLine($"Package check FAILED for {Path.GetFileName(path)}{(inPlan ? " (in the release plan)" : "")}:");
        foreach (var f in failures.Distinct())
        {
            Console.Error.WriteLine($"  - {f}");
        }

        exit = 1;
        continue;
    }

    Console.WriteLine($"Package check passed for {Path.GetFileName(path)}{(inPlan ? " (in the release plan)" : "")}");
    Console.WriteLine($"  dependency groups: {string.Join(' ', groups.Select(g => (string?)g.Attribute("targetFramework")))}");
    Console.WriteLine($"  dependencies: {string.Join(' ', dependencies.Select(d => $"{d.Attribute("id")!.Value} {d.Attribute("version")!.Value}").Distinct())}");
}

return exit;
