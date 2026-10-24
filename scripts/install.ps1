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
    Write-Host "[!] VSIX not found. Packaging now..." -ForegroundColor Yellow
    & "$PSScriptRoot\package-vscode.ps1"
}

if (!(Test-Path $resolvedVsix)) {
    throw "VSIX file not found at $resolvedVsix."
}

# 2. Check for Visual Studio Code CLI
$codeCmd = Get-Command code -ErrorAction SilentlyContinue
if (-not $codeCmd) {
    $userCode = "C:\Users\om\AppData\Local\Programs\Microsoft VS Code\bin\code.cmd"
    if (Test-Path $userCode) {
        $codeCmd = $userCode
    }
}

$installed = $false

if ($codeCmd) {
    Write-Host "[+] Detected Visual Studio Code. Installing extension..." -ForegroundColor Green
    & $codeCmd --install-extension $resolvedVsix --force
    if ($LASTEXITCODE -eq 0) {
        Write-Host "==========================================================" -ForegroundColor Green
        Write-Host "   Extension 'SmartGreeting' successfully installed into VS Code!" -ForegroundColor Green
        Write-Host "   Restart or Reload VS Code window to activate." -ForegroundColor White
        Write-Host "==========================================================" -ForegroundColor Green
        $installed = $true
    }
}

# 3. Check for Visual Studio IDE VSIXInstaller.exe if not installed in VS Code
if (-not $installed) {
    $installerPaths = @(
        "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\VSIXInstaller.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\VSIXInstaller.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\VSIXInstaller.exe",
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\VSIXInstaller.exe"
    )
    $vsixInstaller = $installerPaths | Where-Object { Test-Path $_ } | Select-Object -First 1

    if ($vsixInstaller) {
        Write-Host "[+] Detected Visual Studio IDE installer: $vsixInstaller" -ForegroundColor Green
        Start-Process -FilePath $vsixInstaller -ArgumentList "`"$resolvedVsix`""
    } else {
        Start-Process -FilePath $resolvedVsix
    }
}
