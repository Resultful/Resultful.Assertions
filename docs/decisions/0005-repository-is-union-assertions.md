---
type: decision
title: "The repository is Union.Assertions"
description: "Resultful.Assertions is renamed Union.Assertions: the home for OneOf and union assertion packages across assertion libraries, now that Resultful's own assertions live in Resultful/Resultful. Supersedes decision 1."
status: stable
tags: [union-assertions, decision]
decided:
  by: human:rtkelly13
  at: 2026-11-21
generated:
  by: claude-opus-5.5
  at: 2026-11-21
---

# 5. The repository is `Union.Assertions`

This supersedes [decision 1](0001-repository-is-resultful-assertions.md). The maintainer chose the name.
`Resultful/Resultful.Assertions` is renamed `Resultful/Union.Assertions`. It holds the assertion packages for
OneOf and other union types (`FluentAssertions.OneOf`, `FluentAssertions.OneOf.Extended`, `FluentAssertions.Union`
and future adapters for other assertion libraries), none of which depend on Resultful.

- The solution is `Union.Assertions.slnx`.
- `RepositoryUrl` and `PackageProjectUrl` point at the new name, and the package check expects it.
- Package IDs don't change ([decision 2](0002-package-ids-are-kept.md)).
- GitHub redirects the old repository URLs (both `FluentAssertions.Resultful` and `Resultful.Assertions`), so links
  and clones keep working; clones should still update their remote.
- The local clone moves from `~/code/Resultful/Resultful.Assertions` to `~/code/Resultful/Union.Assertions`, and
  the estate registry (rig) and Orca follow.
- `FluentAssertions.Resultful` stays in this repository until it is deprecated on NuGet; it gets no new
  assertions.

Why:
- [Resultful decision 18](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0018-resultful-assertions-live-here.md)
  moved the assertions for Resultful's own types (`Resultful.Assertions` and its AwesomeAssertions and
  FluentAssertions adapters) into Resultful/Resultful. A repository called `Resultful.Assertions` that doesn't ship
  the `Resultful.Assertions` package would mislead.
- What is left is about unions. Its most-used package, `FluentAssertions.OneOf`, has nothing to do with Resultful.
