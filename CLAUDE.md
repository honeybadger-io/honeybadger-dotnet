# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Structure

This is the official .NET Honeybadger client library, organized as a multi-target solution with three main packages:

- **Honeybadger** (`src/Honeybadger/`) - Core library with `HoneybadgerClient` and error reporting functionality
- **Honeybadger.DotNetCore** (`src/Honeybadger.DotNetCore/`) - ASP.NET Core integration with middleware and DI extensions
- **Honeybadger.Extensions.Logging** (`src/Honeybadger.Extensions.Logging/`) - Microsoft.Extensions.Logging provider

## Build and Test Commands

### Development workflow:
```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build --configuration Release --no-restore

# Run all tests
dotnet test --configuration Release --no-build --no-restore

# Run specific test project
dotnet test tests/Honeybadger.Tests --configuration Release --no-build --no-restore
```

### Packaging (manual release process):
```bash
# Version bump (updates all projects)
dotnet versionize

# Pack individual projects
dotnet pack ./src/Honeybadger --configuration Release
dotnet pack ./src/Honeybadger.Extensions.Logging --configuration Release
dotnet pack ./src/Honeybadger.DotNetCore --configuration Release
```

## Key Architecture Patterns

### Core Components:
- **IHoneybadgerClient** - Main interface for error reporting and context management
- **HoneybadgerClient** - Core implementation with HTTP client and threading support
- **HoneybadgerDotNetClient** - ASP.NET Core-specific client with HttpContext integration
- **NoticeFactory** - Creates Notice objects from exceptions and messages
- **RequestFactory** - Extracts request data from HttpContext

### Threading Model:
- Uses `ThreadLocal<T>` for context and breadcrumbs storage
- Context is per-thread to avoid cross-request contamination
- Supports both sync and async notification methods

### Configuration:
- Primary config via `HoneybadgerOptions` class
- ASP.NET Core apps use `appsettings.json` "Honeybadger" section
- Manual initialization for console apps

### Middleware Integration:
- `HoneybadgerStartupFilter` registers middleware automatically
- `HoneybadgerMiddleware` catches unhandled exceptions
- Configurable via `ReportUnhandledExceptions` option

## Multi-Target Support

Projects target multiple .NET versions:
- Tests: `net8.0;net9.0`
- Libraries: Support modern .NET Core versions up to .NET 9.0

## Release Process

**Important**: The release process is manual due to dependency ordering. `Honeybadger` must be released first, then the dependent packages (`Honeybadger.Extensions.Logging` and `Honeybadger.DotNetCore`) must be updated with new version references before their release.

## Development Environment Setup

- Solution file: `honeybadger-dotnet.sln`
- Test framework: xUnit with Moq for mocking
- CI/CD: GitHub Actions (`.github/workflows/ci.yml`)
- Examples available in `examples/` directory for each integration pattern