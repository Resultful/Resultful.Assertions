---
type: decision
title: "A library-independent core plus one adapter per assertion library"
description: "New Resultful assertions ship as Resultful.Assertions (no assertion library) plus Resultful.Assertions.<Library> adapters, with each adapter referencing the lowest library version it supports."
status: proposed
tags: [union-assertions, decision, packages, licensing]
decided:
  by: ""
  at: 2026-11-20
generated:
  by: claude-opus-5.5
  at: 2026-11-20
---

# 3. A library-independent core plus one adapter per assertion library

> Superseded by [Resultful decision 18](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0018-resultful-assertions-live-here.md):
> `Resultful.Assertions` and its adapters are built in Resultful/Resultful, against the in-repo core, with this
> shape. The pattern can still extend to the OneOf and union packages here.

Proposed. The shape for new Resultful assertion packages:

- **`Resultful.Assertions`**: `ShouldSucceed()`, `ShouldFail()` and `ShouldFailWith(...)` on Resultful's types.
  It throws an exception that every test framework reports as a failure, and depends only on `Resultful`.
- **`Resultful.Assertions.<Library>`** adapters add each assertion library's native style:
  `Resultful.Assertions.FluentAssertions` and `Resultful.Assertions.AwesomeAssertions` first, then Shouldly and
  TUnit where their extension models fit.
- An adapter depends on the core, on `Resultful` and on one assertion library, never on two.
- **Licensing.** FluentAssertions 8 and later is commercial (Xceed licence); 7.x and AwesomeAssertions are not.
  An adapter references the lowest version of its library that it supports, so the package never forces a
  licence on a consumer; a consumer who resolves FluentAssertions 8 or later carries that licence. The existing
  FluentAssertions packages build against 7.x for the same reason, and Dependabot doesn't propose major
  FluentAssertions upgrades.

Still to check: which FluentAssertions range the adapter can support, and whether 8 changed the extension points
it needs.

The OneOf and union packages keep their current shape ([decision 2](0002-package-ids-are-kept.md)). The same
pattern can extend to them later (for example AwesomeAssertions packages for OneOf), but nothing forces it.

Why:
- One package referencing every assertion library would force all of them on every consumer.
- A core with no assertion library is usable from any test framework, and gives the adapters one implementation of
  the checks and the failure messages.

Depends on `Resultful 0.2.0-beta06` being published (see the [roadmap](../roadmap.md)).
