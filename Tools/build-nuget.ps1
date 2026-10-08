
param(
    [string]$Configuration = "Release"
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
    "--output", "build/publish",
    "/p:ContinuousIntegrationBuild=true"
)

if (-not [string]::IsNullOrWhiteSpace($PackageVersion)) { 
    $packArgs += "/p:PackageVersion=$PackageVersion"
}

dotnet @packArgs


InfoMessage "Push"
nuget push "build/publish/*.nupkg"