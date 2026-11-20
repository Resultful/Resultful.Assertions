#:package NuGet.Versioning@7.9.0
#:property PublishAot=false
#:property ManagePackageVersionsCentrally=false
// Release plan: which packages changed since their last release, and which have a new version to publish
// (docs/decisions/0004-publish-only-what-changed.md).
//
// Usage: dotnet run eng/release-plan.cs -- [--nuget] [--out <file>] [--summary <file>]
//   --nuget      also treat a version already on nuget.org as released (CI always passes it)
//   --out        write the packages to publish, one "<id> <version>" per line
//   --summary    append the plan as Markdown (pass $GITHUB_STEP_SUMMARY)
//
// Each packable project under src/ declares <Version>. A release is tagged "<PackageId>/v<version>".
// States:
//   publish    the version has no release tag, isn't on nuget.org, and is above the last tagged version
//   pending    what the package ships changed since its last release tag, but its version is already released
//   unchanged  nothing it ships changed since its last release tag
// Exits 1 if the plan is inconsistent: a version below the last release, or a package to publish that
// references an in-repo package which changed but isn't being published with it.

using System.Diagnostics;
using System.Xml.Linq;
using NuGet.Versioning;

var nuget = args.Contains("--nuget");
string? outFile = Option("--out");
string? summaryFile = Option("--summary");

var root = Git("rev-parse", "--show-toplevel").Output.Trim();
Directory.SetCurrentDirectory(root);

// Files outside the project folders that end up in every package.
string[] sharedInputs = ["Directory.Build.props", "README.md", "LICENSE"];

var projects = Directory.GetFiles(Path.Combine(root, "src"), "*.csproj", SearchOption.AllDirectories)
    .Select(Load)
    .Where(p => p.Packable)
    .OrderBy(p => p.Id, StringComparer.Ordinal)
    .ToList();
var byPath = Directory.GetFiles(Path.Combine(root, "src"), "*.csproj", SearchOption.AllDirectories)
    .Select(Load)
    .ToDictionary(p => p.Path, StringComparer.Ordinal);

var tags = Git("tag", "--list", "*/v*").Output
    .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(t => (Tag: t, Slash: t.LastIndexOf("/v", StringComparison.Ordinal)))
    .Select(t => (t.Tag, Id: t.Tag[..t.Slash], Ok: NuGetVersion.TryParse(t.Tag[(t.Slash + 2)..], out var v), Version: v))
    .Where(t => t.Ok)
    .ToList();

var centralNow = CentralVersions(File.ReadAllText("Directory.Packages.props"));
using var http = new HttpClient();
var errors = new List<string>();
var rows = new List<Row>();

foreach (var project in projects)
{
    var closure = Closure(project);
    var released = tags.Where(t => t.Id.Equals(project.Id, StringComparison.OrdinalIgnoreCase)).OrderByDescending(t => t.Version).ToList();
    var last = released.FirstOrDefault();
    var reasons = new List<string>();

    if (last.Tag is null)
    {
        reasons.Add("no release tag yet");
    }
    else
    {
        foreach (var p in closure)
        {
            if (Git("diff", "--quiet", last.Tag, "--", p.Dir).Code != 0)
            {
                reasons.Add(p == project ? "source" : $"via {p.Id}");
            }
        }

        foreach (var file in sharedInputs.Where(f => Git("diff", "--quiet", last.Tag, "--", f).Code != 0))
        {
            reasons.Add(file);
        }

        var then = Git("show", $"{last.Tag}:Directory.Packages.props");
        var centralThen = then.Code == 0 ? CentralVersions(then.Output) : new Dictionary<string, string>();
        foreach (var package in closure.SelectMany(p => p.PackageReferences).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            centralNow.TryGetValue(package, out var now);
            centralThen.TryGetValue(package, out var before);
            if (now != before)
            {
                reasons.Add($"{package} {before ?? "-"} -> {now ?? "-"}");
            }
        }
    }

    var changed = reasons.Count > 0;
    var isReleased = released.Any(t => t.Version == project.Version) || (nuget && await OnNuGet(project.Id, project.Version));
    string state;
    if (!isReleased)
    {
        state = "publish";
        if (last.Tag is not null && project.Version <= last.Version!)
        {
            errors.Add($"{project.Id}: version {project.Version} is not above the last release {last.Version}");
        }
    }
    else
    {
        state = changed ? "pending" : "unchanged";
    }

    rows.Add(new Row(project, state, last.Version?.ToNormalizedString() ?? "-", string.Join(", ", reasons.Distinct())));
}

var stateOf = rows.ToDictionary(r => r.Project.Id, r => r.State, StringComparer.OrdinalIgnoreCase);
foreach (var row in rows.Where(r => r.State == "publish"))
{
    foreach (var dependency in Closure(row.Project).Where(p => p != row.Project && p.Packable))
    {
        if (stateOf[dependency.Id] == "pending")
        {
            errors.Add($"{row.Project.Id} references {dependency.Id}, which changed since its last release but isn't being published; bump {dependency.Id} too");
        }
    }
}

var publish = rows.Where(r => r.State == "publish").ToList();
var table = new List<string>
{
    "| Package | Version | Last release | State | Changed since last release |",
    "|---|---|---|---|---|",
};
table.AddRange(rows.Select(r => $"| `{r.Project.Id}` | {r.Project.Version.ToNormalizedString()} | {r.Last} | **{r.State}** | {r.Reasons} |"));

var markdown = new List<string> { "## Release plan", "" };
markdown.AddRange(table);
markdown.Add("");
markdown.Add(publish.Count == 0
    ? "Nothing to publish: no package has a new version. Bump `<Version>` in a pending package's project to release it."
    : $"A manual publish run on `main` would push: {string.Join(", ", publish.Select(r => $"`{r.Project.Id} {r.Project.Version.ToNormalizedString()}`"))}.");
if (!nuget)
{
    markdown.Add("");
    markdown.Add("(nuget.org not checked; pass `--nuget`.)");
}

markdown.AddRange(errors.Select(e => $"- **error:** {e}"));

Console.WriteLine(string.Join('\n', markdown));
if (summaryFile is not null)
{
    File.AppendAllText(summaryFile, string.Join('\n', markdown) + "\n");
}

if (outFile is not null)
{
    File.WriteAllLines(outFile, publish.Select(r => $"{r.Project.Id} {r.Project.Version.ToNormalizedString()}"));
}

foreach (var error in errors)
{
    Console.Error.WriteLine($"error: {error}");
}

return errors.Count == 0 ? 0 : 1;

string? Option(string name)
{
    var i = Array.IndexOf(args, name);
    return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
}

List<Project> Closure(Project start)
{
    var seen = new List<Project>();
    var queue = new Queue<Project>([start]);
    while (queue.Count > 0)
    {
        var p = queue.Dequeue();
        if (seen.Contains(p))
        {
            continue;
        }

        seen.Add(p);
        foreach (var reference in p.ProjectReferences)
        {
            queue.Enqueue(byPath[reference]);
        }
    }

    return seen;
}

Project Load(string path)
{
    var full = Path.GetFullPath(path);
    var doc = XDocument.Load(full);
    var dir = Path.GetDirectoryName(full)!;
    string? Property(string name) => doc.Descendants(name).Select(e => e.Value.Trim()).LastOrDefault();
    var packable = !string.Equals(Property("IsPackable"), "false", StringComparison.OrdinalIgnoreCase);
    var versionText = Property("Version");
    if (packable && versionText is null)
    {
        throw new InvalidOperationException($"{Path.GetRelativePath(root, full)} has no <Version>; every package declares its own");
    }

    return new Project(
        full,
        Path.GetRelativePath(root, dir).Replace('\\', '/'),
        Property("PackageId") ?? Path.GetFileNameWithoutExtension(full),
        versionText is null ? new NuGetVersion(0, 0, 0) : NuGetVersion.Parse(versionText),
        packable,
        doc.Descendants("ProjectReference").Select(e => Path.GetFullPath(Path.Combine(dir, e.Attribute("Include")!.Value.Replace('\\', '/')))).ToList(),
        doc.Descendants("PackageReference").Select(e => e.Attribute("Include")!.Value).ToList());
}

static Dictionary<string, string> CentralVersions(string xml) =>
    XDocument.Parse(xml).Descendants("PackageVersion")
        .ToDictionary(e => e.Attribute("Include")!.Value, e => e.Attribute("Version")!.Value, StringComparer.OrdinalIgnoreCase);

async Task<bool> OnNuGet(string id, NuGetVersion version)
{
    var response = await http.GetAsync($"https://api.nuget.org/v3-flatcontainer/{id.ToLowerInvariant()}/index.json");
    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        return false;
    }

    response.EnsureSuccessStatusCode();
    using var json = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    return json.RootElement.GetProperty("versions").EnumerateArray().Any(v => NuGetVersion.Parse(v.GetString()!) == version);
}

static (int Code, string Output) Git(params string[] arguments)
{
    var info = new ProcessStartInfo("git") { RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var a in arguments)
    {
        info.ArgumentList.Add(a);
    }

    using var process = Process.Start(info)!;
    var output = process.StandardOutput.ReadToEnd();
    process.StandardError.ReadToEnd();
    process.WaitForExit();
    return (process.ExitCode, output);
}

sealed record Project(string Path, string Dir, string Id, NuGetVersion Version, bool Packable, List<string> ProjectReferences, List<string> PackageReferences);

sealed record Row(Project Project, string State, string Last, string Reasons);
