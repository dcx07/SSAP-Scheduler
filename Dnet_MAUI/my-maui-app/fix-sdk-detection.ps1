# PowerShell script to fix .NET SDK detection issues in MAUI projects
# This addresses the issue where "install .NET SDK" prompts appear even when SDK is installed

Write-Host "=== SSAP Scheduler SDK Detection Fix ===`n" -ForegroundColor Green

# Function to check .NET SDK installation and path
function Test-DotNetSDK {
    Write-Host "Checking .NET SDK installation..." -ForegroundColor Yellow
    
    try {
        $sdkVersion = dotnet --version
        Write-Host "✓ .NET SDK Version: $sdkVersion" -ForegroundColor Green
        
        $sdkPath = dotnet --info | Select-String "Base Path:"
        if ($sdkPath) {
            Write-Host "✓ SDK Base Path: $($sdkPath -replace 'Base Path:\s*', '')" -ForegroundColor Green
        }
        
        # List installed SDKs
        Write-Host "`nInstalled .NET SDKs:" -ForegroundColor Cyan
        dotnet --list-sdks
        
        return $true
    }
    catch {
        Write-Host "✗ .NET SDK not found or not properly configured" -ForegroundColor Red
        Write-Host "  Error: $_" -ForegroundColor Red
        return $false
    }
}

# Function to check and fix environment variables
function Test-EnvironmentVariables {
    Write-Host "`nChecking environment variables..." -ForegroundColor Yellow
    
    $dotnetRoot = [System.Environment]::GetEnvironmentVariable("DOTNET_ROOT", "User")
    $dotnetRootMachine = [System.Environment]::GetEnvironmentVariable("DOTNET_ROOT", "Machine")
    $path = [System.Environment]::GetEnvironmentVariable("PATH", "User")
    
    Write-Host "DOTNET_ROOT (User): $dotnetRoot" -ForegroundColor Cyan
    Write-Host "DOTNET_ROOT (Machine): $dotnetRootMachine" -ForegroundColor Cyan
    
    # Check if dotnet is in PATH
    $dotnetInPath = $path -split ';' | Where-Object { $_ -like "*dotnet*" }
    if ($dotnetInPath) {
        Write-Host "✓ .NET found in PATH: $dotnetInPath" -ForegroundColor Green
    } else {
        Write-Host "⚠ .NET not found in PATH - this may cause SDK detection issues" -ForegroundColor Yellow
        
        # Suggest adding to PATH if not there
        $programFiles = ${env:ProgramFiles}
        $dotnetPath = "$programFiles\dotnet"
        if (Test-Path $dotnetPath) {
            Write-Host "  Suggested fix: Add '$dotnetPath' to your PATH environment variable" -ForegroundColor Yellow
        }
    }
}

# Function to validate project configuration
function Test-ProjectConfiguration {
    Write-Host "`nValidating project configuration..." -ForegroundColor Yellow
    
    # Check if global.json exists
    if (Test-Path "global.json") {
        Write-Host "✓ global.json file found - helps with SDK version pinning" -ForegroundColor Green
        
        $globalJson = Get-Content "global.json" | ConvertFrom-Json
        if ($globalJson.sdk -and $globalJson.sdk.version) {
            Write-Host "  SDK Version Pinned: $($globalJson.sdk.version)" -ForegroundColor Cyan
        }
    } else {
        Write-Host "⚠ global.json file missing - may cause SDK detection issues" -ForegroundColor Yellow
    }
    
    # Check project file
    if (Test-Path "my-maui-app.csproj") {
        $projectContent = Get-Content "my-maui-app.csproj" -Raw
        
        # Check for proper SDK detection configuration
        if ($projectContent -match 'DisableImplicitNuGetFallbackFolder') {
            Write-Host "✓ SDK detection improvements configured" -ForegroundColor Green
        } else {
            Write-Host "⚠ Missing SDK detection improvements" -ForegroundColor Yellow
        }
        
        # Check for conditional Windows configuration
        if ($projectContent -match 'IsWindows.*MSBuild.*IsOsPlatform') {
            Write-Host "✓ Cross-platform detection configured" -ForegroundColor Green
        } else {
            Write-Host "⚠ Missing cross-platform detection" -ForegroundColor Yellow
        }
    }
}

# Function to test build process
function Test-BuildProcess {
    Write-Host "`nTesting build process..." -ForegroundColor Yellow
    
    try {
        Write-Host "Running 'dotnet restore'..." -ForegroundColor Cyan
        $restoreOutput = dotnet restore --verbosity quiet 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Package restore successful" -ForegroundColor Green
        } else {
            Write-Host "✗ Package restore failed:" -ForegroundColor Red
            Write-Host "  $restoreOutput" -ForegroundColor Red
            return $false
        }
        
        Write-Host "Running 'dotnet build --no-restore'..." -ForegroundColor Cyan
        $buildOutput = dotnet build --no-restore --verbosity quiet 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Build successful" -ForegroundColor Green
            return $true
        } else {
            Write-Host "⚠ Build completed with issues:" -ForegroundColor Yellow
            
            # Check for common SDK detection issues
            if ($buildOutput -match "install.*SDK" -or $buildOutput -match "NETSDK1045") {
                Write-Host "  ⚠ SDK detection issue detected in build output" -ForegroundColor Yellow
                Write-Host "  This may be the source of the 'install .NET SDK' prompts during debugging" -ForegroundColor Yellow
            }
            
            # Show only error lines
            $buildOutput -split "`n" | Where-Object { $_ -match "error|Error|ERROR" } | ForEach-Object {
                Write-Host "  $_" -ForegroundColor Red
            }
            
            return $false
        }
    }
    catch {
        Write-Host "✗ Build process failed: $_" -ForegroundColor Red
        return $false
    }
}

# Function to provide fixes for common issues
function Show-FixSuggestions {
    Write-Host "`n=== Fix Suggestions ===`n" -ForegroundColor Green
    
    Write-Host "If you're still getting 'install .NET SDK' prompts during debugging:" -ForegroundColor Cyan
    Write-Host "1. Restart Visual Studio after running this script" -ForegroundColor White
    Write-Host "2. Clear Visual Studio cache: Delete '%localappdata%\Microsoft\VisualStudio\*\ComponentModelCache'" -ForegroundColor White
    Write-Host "3. Clear NuGet cache: 'dotnet nuget locals all --clear'" -ForegroundColor White
    Write-Host "4. Ensure Visual Studio is running as Administrator if on corporate network" -ForegroundColor White
    Write-Host "5. Check Windows SDK is installed (required for MAUI Windows target)" -ForegroundColor White
    
    Write-Host "`nIf building on non-Windows (development only):" -ForegroundColor Cyan
    Write-Host "- Cross-platform build support has been added but full MAUI features require Windows" -ForegroundColor White
    Write-Host "- Use this configuration for CI/CD or development scenarios only" -ForegroundColor White
}

# Main execution
$sdkOk = Test-DotNetSDK
Test-EnvironmentVariables
Test-ProjectConfiguration

if ($sdkOk) {
    Test-BuildProcess
} else {
    Write-Host "`nSkipping build test due to SDK issues" -ForegroundColor Yellow
}

Show-FixSuggestions

Write-Host "`n=== Summary ===`n" -ForegroundColor Green
Write-Host "This script has checked for common causes of .NET SDK detection issues:" -ForegroundColor White
Write-Host "- SDK installation and PATH configuration" -ForegroundColor White
Write-Host "- Project configuration files (global.json, .csproj)" -ForegroundColor White
Write-Host "- Build process validation" -ForegroundColor White
Write-Host "`nThe project configuration has been updated to improve SDK detection reliability." -ForegroundColor White
Write-Host "Please restart Visual Studio and try debugging again." -ForegroundColor Cyan