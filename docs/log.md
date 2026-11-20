---
type: log
title: "Log"
description: "What changed in the Resultful.Assertions documents, by date."
status: stable
tags: [resultful-assertions, log]
generated:
  by: claude-opus-5.5
  at: 2026-11-20
---

# Log

## 2026-11-20

- Documents started as an OKF bundle, following Resultful's: vision, roadmap, decisions, log.
- The repository was renamed from `FluentAssertions.Resultful` to `Resultful.Assertions` (#10), after publishing
  on push was removed from CI (#9).
- Decisions 1 and 2 (the rename; package IDs are kept) recorded as the maintainer's. Decisions 3 (a
  library-independent core plus adapters) and 4 (publish only what changed) proposed.
- Known issue recorded: `FluentAssertions.Resultful` waits on `Resultful 0.2.0-beta06` for the NU1903 fix.
