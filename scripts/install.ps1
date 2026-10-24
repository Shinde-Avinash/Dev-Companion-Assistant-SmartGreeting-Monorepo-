[CmdletBinding()]
param(
    [string]$VsixPath = "artifacts\SmartGreeting.vsix",
    [switch]$Force
)

$ErrorActionPreference = "Stop"

$root = Resolve-Path "$PSScriptRoot\.."
$resolvedVsix = Join-Path $root $VsixPath

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   SmartGreeting Local Extension Installer" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Ensure VSIX exists
if (!(Test-Path $resolvedVsix)) {
    Write-Host "[!] VSIX not found at $resolvedVsix. Building now..." -ForegroundColor Yellow
    & "$PSScriptRoot\build.ps1"
}

if (!(Test-Path $resolvedVsix)) {
    throw "VSIX file could not be generated at $resolvedVsix."
}

# 2. Check if Visual Studio is running
$runningVs = Get-Process devenv -ErrorAction SilentlyContinue
if ($runningVs) {
    Write-Host ""
    Write-Host "⚠️  ATTENTION: Visual Studio (devenv.exe) is currently running!" -ForegroundColor Yellow
    Write-Host "   Visual Studio locks extension assemblies while running." -ForegroundColor Yellow
    Write-Host "   Please save your work and close Visual Studio, then press ENTER to continue..." -ForegroundColor Yellow
    if (-not $Force) {
        Read-Host "Press ENTER after closing Visual Studio (or Ctrl+C to cancel)"
    }
}

# 3. Locate VSIXInstaller.exe
$installerPaths = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\VSIXInstaller.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\VSIXInstaller.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\VSIXInstaller.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Preview\Common7\IDE\VSIXInstaller.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\VSIXInstaller.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\VSIXInstaller.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\VSIXInstaller.exe"
)

$vsixInstaller = $installerPaths | Where-Object { Test-Path $_ } | Select-Object -First 1

if ($vsixInstaller) {
    Write-Host "[+] Found Visual Studio VSIX Installer: $vsixInstaller" -ForegroundColor Green
    Write-Host "[+] Launching VSIX Installer dialog..." -ForegroundColor Cyan
    Start-Process -FilePath $vsixInstaller -ArgumentList "`"$resolvedVsix`""
} else {
    Write-Host "[+] Launching VSIX with default Windows file association..." -ForegroundColor Cyan
    try {
        Start-Process -FilePath $resolvedVsix
    } catch {
        Write-Host "[!] Could not automatically launch VSIXInstaller." -ForegroundColor Yellow
        Write-Host "    You can manually double-click the file to install:" -ForegroundColor White
        Write-Host "    $resolvedVsix" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "==========================================================" -ForegroundColor Green
Write-Host "   Installation / Update Instructions:" -ForegroundColor Green
Write-Host "   1. Click 'Install' or 'Modify' in the VSIX Installer dialog." -ForegroundColor White
Write-Host "   2. Once finished, open Visual Studio." -ForegroundColor White
Write-Host "   3. SmartGreeting will welcome you on startup!" -ForegroundColor White
Write-Host "==========================================================" -ForegroundColor Green
