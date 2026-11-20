---
type: vision
title: "Resultful.Assertions: Vision"
description: "What Resultful.Assertions is for: making failure-as-a-value and union types easy to assert on, in whichever assertion library a team uses."
status: draft
tags: [resultful-assertions, vision]
generated:
  by: claude-opus-5.5
  at: 2026-11-20
---

# Resultful.Assertions: Vision

> Draft. The specifics are in [the roadmap](roadmap.md) and [the decisions](decisions/index.md).

## Intention

When failure is a value rather than an exception, the tests have to check that value. Did the call succeed? Which
case does the union hold? What is the error? Without help, a test unwraps the value by hand: a `Match` with a
lambda that fails the test in the branch it didn't expect, or a cast followed by an assertion. The failure message
says that a lambda threw, not that the result was a failure holding `"name is required"`.

Resultful.Assertions exists so that asserting on a result, an option or a union reads as plainly as asserting on
anything else, and fails with a message that says what was expected and what was found. It meets teams in the
assertion library they already use, rather than asking them to adopt another one.

## What it is

- **Assertions for failure-as-a-value types.** Results (Resultful and others), options, and unions such as
  OneOf: which case is held, the value in it, success and failure.
- **In every assertion library where it fits.** FluentAssertions today; AwesomeAssertions, Shouldly, TUnit and
  others where their extension model makes a native-feeling package worthwhile. Each package reads like the
  library it extends.
- **Small packages with narrow dependencies.** A package depends on the type library it asserts on and on one
  assertion library. Nobody takes an assertion library they don't use, or a type library they don't use, to get
  the assertions they want.
- **Stable for the people already using it.** Existing package IDs keep working. `FluentAssertions.OneOf` is used
  by far more projects than the rest of the family combined, most of them without Resultful.

## What it is not

- **Not an assertion library.** It extends assertion libraries; it doesn't compete with them.
- **Not tied to Resultful.** Resultful is one of the types it asserts on, not a requirement. The OneOf and union
  packages never depend on it.
- **Not a test framework adapter.** Packages that are independent of an assertion library throw an exception that
  every test framework already reports as a failure.

## Who it is for

.NET teams whose code returns results, options or unions, and who want their tests to say what they mean in the
assertion style they already write.

## Principles

- **The failure message is the product.** An assertion that fails should say what was expected, what was found,
  and the value inside it.
- **Native to each library.** `result.Should().BeSuccess()` in FluentAssertions, `result.ShouldSucceed()` in
  Shouldly: whatever the library's own idiom is.
- **Dependencies point one way.** Assertion packages depend on type libraries and assertion libraries; nothing
  depends on them.
- **Licences are visible.** Where an assertion library's licence changes between versions (FluentAssertions 8 is
  commercial), the supported range is a deliberate decision and is documented.
- **Release only what changed.** Each package is versioned and released on its own, so a fix in one doesn't
  republish the others.

## What success looks like

- A test asserting on a result, option or union is one line, in any supported assertion library.
- Failure messages show the case and the value without a debugger.
- Existing users of `FluentAssertions.OneOf` upgrade without changing their code.
- A release publishes only the packages whose code changed.
