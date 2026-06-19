# Release Checklist

Use this checklist before publishing a package to NuGet. NuGet package versions
are immutable, so complete these checks before pushing any package.

## Pre-Publish Checks

- Confirm `Directory.Build.props` contains the intended package version.
- Confirm the version has not already been published for any package ID:
  - `ISOCodex.Addressing`
  - `ISOCodex.Addressing.Brazil`
  - `ISOCodex.Addressing.GreatBritain`
  - `ISOCodex.Addressing.UnitedStates`
  - `ISOCodex.Addressing.Canada`
  - `ISOCodex.Addressing.Germany`
  - `ISOCodex.Addressing.India`
  - `ISOCodex.Addressing.Italy`
  - `ISOCodex.Addressing.Mexico`
  - `ISOCodex.Addressing.Spain`
  - `ISOCodex.Addressing.Ireland`
  - `ISOCodex.Addressing.France`
- Commit and push the version change before creating the final packages.
- Confirm the working tree is clean and aligned with `origin/main`.

## Package Verification

Run the release check from the repository root after the release commit has been
pushed:

```powershell
pwsh ./scripts/release-check.ps1
```

The script performs:

- clean `Release` outputs
- restore
- Release build
- Release tests
- solution pack
- package count/name inspection
- package content inspection for readme, nuspec, and `netstandard2.1` library output

The generated packages are written to `artifacts/release-check`, which is
ignored by Git.

Before publishing, inspect the generated nuspec metadata and confirm:

- all packages use the intended version
- all packages reference the pushed release commit
- every country package depends on the same version of `ISOCodex.Addressing`
  as the published core package

## Publish Order

Publish the core package first:

```powershell
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
```

Then publish the country packages:

```powershell
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.GreatBritain.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.UnitedStates.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.Canada.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.Spain.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.Ireland.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.France.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.India.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.Brazil.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.Mexico.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.Germany.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
dotnet nuget push artifacts/release-check/ISOCodex.Addressing.Italy.<version>.nupkg --source https://api.nuget.org/v3/index.json --api-key <api-key>
```

After publishing, confirm every package ID lists the new version on NuGet.
