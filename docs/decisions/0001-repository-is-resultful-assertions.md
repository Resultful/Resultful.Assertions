---
type: decision
title: "The repository is Resultful.Assertions"
description: "FluentAssertions.Resultful is renamed Resultful.Assertions: the home for assertion packages for result, option and union types across assertion libraries."
status: stable
tags: [union-assertions, decision]
decided:
  by: human:rtkelly13
  at: 2026-11-20
generated:
  by: claude-opus-5.5
  at: 2026-11-20
---

# 1. The repository is `Resultful.Assertions`

> Superseded by [decision 5](0005-repository-is-union-assertions.md): the repository is renamed
> `Resultful/Union.Assertions`, because the assertions for Resultful's own types moved to Resultful/Resultful.

The maintainer chose the name. `Resultful/FluentAssertions.Resultful` is renamed `Resultful/Resultful.Assertions`
(#10). It holds many assertion projects: assertions for result, option and union types (OneOf, Resultful and
others), in every assertion library where that is worthwhile (FluentAssertions, AwesomeAssertions, Shouldly,
TUnit and so on). Resultful assertions are one part of it.

- The solution is `Resultful.Assertions.slnx`.
- GitHub redirects the old repository URLs, so links and clones keep working; clones should still update their
  remote.
- The local clone moves from `~/code/Resultful/FluentAssertions.Resultful` to `~/code/Resultful/Resultful.Assertions`,
  and the estate registry (rig) and Orca follow.

Why:
- The old name described one package. The repository's most-used package, `FluentAssertions.OneOf`, has nothing to
  do with Resultful, and the plans cover libraries other than FluentAssertions.
- It sits beside `Resultful/Resultful` because the packages share an organisation and a maintainer, not because
  every package depends on Resultful.

Related: [Resultful decision 12](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0012-assertions-live-in-resultful-assertions.md).
