# Local Build & Test Script (Windows)
# This script simulates the GitHub Actions workflow locally

$ErrorActionPreference = "Stop"

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "  Validated Primitives - Local Build" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$ProjectPath = "src\Valdiated.Primatives\Validated.Primitives.csproj"
$TestProjectPath = "tests\Valdiated.Primatives.Tests\Validated.Primitives.Tests.csproj"
$OutputDir = ".\artifacts"

# Calculate version (same as workflow)
[xml]$projectXml = Get-Content $ProjectPath
$BaseVersion = $projectXml.Project.PropertyGroup.Version
$CommitCount = (git rev-list --count HEAD 2>$null)
if (-not $CommitCount) { $CommitCount = 0 }
$MajorMinor = ($BaseVersion -split '\.')[0..1] -join '.'
$Version = "$MajorMinor.$CommitCount"

Write-Host "?? Package Information:" -ForegroundColor Yellow
Write-Host "   Base Version:   $BaseVersion"
Write-Host "   Commit Count:   $CommitCount"
Write-Host "   Build Version:  $Version"
Write-Host ""

# Clean previous builds
Write-Host "?? Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean --configuration Release > $null 2>&1
if (Test-Path $OutputDir) {
    Remove-Item -Path $OutputDir -Recurse -Force
}
New-Item -Path $OutputDir -ItemType Directory -Force > $null

# Restore dependencies
Write-Host "?? Restoring dependencies..." -ForegroundColor Yellow
dotnet restore

# Build
Write-Host "?? Building project..." -ForegroundColor Yellow
dotnet build --configuration Release `
  --no-restore `
  /p:Version="$Version" `
  /p:CI=true

# Test
Write-Host "?? Running tests..." -ForegroundColor Yellow
dotnet test --configuration Release `
  --no-build `
  --verbosity normal `
  --logger "console;verbosity=detailed"

# Pack
Write-Host "?? Creating NuGet package..." -ForegroundColor Yellow
dotnet pack $ProjectPath `
  --configuration Release `
  --no-build `
  --output $OutputDir `
  /p:Version="$Version" `
  /p:PackageVersion="$Version" `
  /p:CI=true

Write-Host ""
Write-Host "======================================" -ForegroundColor Green
Write-Host "  ? Build completed successfully!" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""
Write-Host "?? Packages created:" -ForegroundColor Yellow
Get-ChildItem $OutputDir | Format-Table Name, Length, LastWriteTime
Write-Host ""
Write-Host "Version: $Version" -ForegroundColor Cyan
Write-Host "Location: $OutputDir" -ForegroundColor Cyan
Write-Host ""
Write-Host "To publish manually:" -ForegroundColor Yellow
Write-Host "  dotnet nuget push $OutputDir\*.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor Gray
Write-Host ""
