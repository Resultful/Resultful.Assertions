---
type: vision
title: "Union.Assertions: Vision"
description: "What Union.Assertions is for: making OneOf and other union types easy to assert on, in whichever assertion library a team uses."
status: draft
tags: [union-assertions, vision]
generated:
  by: claude-opus-5.5
  at: 2026-11-20
---

# Union.Assertions: Vision

> Draft. The specifics are in [the roadmap](roadmap.md) and [the decisions](decisions/index.md).

## Intention

When a value is a union, the tests have to check which case it holds. Is it the `string` or the `int`? Is the
option empty? What is the error? Without help, a test unwraps the value by hand: a `Match` with a lambda that fails
the test in the branch it didn't expect, or a cast followed by an assertion. The failure message says that a
lambda threw, not that the union held a `NotFound` with `"user 42"`.

Union.Assertions exists so that asserting on a union reads as plainly as asserting on anything else, and fails
with a message that says what was expected and what was found. It meets teams in the assertion library they
already use, rather than asking them to adopt another one.

Assertions for [Resultful](https://github.com/Resultful/Resultful)'s own types live in the Resultful repository
([its decision 18](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0018-resultful-assertions-live-here.md)).

## What it is

- **Assertions for union types.** OneOf, OneOf.Extended, and any union-like type with a `Match` or `Switch`
  (options and results from other libraries included): which case is held and the value in it.
- **In every assertion library where it fits.** FluentAssertions today; AwesomeAssertions, Shouldly, TUnit and
  others where their extension model makes a native-feeling package worthwhile. Each package reads like the
  library it extends.
- **Small packages with narrow dependencies.** A package depends on the type library it asserts on and on one
  assertion library. Nobody takes an assertion library they don't use, or a type library they don't use, to get
  the assertions they want.
- **Stable for the people already using it.** Existing package IDs keep working through every rename.
  `FluentAssertions.OneOf` is used by far more projects than the rest of the family combined, most of them
  without Resultful.

## What it is not

- **Not an assertion library.** It extends assertion libraries; it doesn't compete with them.
- **Not where Resultful's assertions live.** Those are built beside Resultful's core. The OneOf and union
  packages never depend on Resultful. `FluentAssertions.Resultful` is still built here until it is deprecated on
  NuGet.
- **Not a test framework adapter.** Each package extends an assertion library, and the assertion library reports
  failures to the test framework.

## Who it is for

.NET teams whose code returns OneOf or other unions, and who want their tests to say what they mean in the
assertion style they already write.

## Principles

- **The failure message is the product.** An assertion that fails should say what was expected, what was found,
  and the value inside it.
- **Native to each library.** `union.Should().BeCase<string>()` in FluentAssertions, `union.ShouldBeCase<string>()`
  in Shouldly: whatever the library's own idiom is.
- **Dependencies point one way.** Assertion packages depend on type libraries and assertion libraries; nothing
  depends on them.
- **Licences are visible.** Where an assertion library's licence changes between versions (FluentAssertions 8 is
  commercial), the supported range is a deliberate decision and is documented.
- **Release only what changed.** Each package is versioned and released on its own, so a fix in one doesn't
  republish the others.

## What success looks like

- A test asserting on a union is one line, in any supported assertion library.
- Failure messages show the case and the value without a debugger.
- Existing users of `FluentAssertions.OneOf` upgrade without changing their code.
- A release publishes only the packages whose code changed.
