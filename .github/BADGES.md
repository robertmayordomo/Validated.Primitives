# GitHub Actions Status Badges

Add these badges to your README.md to show build status:

## Build Status Badge

### Simple Workflow
```markdown
[![Build & Publish](https://github.com/robertmayordomo/Validated.PrimitivesV1/actions/workflows/build-test-publish.yml/badge.svg)](https://github.com/robertmayordomo/Validated.PrimitivesV1/actions/workflows/build-test-publish.yml)
```

### GitVersion Workflow
```markdown
[![Build & Publish](https://github.com/robertmayordomo/Validated.PrimitivesV1/actions/workflows/build-test-publish-gitversion.yml/badge.svg)](https://github.com/robertmayordomo/Validated.PrimitivesV1/actions/workflows/build-test-publish-gitversion.yml)
```

## NuGet Package Badge

```markdown
[![NuGet](https://img.shields.io/nuget/v/Validated.Primitives.svg)](https://www.nuget.org/packages/Validated.Primitives/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Validated.Primitives.svg)](https://www.nuget.org/packages/Validated.Primitives/)
```

## Combined Example

Add this to the top of your README.md:

```markdown
# Validated Primitives

[![Build & Publish](https://github.com/robertmayordomo/Validated.PrimitivesV1/actions/workflows/build-test-publish.yml/badge.svg)](https://github.com/robertmayordomo/Validated.PrimitivesV1/actions/workflows/build-test-publish.yml)
[![NuGet](https://img.shields.io/nuget/v/Validated.Primitives.svg)](https://www.nuget.org/packages/Validated.Primitives/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Validated.Primitives.svg)](https://www.nuget.org/packages/Validated.Primitives/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET library that provides strongly-typed, self-validating primitive value objects...
```

## Result

The badges will look like this:

![Build & Publish](https://img.shields.io/badge/build-passing-brightgreen)
![NuGet](https://img.shields.io/badge/nuget-v1.0.0-blue)
![Downloads](https://img.shields.io/badge/downloads-1.2k-blue)
![License](https://img.shields.io/badge/License-MIT-yellow)
