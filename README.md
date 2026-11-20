# Resultful.Assertions

[![ci](https://github.com/Resultful/Resultful.Assertions/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Resultful/Resultful.Assertions/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Assertions for result, option and union types, in the assertion library your tests already use. A failure that
is a value (a `Result`, a `OneOf`, an option) should be as easy to assert on as an exception.

This repository was called `FluentAssertions.Resultful`. GitHub redirects the old URLs, and every package keeps
its ID.

## Packages

| Package | NuGet | What it asserts on |
|---|---|---|
| `FluentAssertions.OneOf` | [![NuGet](https://img.shields.io/nuget/v/FluentAssertions.OneOf.svg)](https://www.nuget.org/packages/FluentAssertions.OneOf/) | [OneOf](https://github.com/mcintyre321/OneOf) unions, with FluentAssertions |
| `FluentAssertions.OneOf.Extended` | [![NuGet](https://img.shields.io/nuget/v/FluentAssertions.OneOf.Extended.svg)](https://www.nuget.org/packages/FluentAssertions.OneOf.Extended/) | `OneOf.Extended` (9 to 32 cases), with FluentAssertions |
| `FluentAssertions.Union` | [![NuGet](https://img.shields.io/nuget/v/FluentAssertions.Union.svg)](https://www.nuget.org/packages/FluentAssertions.Union/) | Any union-like type (anything with a `Match`/`Switch`), with FluentAssertions; the base the others build on |
| `FluentAssertions.Resultful` | [![NuGet](https://img.shields.io/nuget/v/FluentAssertions.Resultful.svg)](https://www.nuget.org/packages/FluentAssertions.Resultful/) | [Resultful](https://github.com/Resultful/Resultful) results, with FluentAssertions |

More assertion libraries (AwesomeAssertions, Shouldly, TUnit) are planned; see the [roadmap](docs/roadmap.md).

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

`FluentAssertions.Resultful` currently gives `result.Should()` access to the subject only. Success and failure
assertions (`ShouldSucceed`, `ShouldFail`) are planned; see the [roadmap](docs/roadmap.md).

## Documentation

- [Vision](docs/vision.md): what this repository is for
- [Roadmap](docs/roadmap.md) and [decisions](docs/decisions/index.md)

## Building

```sh
dotnet build Resultful.Assertions.slnx
dotnet test Resultful.Assertions.slnx
```
