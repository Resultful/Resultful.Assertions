---
type: decision
title: "Publish only what changed"
description: "Each package has its own version and release tags; CI works out which packages changed and which have a new version, and a manual run on main publishes only those."
status: proposed
tags: [resultful-assertions, decision, release, ci]
decided:
  by: ""
  at: 2026-11-20
generated:
  by: claude-opus-5.5
  at: 2026-11-20
---

# 4. Publish only what changed

Proposed. The repository holds many packages, released independently. A release publishes only the packages
whose code changed, never the whole repository.

## The scheme

- **Each package declares its version** with `<Version>` in its own `.csproj`. A release starts with a pull
  request that bumps the versions to publish, so every release is reviewed.
- **Each published version is tagged** `<PackageId>/v<version>`, for example `FluentAssertions.OneOf/v0.0.7`. The
  publish job creates the tags after the push succeeds. The tags are the record of what was released from which
  commit.
- **A package's inputs** are its project folder, the folders of every project it references (directly or
  transitively), `Directory.Build.props`, the root `README.md` and `LICENSE` (both packed into every package), and
  the `Directory.Packages.props` entries for the packages it (transitively) references. Tests, docs, `eng/` and CI
  are not inputs. A Dependabot bump of a test package changes nothing that ships.
- **The release plan** (`eng/release-plan.cs`, run on every pull request and push) gives each package one of
  these states:
  - **publish**: its version has no release tag, isn't on nuget.org, and is higher than its last tagged
    version.
  - **pending**: its inputs changed since its last release tag (or it has no tag yet), but its version is
    already released. It needs a version bump before it can be released. A change to a shared project shows up
    here for every dependant.
  - **unchanged**: nothing it ships has changed since its last release tag.
- **Dependencies are checked.** A package can be published only if every in-repo package it references is
  either published in the same run or unchanged since its tag. Otherwise its nuspec would point at a version on
  nuget.org that doesn't contain the code it was built against.
- **Dependants are reported, not forced.** When a shared project is bumped, its dependants show as pending. The
  maintainer decides whether to bump them in the same release. This is how a package can be held back
  (`FluentAssertions.Resultful` is, until `Resultful 0.2.0-beta06` is published).
- **Publishing is a manual workflow run on `main`** with `publish` ticked. Merging never publishes. The build job
  computes the plan, packs, and runs the package check (`eng/verify-package.cs`). The publish job pushes only the
  planned packages and then tags them. `dotnet nuget push --skip-duplicate` remains as a safety net, not the
  mechanism.

## Bootstrapping

Nothing has been released from this repository with a tag. The last published version of all four packages is
`0.0.6-beta01`, and the code has changed since. The packages start at that version with no tags, so the plan
reports all four as pending. The first release bumps the ones the maintainer wants to ship.

## Alternatives considered

- **MinVer with per-package tag prefixes.** It computes versions from tags and commit height, so every commit
  changes every package's version. A republished package would then reference a version of a sibling that was
  never pushed, and the release still needs separate change detection.
- **Push everything with `--skip-duplicate`.** It only works if versions don't change, and it hides real
  mistakes (a changed package with an unchanged version is skipped silently).
- **Path filters in the workflow.** They see one push, not everything since the last release, and they don't
  follow project references.

Why: one version per package keeps each package's release history meaningful to its own users. Most users of
`FluentAssertions.OneOf` don't use Resultful, and shouldn't see a new version every time `FluentAssertions.Resultful`
changes.
