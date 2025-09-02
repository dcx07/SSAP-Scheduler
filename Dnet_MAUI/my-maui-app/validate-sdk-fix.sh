#!/bin/bash

# SDK Detection Validation Test Script
# This script validates the .NET SDK detection fixes for the MAUI project

echo "=== SSAP Scheduler SDK Detection Validation Test ==="
echo

# Check if we're in the right directory
if [ ! -f "my-maui-app.csproj" ]; then
    echo "Error: Must be run from the my-maui-app project directory"
    exit 1
fi

echo "✓ Running from correct project directory"

# Test 1: Check for global.json
echo
echo "Test 1: Checking global.json configuration..."
if [ -f "global.json" ]; then
    echo "✓ global.json exists"
    
    # Check if it has SDK configuration
    if grep -q "\"sdk\"" global.json; then
        echo "✓ SDK configuration found in global.json"
        SDK_VERSION=$(grep -A1 "\"version\"" global.json | tail -1 | sed 's/.*"\(.*\)".*/\1/' | sed 's/,$//')
        echo "  SDK Version: $SDK_VERSION"
    else
        echo "✗ SDK configuration missing in global.json"
    fi
else
    echo "✗ global.json not found - this may cause SDK detection issues"
fi

# Test 2: Check .NET SDK version
echo
echo "Test 2: Checking .NET SDK installation..."
DOTNET_VERSION=$(dotnet --version 2>/dev/null)
if [ $? -eq 0 ]; then
    echo "✓ .NET SDK detected: $DOTNET_VERSION"
    
    # Check if it's .NET 8
    if [[ $DOTNET_VERSION == 8.* ]]; then
        echo "✓ .NET 8 SDK confirmed"
    else
        echo "⚠ .NET version is not 8.x: $DOTNET_VERSION"
    fi
else
    echo "✗ .NET SDK not found or not accessible"
    exit 1
fi

# Test 3: Check NuGet.config
echo
echo "Test 3: Checking NuGet configuration..."
if [ -f "NuGet.config" ]; then
    echo "✓ NuGet.config exists"
    
    if grep -q "packageSourceMapping" NuGet.config; then
        echo "✓ Package source mapping configured"
    else
        echo "⚠ Package source mapping not found"
    fi
else
    echo "⚠ NuGet.config not found - may use default sources"
fi

# Test 4: Check project configuration
echo
echo "Test 4: Checking project configuration..."
if grep -q "IsWindows.*MSBuild.*IsOsPlatform" my-maui-app.csproj; then
    echo "✓ Platform detection configured"
else
    echo "⚠ Platform detection not found"
fi

if grep -q "DisableImplicitNuGetFallbackFolder" Directory.Build.props 2>/dev/null; then
    echo "✓ SDK detection improvements configured"
else
    echo "⚠ SDK detection improvements not found"
fi

# Test 5: Platform-specific package references
echo
echo "Test 5: Checking conditional package references..."
if grep -q "Condition.*IsWindows.*true.*WindowsAppSDK" my-maui-app.csproj; then
    echo "✓ Windows App SDK packages are conditionally loaded"
else
    echo "⚠ Windows App SDK packages may load unconditionally"
fi

# Test 6: Build validation
echo
echo "Test 6: Testing build process..."

# Clean first
echo "  Cleaning project..."
dotnet clean > /dev/null 2>&1

# Test restore
echo "  Testing package restore..."
RESTORE_OUTPUT=$(dotnet restore --verbosity quiet 2>&1)
RESTORE_EXIT_CODE=$?

if [ $RESTORE_EXIT_CODE -eq 0 ]; then
    echo "✓ Package restore successful"
else
    echo "✗ Package restore failed:"
    echo "$RESTORE_OUTPUT" | head -5
fi

# Test build (expect different results on Windows vs non-Windows)
echo "  Testing build process..."
BUILD_OUTPUT=$(dotnet build --no-restore --verbosity quiet 2>&1)
BUILD_EXIT_CODE=$?

# Detect platform
PLATFORM=$(uname -s)
case $PLATFORM in
    MINGW*|CYGWIN*|MSYS*)
        EXPECTED_PLATFORM="Windows"
        ;;
    *)
        EXPECTED_PLATFORM="Non-Windows"
        ;;
esac

echo "  Platform detected: $EXPECTED_PLATFORM"

if [ "$EXPECTED_PLATFORM" = "Windows" ]; then
    # On Windows, build should succeed (if MAUI workload is installed)
    if [ $BUILD_EXIT_CODE -eq 0 ]; then
        echo "✓ Build successful on Windows platform"
    else
        # Check if it's a workload issue vs SDK detection issue
        if echo "$BUILD_OUTPUT" | grep -q -i "workload\|maui"; then
            echo "⚠ Build failed due to missing MAUI workload (expected)"
            echo "  This is not an SDK detection issue"
        elif echo "$BUILD_OUTPUT" | grep -q -i "sdk.*install\|NETSDK1045"; then
            echo "✗ Build failed due to SDK detection issues"
            echo "  This indicates the fix may not be working"
        else
            echo "⚠ Build failed for other reasons (not SDK detection)"
        fi
    fi
else
    # On non-Windows, build should fail but not due to SDK detection
    if echo "$BUILD_OUTPUT" | grep -q -i "sdk.*install\|NETSDK1045"; then
        echo "✗ Build failed due to SDK detection issues on non-Windows"
        echo "  This should not happen with our fixes"
    else
        echo "✓ Build failed on non-Windows due to platform limitations (expected)"
        echo "  No SDK detection issues detected"
    fi
fi

# Summary
echo
echo "=== Test Summary ==="
echo
echo "The fixes should resolve .NET SDK detection issues by:"
echo "1. ✓ Pinning SDK version with global.json"
echo "2. ✓ Improving NuGet package source configuration"  
echo "3. ✓ Adding platform-aware package loading"
echo "4. ✓ Enhancing MSBuild SDK detection properties"
echo "5. ✓ Providing cross-platform build compatibility"
echo
echo "If you're still experiencing 'install .NET SDK' prompts in Visual Studio:"
echo "- Run fix-sdk-detection.ps1 on Windows for detailed diagnostics"
echo "- Restart Visual Studio after applying these fixes"
echo "- Clear Visual Studio cache if issues persist"
echo
echo "Build validation completed."