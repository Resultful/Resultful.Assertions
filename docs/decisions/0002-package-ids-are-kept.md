---
type: decision
title: "Existing package IDs are kept"
description: "FluentAssertions.OneOf, FluentAssertions.Union, FluentAssertions.OneOf.Extended and FluentAssertions.Resultful keep their NuGet IDs through the rename."
status: stable
tags: [resultful-assertions, decision, packages]
decided:
  by: human:rtkelly13
  at: 2026-11-20
generated:
  by: claude-opus-5.5
  at: 2026-11-20
---

# 2. Existing package IDs are kept

The repository's name and its package IDs are independent. Renaming the repository renames no package and
deprecates none.

| Package | Stays | Notes |
|---|---|---|
| `FluentAssertions.OneOf` | yes | About 388,000 downloads, by far the most used package in the family, and mostly used without Resultful. |
| `FluentAssertions.OneOf.Extended` | yes | |
| `FluentAssertions.Union` | yes | The base the OneOf packages build on. |
| `FluentAssertions.Resultful` | yes, for now | May later be deprecated in favour of `Resultful.Assertions.FluentAssertions` ([decision 3](0003-core-plus-adapters.md)); that is a separate decision, not part of the rename. |

Why:
- A package ID is a contract with every project that references it. Changing it forces every user to edit their
  project for no gain.
- None of the OneOf or union packages depends on Resultful, and their users shouldn't have to learn about it.

New packages follow the naming in [decision 3](0003-core-plus-adapters.md).
