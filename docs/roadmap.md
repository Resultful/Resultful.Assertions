---
type: roadmap
title: "Roadmap"
description: "Where the Union.Assertions package family is going: per-package releases, more assertion libraries for OneOf and unions, and the known issues. Resultful's own assertions moved to Resultful/Resultful."
status: draft
tags: [union-assertions, roadmap]
generated:
  by: claude-opus-5.5
  at: 2026-11-20
---

# Roadmap

## The goal

Asserting on a union should be one readable line in whichever assertion library a team uses
([vision](vision.md)). Each step below is a series of small pull requests, each mergeable on its own and each
leaving `main` releasable. Merging never publishes: a release is a manual workflow run on `main`
([decision 4](decisions/0004-publish-only-what-changed.md)). There are no calendar dates.

This started as the assertions track (A1 to A5) of [Resultful's roadmap](https://github.com/Resultful/Resultful/blob/main/docs/roadmap.md).
Since [Resultful decision 18](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0018-resultful-assertions-live-here.md),
the assertions for Resultful's own types (R2 and R3 below) are tracked there; this roadmap covers the OneOf and
union packages.

## Done

| Step | Pull requests |
|---|---|
| **Stop publishing on push.** The old workflow packed every project as `1.0.0` and pushed it on each merge to `main`; it only failed because the API key was rejected. | #9 |
| **Rename** to `Resultful.Assertions` ([decision 1](decisions/0001-repository-is-resultful-assertions.md)), keeping package IDs ([decision 2](decisions/0002-package-ids-are-kept.md)); solution as `.slnx`. | #10 |
| **CI to the core repository's standard**: least privilege, `persist-credentials: false`, SDK from `global.json`, zizmor, ls-lint, Dependabot. | #9 |
| **Vision and docs bundle.** | #12 |
| **Publish only what changed** (R1, [decision 4](decisions/0004-publish-only-what-changed.md)): per-package versions and `<PackageId>/v<version>` tags, a release plan on every build, a package-shape check, and a manual publish job on `main` that pushes and tags only the planned packages. | #15 |
| **Rename** to `Union.Assertions` ([decision 5](decisions/0005-repository-is-union-assertions.md)), keeping package IDs; solution as `Union.Assertions.slnx`. R2 and R3 moved to Resultful/Resultful. | #18 |

## Known issues

- **`FluentAssertions.Resultful` carries a vulnerable dependency chain.** It depends on `Resultful 0.2.0-beta05`,
  which pulls `OneOf 3.0.178` and through it `NETStandard.Library 1.6.1` (NU1903 on `net8.0` and later).
  `Resultful 0.2.0-beta06` fixes this but is not published yet. Until it is:
  - `FluentAssertions.Resultful` is held back from releases; the package check refuses to publish it while its
    `Resultful` floor is below `0.2.0-beta06`.
  - Don't move `Directory.Packages.props` to an unpublished version; restore would fail.
  - It gets no new assertions: it is to be deprecated on NuGet in favour of `Resultful.Assertions.FluentAssertions`
    ([Resultful decision 18](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0018-resultful-assertions-live-here.md)),
    so it may never be released from here again. See R6.

## Next

### R1. The first tagged release

Publishing only what changed is in place (see Done). Nothing has been released from this repository with a tag:
the last published version of all four packages is `0.0.6-beta01`, and the code has changed since (the .NET 10
upgrade), so the release plan reports all four as pending. The first release is a pull request that bumps the
versions to ship, then a manual run with `publish` ticked. A package with no tag counts as changed, so
`FluentAssertions.OneOf` can't be released before `FluentAssertions.Union`, which it references (the plan fails
the build if it is tried); release them together.

Done when: every package that ships has a `<PackageId>/v<version>` tag.

### R2. A library-independent `Resultful.Assertions` core, with adapters

Moved to Resultful/Resultful ([Resultful decision 18](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0018-resultful-assertions-live-here.md)):
`Resultful.Assertions` and its AwesomeAssertions and FluentAssertions adapters are built there, against the
in-repo core.

### R3. Deprecate `FluentAssertions.Resultful`

Moved to Resultful/Resultful, with R2: it is to be deprecated on NuGet in favour of
`Resultful.Assertions.FluentAssertions` once that is published. Removing the project from this repository is R6.

### R4. More assertion libraries and more types

- AwesomeAssertions packages for OneOf and unions, alongside the FluentAssertions ones.
- Shouldly and TUnit adapters for OneOf and unions, where their extension models fit.
- Other union libraries where there is demand.

### R5. Release hygiene

- A per-package README, so each NuGet page describes its own package and a README edit doesn't mark every package
  as changed.
- Required reviewers on the `release` environment the publish job uses. GitHub creates the environment, unprotected, on the first publish run; add the reviewers (and move the `NUGET_KEY` secret into it) before that.
- NuGet trusted publishing (`NuGet/login` with `id-token: write`) instead of the org `NUGET_KEY` secret.
- Restore the commented-out structural comparison in `FluentAssertions.OneOf`, or remove it.

### R6. Remove `FluentAssertions.Resultful`

Once `FluentAssertions.Resultful` is deprecated on NuGet (R3), remove `src/FluentAssertions.Resultful`, its tests,
the `Resultful` entry in `Directory.Packages.props`, and the `Resultful` floor check in `eng/verify-package.cs`.
After that nothing in this repository depends on Resultful.
