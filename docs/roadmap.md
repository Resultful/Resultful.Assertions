---
type: roadmap
title: "Roadmap"
description: "Where the Resultful.Assertions package family is going: per-package releases, a library-independent Resultful.Assertions core with adapters, more assertion libraries, and the known issues."
status: draft
tags: [resultful-assertions, roadmap]
generated:
  by: claude-opus-5.5
  at: 2026-11-20
---

# Roadmap

## The goal

Asserting on a result, option or union should be one readable line in whichever assertion library a team uses
([vision](vision.md)). Each step below is a series of small pull requests, each mergeable on its own and each
leaving `main` releasable. Merging never publishes: a release is a manual workflow run on `main`
([decision 4](decisions/0004-publish-only-what-changed.md)). There are no calendar dates.

This is the assertions track (A1 to A5) of [Resultful's roadmap](https://github.com/Resultful/Resultful/blob/main/docs/roadmap.md),
in more detail.

## Done

| Step | Pull requests |
|---|---|
| **Stop publishing on push.** The old workflow packed every project as `1.0.0` and pushed it on each merge to `main`; it only failed because the API key was rejected. | #9 |
| **Rename** to `Resultful.Assertions` ([decision 1](decisions/0001-repository-is-resultful-assertions.md)), keeping package IDs ([decision 2](decisions/0002-package-ids-are-kept.md)); solution as `.slnx`. | #10 |
| **CI to the core repository's standard**: least privilege, `persist-credentials: false`, SDK from `global.json`, zizmor, ls-lint, Dependabot. | #9 |
| **Vision and docs bundle.** | this pull request |

## Known issues

- **`FluentAssertions.Resultful` carries a vulnerable dependency chain.** It depends on `Resultful 0.2.0-beta05`,
  which pulls `OneOf 3.0.178` and through it `NETStandard.Library 1.6.1` (NU1903 on `net8.0` and later).
  `Resultful 0.2.0-beta06` fixes this but is not published yet. Until it is:
  - `FluentAssertions.Resultful` is held back from releases; the package check refuses to publish it while its
    `Resultful` floor is below `0.2.0-beta06`.
  - Don't move `Directory.Packages.props` to an unpublished version; restore would fail.
  - Once `0.2.0-beta06` is on nuget.org: bump `Resultful`, bump `FluentAssertions.Resultful`'s version, release.

## Next

### R1. Publish only what changed

[Decision 4](decisions/0004-publish-only-what-changed.md). Each package declares its own version; a release tags
each published package as `<PackageId>/v<version>`. CI computes, on every pull request, which packages changed
since their last tag and which have a new version to publish. The manual publish run pushes only those.

Done when: a release that bumps one package publishes exactly that package, and the plan is visible on every pull
request.

The first release after this lands will publish every package whose version is bumped. Nothing has been
released since `0.0.6-beta01`, and the code has changed since (the .NET 10 upgrade), so all four are reported as
changed until their versions are bumped.

### R2. A library-independent `Resultful.Assertions` core, with adapters

[Decision 3](decisions/0003-core-plus-adapters.md), proposed.

- `Resultful.Assertions`: `ShouldSucceed()`, `ShouldFail()` and `ShouldFailWith(...)`, throwing an exception
  every test framework reports as a failure. Depends only on `Resultful`.
- `Resultful.Assertions.FluentAssertions` and `Resultful.Assertions.AwesomeAssertions`: each library's native
  style (`result.Should().BeSuccess()`).
- **Licensing.** FluentAssertions 8 and later is commercial (Xceed licence); 7.x and AwesomeAssertions are not.
  The FluentAssertions adapter references the lowest version it supports (7.x), so it never forces the
  commercial licence on anyone; a consumer who resolves 8 or later carries that licence. Which range the adapter
  supports, and whether 8 changed the extension points it needs, is still to be checked.
- Needs `Resultful 0.2.0-beta06` or later on nuget.org first.

Done when: a project can assert on a `Resultful` result with no assertion library at all, and with
FluentAssertions or AwesomeAssertions in their own style.

### R3. Deprecate `FluentAssertions.Resultful`

Deprecate it on NuGet in favour of `Resultful.Assertions.FluentAssertions` once that exists. Nothing else changes
ID: `FluentAssertions.OneOf`, `FluentAssertions.OneOf.Extended` and `FluentAssertions.Union` stay.

### R4. More assertion libraries and more types

- Shouldly and TUnit adapters, where their extension models fit.
- AwesomeAssertions packages for OneOf and unions, alongside the FluentAssertions ones.
- Other result and option libraries where there is demand.
- Track Resultful's breaking release (its M3) in the adapters.

### R5. Release hygiene

- A per-package README, so each NuGet page describes its own package and a README edit doesn't mark every package
  as changed.
- A `release` environment with required reviewers on the publish job (it doesn't exist in this repository yet).
- NuGet trusted publishing (`NuGet/login` with `id-token: write`) instead of the org `NUGET_KEY` secret.
- Restore the commented-out structural comparison in `FluentAssertions.OneOf`, or remove it.
