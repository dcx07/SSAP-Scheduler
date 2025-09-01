# PowerShell script to validate MAUI project configuration
# Run this in Windows to check if the fixes resolve the issues

Write-Host "=== SSAP Scheduler MAUI Project Validation ===" -ForegroundColor Green

Write-Host "`nChecking project configuration..." -ForegroundColor Yellow

# Check if project file exists and has correct SDK
$projectFile = "my-maui-app.csproj"
if (Test-Path $projectFile) {
    $content = Get-Content $projectFile -Raw
    
    Write-Host "✓ Project file found" -ForegroundColor Green
    
    # Check SDK
    if ($content -match 'Sdk="Microsoft\.NET\.Sdk"') {
        Write-Host "✓ Using correct SDK (Microsoft.NET.Sdk)" -ForegroundColor Green
    } else {
        Write-Host "✗ Wrong SDK detected" -ForegroundColor Red
    }
    
    # Check MAUI properties
    if ($content -match '<UseMaui>true</UseMaui>') {
        Write-Host "✓ MAUI enabled" -ForegroundColor Green
    } else {
        Write-Host "✗ MAUI not enabled" -ForegroundColor Red
    }
    
    # Check Windows App SDK
    if ($content -match 'WindowsAppSDKSelfContained.*true') {
        Write-Host "✓ Windows App SDK self-contained enabled" -ForegroundColor Green
    } else {
        Write-Host "⚠ Windows App SDK self-contained not configured" -ForegroundColor Yellow
    }
    
    # Check MSIX packaging disabled
    if ($content -match 'WindowsPackageType.*None') {
        Write-Host "✓ MSIX packaging disabled" -ForegroundColor Green
    } else {
        Write-Host "⚠ MSIX packaging might be enabled" -ForegroundColor Yellow
    }
    
    # Check for specific resource inclusions
    if ($content -match 'Resources\\Images\\\*\.png') {
        Write-Host "✓ Specific resource patterns used (prevents excessive copying)" -ForegroundColor Green
    } else {
        Write-Host "⚠ Broad resource patterns might cause issues" -ForegroundColor Yellow
    }
} else {
    Write-Host "✗ Project file not found!" -ForegroundColor Red
}

# Check Directory.Build files
if (Test-Path "Directory.Build.props") {
    Write-Host "✓ Directory.Build.props found" -ForegroundColor Green
} else {
    Write-Host "⚠ Directory.Build.props missing" -ForegroundColor Yellow
}

if (Test-Path "Directory.Build.targets") {
    Write-Host "✓ Directory.Build.targets found" -ForegroundColor Green
} else {
    Write-Host "⚠ Directory.Build.targets missing" -ForegroundColor Yellow
}

Write-Host "`n=== Build Test ===" -ForegroundColor Yellow
Write-Host "Running dotnet restore..." -ForegroundColor Cyan

try {
    $restoreResult = dotnet restore 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Restore successful" -ForegroundColor Green
    } else {
        Write-Host "✗ Restore failed:" -ForegroundColor Red
        Write-Host $restoreResult -ForegroundColor Red
    }
} catch {
    Write-Host "✗ Error running restore: $_" -ForegroundColor Red
}

Write-Host "`nTrying build (this might fail if Windows SDK/MAUI workload not installed)..." -ForegroundColor Cyan

try {
    $buildResult = dotnet build --no-restore 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Build successful - issues should be resolved!" -ForegroundColor Green
    } else {
        Write-Host "Build output:" -ForegroundColor Yellow
        Write-Host $buildResult -ForegroundColor White
        
        # Check for specific error patterns
        if ($buildResult -match "directory.*copying|copying.*directory") {
            Write-Host "⚠ Still seeing directory copying issues" -ForegroundColor Red
        } else {
            Write-Host "✓ No directory copying issues detected in build output" -ForegroundColor Green
        }
        
        if ($buildResult -match "WindowsAppSDK.*not found|dll.*not found") {
            Write-Host "⚠ Windows App SDK DLL issues still present" -ForegroundColor Red
        } else {
            Write-Host "✓ No Windows App SDK DLL issues detected" -ForegroundColor Green
        }
    }
} catch {
    Write-Host "✗ Error running build: $_" -ForegroundColor Red
}

Write-Host "`n=== Summary ===" -ForegroundColor Green
Write-Host "Configuration changes made to fix:"
Write-Host "1. Directory copying issue - Added specific resource patterns and exclusions"
Write-Host "2. Windows App SDK DLL errors - Added explicit package references and configuration"
Write-Host "3. MSIX packaging issues - Comprehensive disabling of MSIX features"
Write-Host "`nIf build succeeds, both issues should be resolved!" -ForegroundColor Cyan