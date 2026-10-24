[CmdletBinding()]
param(
    [string]$NewVersion = "",
    [switch]$BumpPatch,
    [switch]$Force
)

$ErrorActionPreference = "Stop"

$root = Resolve-Path "$PSScriptRoot\.."
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   SmartGreeting One-Command Developer Update Workflow" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Update Version in package.json if requested
$pkgJsonPath = Join-Path $root "src\SmartGreeting.VsCode\package.json"
if ($BumpPatch -or $NewVersion) {
    if (Test-Path $pkgJsonPath) {
        $content = Get-Content $pkgJsonPath -Raw
        if ($NewVersion) {
            $updatedVersion = $NewVersion
        } else {
            if ($content -match '"version":\s*"(\d+)\.(\d+)\.(\d+)"') {
                $major = [int]$matches[1]
                $minor = [int]$matches[2]
                $patch = [int]$matches[3] + 1
                $updatedVersion = "$major.$minor.$patch"
            } else {
                $updatedVersion = "1.0.1"
            }
        }
        Write-Host "[+] Bumping extension version to: $updatedVersion" -ForegroundColor Yellow
        $content = $content -replace '"version":\s*".*?"', "`"version`": `"$updatedVersion`""
        Set-Content -Path $pkgJsonPath -Value $content -NoNewline
    }
}

# 2. Package VS Code VSIX
Write-Host "[1/2] Packaging extension into artifacts\SmartGreeting.vsix..." -ForegroundColor Cyan
& "$PSScriptRoot\package-vscode.ps1"

# 3. Install Extension into VS Code
Write-Host "[2/2] Installing extension..." -ForegroundColor Cyan
& "$PSScriptRoot\install.ps1" -Force

Write-Host ""
Write-Host "==========================================================" -ForegroundColor Green
Write-Host "   Extension updated successfully!" -ForegroundColor Green
Write-Host "   Press Ctrl+Shift+P -> 'Developer: Reload Window' in VS Code to activate." -ForegroundColor White
Write-Host "==========================================================" -ForegroundColor Green
