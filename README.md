# Union.Assertions

[![ci](https://github.com/Resultful/Union.Assertions/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Resultful/Union.Assertions/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Assertions for OneOf and other union types, in the assertion library your tests already use. Checking which case
a union holds, and the value in it, should be as easy as asserting on anything else.

This repository was called `Resultful.Assertions`, and before that `FluentAssertions.Resultful`. GitHub redirects
the old URLs, and every package keeps its ID. Assertions for [Resultful](https://github.com/Resultful/Resultful)'s
own types (`Resultful.Assertions` and its adapters) are built in the Resultful repository.

## Packages

| Package | NuGet | What it asserts on |
|---|---|---|
| `FluentAssertions.OneOf` | [![NuGet](https://img.shields.io/nuget/v/FluentAssertions.OneOf.svg)](https://www.nuget.org/packages/FluentAssertions.OneOf/) | [OneOf](https://github.com/mcintyre321/OneOf) unions, with FluentAssertions |
| `FluentAssertions.OneOf.Extended` | [![NuGet](https://img.shields.io/nuget/v/FluentAssertions.OneOf.Extended.svg)](https://www.nuget.org/packages/FluentAssertions.OneOf.Extended/) | `OneOf.Extended` (9 to 32 cases), with FluentAssertions |
| `FluentAssertions.Union` | [![NuGet](https://img.shields.io/nuget/v/FluentAssertions.Union.svg)](https://www.nuget.org/packages/FluentAssertions.Union/) | Any union-like type (anything with a `Match`/`Switch`), with FluentAssertions; the base the others build on |

More assertion libraries (AwesomeAssertions, Shouldly, TUnit) for OneOf and unions are planned; see the [roadmap](docs/roadmap.md).

The packages build against FluentAssertions 7.x. FluentAssertions 8 and later is commercially licensed, so a
project that resolves 8 or later carries that licence.

## Usage

### OneOf

```cs
var testValue = OneOf<int, string, DateTime>.FromT1("Test Value");

testValue.Should().BeCase<string>()
    .And.Should().Be("Test Value");
```

`BeCase<T>()` asserts which case the `OneOf` holds and, if it matches, returns the value for further assertions.
The type passed in can be anything, even a type the `OneOf` can't hold; a wrong type fails the test, so the
mistake shows up quickly.

### Any union type

```cs
option.ShouldBeCase<int>()
    .And.Should().Be(3);

option.ShouldBeNone();
```

### Resultful

`FluentAssertions.Resultful` was built here until it was removed ([decision 6](docs/decisions/0006-fluentassertions-resultful-is-removed.md)).
Its last version, `0.0.6-beta01`, stays on NuGet and is to be deprecated in favour of
`Resultful.Assertions.FluentAssertions`. Success and failure assertions for Resultful (`ShouldSucceed`,
`ShouldFail`) are being built as `Resultful.Assertions` and its adapters in
[Resultful/Resultful](https://github.com/Resultful/Resultful) ([its decision 18](https://github.com/Resultful/Resultful/blob/main/docs/decisions/0018-resultful-assertions-live-here.md)).

## Documentation

- [Vision](docs/vision.md): what this repository is for
- [Roadmap](docs/roadmap.md) and [decisions](docs/decisions/index.md)

## Building

```sh
dotnet build Union.Assertions.slnx
dotnet test Union.Assertions.slnx
```
