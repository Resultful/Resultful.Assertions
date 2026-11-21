---
type: decision
title: "Existing package IDs are kept"
description: "FluentAssertions.OneOf, FluentAssertions.Union, FluentAssertions.OneOf.Extended and FluentAssertions.Resultful keep their NuGet IDs through the renames."
status: stable
tags: [union-assertions, decision, packages]
decided:
  by: human:rtkelly13
  at: 2026-11-20
generated:
  by: claude-opus-5.5
  at: 2026-11-20
---

# 2. Existing package IDs are kept

The repository's name and its package IDs are independent. Renaming the repository renames no package and
deprecates none. This held for both renames: to `Resultful.Assertions` ([decision 1](0001-repository-is-resultful-assertions.md))
and to `Union.Assertions` ([decision 5](0005-repository-is-union-assertions.md)).

| Package | Stays | Notes |
|---|---|---|
| `FluentAssertions.OneOf` | yes | About 388,000 downloads, by far the most used package in the family, and mostly used without Resultful. |
| `FluentAssertions.OneOf.Extended` | yes | |
| `FluentAssertions.Union` | yes | The base the OneOf packages build on. |
| `FluentAssertions.Resultful` | yes, until deprecated | Removed from this repository ([decision 6](0006-fluentassertions-resultful-is-removed.md)); the ID and its published versions stay on NuGet. To be deprecated in favour of `Resultful.Assertions.FluentAssertions`, which is built in Resultful/Resultful ([Resultful decision 18](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0018-resultful-assertions-live-here.md)). That decision, not either rename, deprecates it. |

Why:
- A package ID is a contract with every project that references it. Changing it forces every user to edit their
  project for no gain.
- None of the OneOf or union packages depends on Resultful, and their users shouldn't have to learn about it.

New Resultful assertion packages follow the naming in [Resultful decision 18](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0018-resultful-assertions-live-here.md); new union packages keep the `<Library>.<Type>` naming used here.
