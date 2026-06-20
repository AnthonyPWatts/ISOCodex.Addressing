param(
    [string] $Configuration = "Release",
    [string] $OutputDirectory = "artifacts/release-check"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$solution = Join-Path $repoRoot "ISOCodex.Addressing.sln"
$outputPath = Join-Path $repoRoot $OutputDirectory
$propsPath = Join-Path $repoRoot "Directory.Build.props"
$packageProjectPaths = @(
    "src/Addressing/Addressing.csproj",
    "src/Addressing.Brazil/Addressing.Brazil.csproj",
    "src/Addressing.Canada/Addressing.Canada.csproj",
    "src/Addressing.France/Addressing.France.csproj",
    "src/Addressing.Germany/Addressing.Germany.csproj",
    "src/Addressing.GreatBritain/Addressing.GreatBritain.csproj",
    "src/Addressing.India/Addressing.India.csproj",
    "src/Addressing.Ireland/Addressing.Ireland.csproj",
    "src/Addressing.Italy/Addressing.Italy.csproj",
    "src/Addressing.Mexico/Addressing.Mexico.csproj",
    "src/Addressing.Spain/Addressing.Spain.csproj",
    "src/Addressing.UnitedStates/Addressing.UnitedStates.csproj"
)

Set-Location $repoRoot

Write-Host "Cleaning $Configuration outputs..."
dotnet clean $solution --configuration $Configuration

if (Test-Path $outputPath)
{
    Remove-Item -LiteralPath $outputPath -Recurse -Force
}

New-Item -ItemType Directory -Path $outputPath | Out-Null

Write-Host "Restoring..."
dotnet restore $solution

Write-Host "Building..."
dotnet build $solution --configuration $Configuration --no-restore

Write-Host "Testing..."
dotnet test $solution --configuration $Configuration --no-build

Write-Host "Packing..."
dotnet pack $solution --configuration $Configuration --no-restore -o $outputPath

$packages = Get-ChildItem -LiteralPath $outputPath -Filter "*.nupkg" | Sort-Object Name

[xml] $props = Get-Content -LiteralPath $propsPath
$version = $props.Project.PropertyGroup.Version

if ([string]::IsNullOrWhiteSpace($version))
{
    throw "Directory.Build.props does not contain a Version value"
}

$expectedPackages = foreach ($projectPath in $packageProjectPaths)
{
    $fullProjectPath = Join-Path $repoRoot $projectPath
    [xml] $project = Get-Content -LiteralPath $fullProjectPath
    $packageId = $project.Project.PropertyGroup.PackageId

    if ([string]::IsNullOrWhiteSpace($packageId))
    {
        throw "$projectPath does not contain a PackageId value"
    }

    "$packageId.$version.nupkg"
}

$packageNames = @($packages | ForEach-Object { $_.Name })

if ($packageNames.Count -ne $expectedPackages.Count)
{
    throw "Expected $($expectedPackages.Count) packages, but found $($packageNames.Count): $($packageNames -join ', ')"
}

foreach ($expectedPackage in $expectedPackages)
{
    if ($packageNames -notcontains $expectedPackage)
    {
        throw "Expected package '$expectedPackage' was not produced. Found: $($packageNames -join ', ')"
    }
}

Add-Type -AssemblyName System.IO.Compression.FileSystem

foreach ($package in $packages)
{
    Write-Host "Inspecting $($package.Name)..."

    $archive = [System.IO.Compression.ZipFile]::OpenRead($package.FullName)

    try
    {
        $entries = @($archive.Entries | ForEach-Object { $_.FullName })

        if ($entries -notcontains "README.md")
        {
            throw "$($package.Name) does not contain README.md"
        }

        $nuspec = $entries | Where-Object { $_ -like "*.nuspec" } | Select-Object -First 1

        if ($null -eq $nuspec)
        {
            throw "$($package.Name) does not contain a nuspec"
        }

        $libraryEntry = $entries |
            Where-Object { $_ -like "lib/netstandard2.1/*.dll" } |
            Select-Object -First 1

        if ($null -eq $libraryEntry)
        {
            throw "$($package.Name) does not contain a netstandard2.1 library"
        }

        if ($package.Name -ne "ISOCodex.Addressing.$version.nupkg")
        {
            $nuspecEntry = $archive.GetEntry($nuspec)

            if ($null -eq $nuspecEntry)
            {
                throw "$($package.Name) nuspec could not be read"
            }

            $reader = New-Object System.IO.StreamReader($nuspecEntry.Open())

            try
            {
                [xml] $nuspecXml = $reader.ReadToEnd()
            }
            finally
            {
                $reader.Dispose()
            }

            $coreDependency = $nuspecXml.package.metadata.dependencies.group.dependency |
                Where-Object { $_.id -eq "ISOCodex.Addressing" } |
                Select-Object -First 1

            if ($null -eq $coreDependency)
            {
                throw "$($package.Name) does not depend on ISOCodex.Addressing"
            }

            if ($coreDependency.version -ne $version)
            {
                throw "$($package.Name) depends on ISOCodex.Addressing $($coreDependency.version), expected $version"
            }
        }
    }
    finally
    {
        $archive.Dispose()
    }
}

Write-Host "Release check passed. Packages are in $outputPath"
