---
type: decision
title: "FluentAssertions.Resultful is removed from this repository"
description: "The FluentAssertions.Resultful project and its tests are deleted now, rather than after its NuGet deprecation; its history stays in this repository. Changes decision 5."
status: stable
tags: [union-assertions, decision, packages]
decided:
  by: human:rtkelly13
  at: 2026-11-21
generated:
  by: claude-opus-5.5
  at: 2026-11-21
---

# 6. `FluentAssertions.Resultful` is removed from this repository

The maintainer chose to remove it now. This changes one point of [decision 5](0005-repository-is-union-assertions.md),
which kept the project here until its NuGet deprecation.

- `src/FluentAssertions.Resultful` and `tests/FluentAssertions.Resultful.Tests` are deleted, with the `Resultful`
  entry in `Directory.Packages.props` and the `Resultful` floor check in `eng/verify-package.cs`.
- Nothing in this repository depends on Resultful any more.
- The package ID and its published versions (last `0.0.6-beta01`) stay on NuGet. Deprecating it in favour of
  `Resultful.Assertions.FluentAssertions` is Resultful/Resultful's job
  ([Resultful decision 18](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0018-resultful-assertions-live-here.md)).
  The ID isn't reused ([decision 2](0002-package-ids-are-kept.md)).
- Its source and history stay in this repository's git history, for Resultful/Resultful to consult.

Why:
- It contained no assertions (only `result.Should()` returning the subject), so nothing is lost.
- It would never be released from here again: the replacement is built against Resultful's in-repo core.
- Keeping it carried a vulnerable dependency chain (`Resultful 0.2.0-beta05`, NU1903) and a special case in the
  package check, for a package with no future here.
