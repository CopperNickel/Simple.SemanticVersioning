# Simple.SemanticVersioning

A lightweight .NET library for parsing, creating, and comparing semantic versions according to the [Semantic Versioning 2.0.0](https://semver.org/) specification.

## Features

- **Full Semantic Versioning Support**: Comprehensive implementation of semver.org specification
- **Flexible Version Parsing**: Parse version strings with Major, Minor, Patch, Revision, Prerelease, and Metadata components
- **Version Comparison**: Built-in comparison operators and IComparable support for easy version ordering
- **Prerelease Rank Detection**: Automatic recognition of common prerelease suffixes (alpha, beta, rc, dev, etc.)
- **JSON Serialization**: Native JSON support for serializing/deserializing semantic versions
- **.NET 10 Ready**: Built on .NET 10 with implicit usings and nullable reference types enabled
- **Zero Dependencies**: No external dependencies required

## Installation

Add the NuGet package to your project:

```bash
dotnet add package Simple.SemanticVersioning
```

Or via Package Manager:

```
Install-Package Simple.SemanticVersioning
```

## Quick Start

### Creating a Version

```csharp
using Simple.SemanticVersioning;

// Create from components
var version = new SemanticVersion(
    parts: new[] { 1, 2, 3 },
    prerelease: "alpha.1",
    metadata: "build.123"
);

// Create simple versions
var v1 = new SemanticVersion(1, 0, 0);
var v2 = new SemanticVersion(2, 5);

// Access version components
Console.WriteLine(version.Major);        // 1
Console.WriteLine(version.Minor);        // 2
Console.WriteLine(version.Patch);        // 3
Console.WriteLine(version[3]);           // 0 (Revision - if not set)
Console.WriteLine(version.Prerelease);   // alpha.1
Console.WriteLine(version.Metadata);     // build.123
```

### Parsing Versions

```csharp
// Parse from string
if (SemanticVersion.TryParse("1.2.3-alpha.1+build.123", out var version))
{
    Console.WriteLine(version.Major);  // 1
    Console.WriteLine(version.IsRelease); // false
}

// Using span parsing
Span<char> versionSpan = "2.0.0-rc.1".AsSpan();
if (SemanticVersion.TryParseSpan(versionSpan, out var version))
{
    Console.WriteLine(version.ToString());
}
```

### Comparing Versions

```csharp
var v1 = new SemanticVersion(1, 0, 0);
var v2 = new SemanticVersion([2, 0, 0], "beta");
var v3 = new SemanticVersion(2, 0, 0);

// Using comparison operators
if (v1 < v2) 
{
    Console.WriteLine("1.0.0 is less than 2.0.0-beta");
}

if (v2 < v3)
{
    Console.WriteLine("2.0.0-beta is less than 2.0.0 (release)");
}

// Using CompareTo
int result = v1.CompareTo(v3);  // -1 (v1 is less than v3)
```

### Prerelease Ranks

The library automatically recognizes and ranks common prerelease identifiers:

```csharp
var version = new SemanticVersion(new[] { 1L, 0L, 0L }, "alpha.1", null);

Console.WriteLine(version.Rank.Name);        // Alpha. Early, unstable, incomplete features
Console.WriteLine(version.Rank.Suffix);     // a or alpha
Console.WriteLine(version.Rank.Level);      // -4 (lower levels = earlier in development)
Console.WriteLine(version.IsRelease);       // false
```

**Supported Prerelease Ranks** (from most stable to least):
- **Development**: `dev`, `nightly`, `snapshot` (Level: -5)
- **Experimental**: `exp` (Level: -2)
- **Canary**: `canary` (Level: -6)
- **Test/QA**: `test`, `qa` (Level: -6)
- **Alpha**: `a`, `alpha` (Level: -4)
- **Milestone**: `m` (Level: -3)
- **Beta**: `b`, `beta` (Level: -3)
- **Preview**: `preview` (Level: -2)
- **Build**: `build` (Level: -2)
- **Prerelease**: `pre` (Level: -2)
- **Release Candidate**: `rc` (Level: -1)
- **Early Access**: `ea` (Level: -1)
- **Hotfix**: `hotfix` (Level: -1)
- **Next**: `next` (Level: -1)
- **Final**: `final` (Level: 0)
- **Release**: (empty or no suffix) (Level: 0)

### JSON Support

```csharp
using System.Text.Json;

var version = new SemanticVersion(new[] { 1L, 2L, 3L }, "alpha", null);

// Serialize to JSON
string json = JsonSerializer.Serialize(version);
// Result: "1.2.3-alpha"

// Deserialize from JSON
var deserialized = JsonSerializer.Deserialize<SemanticVersion>(@"""1.2.3-alpha""");
```

### Version Parts

The library supports multiple version components:

```csharp
var version = new SemanticVersion(1, 2, 3, 4);

Console.WriteLine(version[0]);  // Major: 1
Console.WriteLine(version[1]);  // Minor: 2
Console.WriteLine(version[2]);  // Patch: 3
Console.WriteLine(version[3]);  // Revision: 4
Console.WriteLine(version.Parts);  // [1, 2, 3, 4]
```

> **Note**: Rightmost zero values are automatically trimmed from the Parts collection.

## API Reference

### SemanticVersion Class

#### Properties
- `Major` (long): Major version number
- `Minor` (long): Minor version number
- `Patch` (long): Patch version number
- `Revision` (long): Revision number (4th component)
- `Parts` (IReadOnlyList<long>): All version parts
- `Prerelease` (string): Prerelease identifier
- `Metadata` (string): Build metadata
- `IsRelease` (bool): Whether this is a stable release
- `Rank` (SemanticVersionPrereleaseRank): Detected prerelease rank

#### Methods
- `TryParse(string?, SemanticVersion?)`: Try to parse a version string
- `TryParseSpan(ReadOnlySpan<char>, SemanticVersion?)`: Try to parse from a span
- `ToString()`: Get string representation
- `ToString(string?, IFormatProvider?)`: Get formatted string representation
- `Equals(SemanticVersion?)`: Check equality
- `CompareTo(SemanticVersion?)`: Compare with another version
- `operator ==`, `operator !=`, `operator <`, `operator >`, `operator <=`, `operator >=`: Comparison operators

### SemanticVersionPrereleaseRank Class

#### Properties
- `Suffix` (string): The prerelease suffix (e.g., "alpha", "a", "beta")
- `Name` (string): Human-readable name and description
- `Level` (int): Stability level (higher = more stable)
- `IsRelease` (bool): Whether this rank represents a release version

#### Static Methods
- `Find(string?)`: Find the rank for a given prerelease string

## Comparison Logic

The library follows semantic versioning rules for comparison:

1. **Version Numbers**: 1.0.0 < 1.1.0 < 2.0.0
2. **Prerelease Precedence**: 1.0.0-alpha < 1.0.0-beta < 1.0.0-rc < 1.0.0
3. **Prerelease Level**: Versions with lower prerelease levels are considered less stable
4. **Metadata**: Build metadata does not affect version precedence (ignored in comparison)

## Building from Source

### Prerequisites
- .NET 10 SDK or later

### Build

```bash
dotnet build Simple.SemanticVersioning.sln
```

### Run Tests

```bash
dotnet test Simple.SemanticVersioning.sln
```

## Project Structure

- `Simple.SemanticVersioning/` - Main library implementation
  - `SemanticVersioning.SemanticVersion.cs` - Core SemanticVersion class
  - `SemanticVersioning.SemanticVersionPrereleaseRank.cs` - Prerelease rank definitions
  - `SemanticVersioning.SemanticVersioningComparer.cs` - Comparison logic
  - `SemanticVersioning.SemanticVersionJsonConverter.cs` - JSON serialization support

- `Simple.SemanticVersioning.Test/` - Comprehensive unit tests

## License

Copyright (c) 2026 Dmitrii Bychenko

## Repository

[GitHub - CopperNickel/Simple.SemanticVersioning](https://github.com/CopperNickel/Simple.SemanticVersioning)

## Semantic Versioning Reference

For more information about semantic versioning, visit [semver.org](https://semver.org/)

**Format**: `MAJOR.MINOR.PATCH[-PRERELEASE][+METADATA]`

- **MAJOR**: Incremented for incompatible API changes
- **MINOR**: Incremented for backward-compatible functionality additions
- **PATCH**: Incremented for backward-compatible bug fixes
- **PRERELEASE**: Optional pre-release identifier
- **METADATA**: Optional build metadata (does not affect version precedence)

## Examples

### Example 1: Version Comparison in an Application

```csharp
public class VersionChecker
{
    public static void CheckUpdate(string installedVersion, string latestVersion)
    {
        if (SemanticVersion.TryParse(installedVersion, out var current) &&
            SemanticVersion.TryParse(latestVersion, out var latest))
        {
            if (current < latest)
            {
                Console.WriteLine($"Update available: {current} -> {latest}");
            }
            else
            {
                Console.WriteLine("Using the latest version");
            }
        }
    }
}
```

### Example 2: Version-Aware Feature Flags

```csharp
public class FeatureManager
{
    private static readonly SemanticVersion FeatureIntroduced = 
        new(new[] { 2L, 1L, 0L }, null, null);

    public bool IsFeatureAvailable(string currentVersion)
    {
        if (SemanticVersion.TryParse(currentVersion, out var version))
        {
            return version >= FeatureIntroduced;
        }
        return false;
    }
}
```

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests to the repository.