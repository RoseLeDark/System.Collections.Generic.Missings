
param(
    [string]$Configuration = "Release",
    [string]$PackageVersion = ""
)

$ErrorActionPreference = "Stop"

function InfoMessage {
    param([string]$Message)
    Write-Host "`n=== $Message ==="
}

InfoMessage "Clean"
Remove-Item -Path  "build/publish/*.nupkg"  -Force -ErrorAction SilentlyContinue  
Remove-Item -Path  "build/publish/*.snupkg"  -Force -ErrorAction SilentlyContinue 


InfoMessage "Restore"
dotnet restore

InfoMessage "Build"
dotnet build SystemEx.slnx --configuration $Configuration --no-restore
Remove-Item -Path  bin/ -Force -ErrorAction SilentlyContinue 


InfoMessage "Pack"
$packArgs = @(
    "pack",
    "src/RoseLeDark.Collections.Missings.csproj",
    "--configuration", $Configuration,
    "--no-build",
    "--output", "build/publish",
    "/p:ContinuousIntegrationBuild=true"
)

if (-not [string]::IsNullOrWhiteSpace($PackageVersion)) { 
    $packArgs += "/p:PackageVersion=$PackageVersion"
}

 

dotnet @packArgs
