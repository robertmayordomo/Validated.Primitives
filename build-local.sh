#!/bin/bash

# Local Build & Test Script
# This script simulates the GitHub Actions workflow locally

set -e

echo "======================================"
echo "  Validated Primitives - Local Build"
echo "======================================"
echo ""

# Configuration
PROJECT_PATH="src/Valdiated.Primatives/Validated.Primitives.csproj"
DOMAIN_PROJECT_PATH="src/Validated.Primitives.Domain/Validated.Primitives.Domain.csproj"
TEST_PROJECT_PATH="tests/Valdiated.Primatives.Tests/Validated.Primitives.Tests.csproj"
DOMAIN_TEST_PROJECT_PATH="tests/Validated.Primitives.Domain.Tests/Validated.Primitives.Domain.Tests.csproj"
OUTPUT_DIR="./artifacts"

# Calculate version (same as workflow)
BASE_VERSION=$(grep -oP '(?<=<Version>)[^<]+' "$PROJECT_PATH" | head -1)
COMMIT_COUNT=$(git rev-list --count HEAD 2>/dev/null || echo "0")
MAJOR_MINOR=$(echo "$BASE_VERSION" | cut -d '.' -f 1-2)
VERSION="$MAJOR_MINOR.$COMMIT_COUNT"

echo "?? Package Information:"
echo "   Base Version:   $BASE_VERSION"
echo "   Commit Count:   $COMMIT_COUNT"
echo "   Build Version:  $VERSION"
echo ""

# Clean previous builds
echo "?? Cleaning previous builds..."
dotnet clean --configuration Release > /dev/null 2>&1
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

# Restore dependencies
echo "?? Restoring dependencies..."
dotnet restore

# Build
echo "?? Building project..."
dotnet build --configuration Release \
  --no-restore \
  /p:Version="$VERSION" \
  /p:CI=true

# Test
echo "?? Running tests..."
dotnet test --configuration Release \
  --no-build \
  --verbosity normal \
  --logger "console;verbosity=detailed"

# Pack
echo "?? Creating NuGet packages..."
dotnet pack "$PROJECT_PATH" \
  --configuration Release \
  --no-build \
  --output "$OUTPUT_DIR" \
  /p:Version="$VERSION" \
  /p:PackageVersion="$VERSION" \
  /p:CI=true

dotnet pack "$DOMAIN_PROJECT_PATH" \
  --configuration Release \
  --no-build \
  --output "$OUTPUT_DIR" \
  /p:Version="$VERSION" \
  /p:PackageVersion="$VERSION" \
  /p:CI=true

echo ""
echo "======================================"
echo "  ? Build completed successfully!"
echo "======================================"
echo ""
echo "?? Packages created:"
ls -lh "$OUTPUT_DIR"
echo ""
echo "Version: $VERSION"
echo "Location: $OUTPUT_DIR"
echo ""
echo "To publish manually:"
echo "  dotnet nuget push $OUTPUT_DIR/*.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json"
echo ""
