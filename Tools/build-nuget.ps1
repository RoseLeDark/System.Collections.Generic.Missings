#Example:
#.\tools\publish-nuget.ps1 -PackageVersion 4.3.0

param(
    [string]$Configuration = "Release",
    [string]$PackageVersion = "",
    [string]$Source = "https://api.nuget.org/v3/index.json",
    [switch]$Push
)

$ErrorActionPreference = "Stop"

function InfoMessage {
    param([string]$Message)
    Write-Host "`n=== $Message ==="
}

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

InfoMessage "Restore"
dotnet restore

InfoMessage "Build"
dotnet build SystemEx.slnx --configuration $Configuration --no-restore

InfoMessage "Pack"
$packArgs = @(
    "pack",
    "src/SystemEx.csproj",
    "--configuration", $Configuration,
    "--no-build",
    "--output", "bin/publish",
    "/p:ContinuousIntegrationBuild=true"
)

if (-not [string]::IsNullOrWhiteSpace($PackageVersion)) {
    $packArgs += "/p:PackageVersion=$PackageVersion"
}

dotnet @packArgs


InfoMessage "Package pushed to bin/publish for upload to https://www.nuget.org/packages/manage/upload."

if ($Push) {
    dotnet nuget push "bin/publish/*.nupkg" --source $Source
}