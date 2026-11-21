---
type: log
title: "Log"
description: "What changed in the Union.Assertions documents, by date."
status: stable
tags: [union-assertions, log]
generated:
  by: claude-opus-5.5
  at: 2026-11-20
---

# Log

## 2026-11-21

- The repository is renamed from `Resultful.Assertions` to `Union.Assertions` (decision 5, superseding decision 1),
  following [Resultful decision 18](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0018-resultful-assertions-live-here.md),
  which moved the assertions for Resultful's own types into Resultful/Resultful. Package IDs are unchanged
  (decision 2).
- Decision 3 (a library-independent core plus adapters) is superseded by Resultful decision 18. The roadmap's R2
  and R3 now point there; R6 (remove `FluentAssertions.Resultful` once it is deprecated) is added.
- The vision is narrowed to OneOf and union types.
- `FluentAssertions.Resultful` is removed from the repository (decision 6, R6), with its tests, the `Resultful`
  package version and the package check's `Resultful` floor. The NU1903 known issue goes with it. Its last
  published version stays on NuGet.

## 2026-11-20

- Documents started as an OKF bundle, following Resultful's: vision, roadmap, decisions, log.
- The repository was renamed from `FluentAssertions.Resultful` to `Resultful.Assertions` (#10), after publishing
  on push was removed from CI (#9).
- Decisions 1 and 2 (the rename; package IDs are kept) recorded as the maintainer's. Decisions 3 (a
  library-independent core plus adapters) and 4 (publish only what changed) proposed.
- Known issue recorded: `FluentAssertions.Resultful` waits on `Resultful 0.2.0-beta06` for the NU1903 fix.
- Publishing only what changed implemented (decision 4): `eng/release-plan.cs`, `eng/verify-package.cs` and a
  manual publish job. R1 is now the first tagged release.
