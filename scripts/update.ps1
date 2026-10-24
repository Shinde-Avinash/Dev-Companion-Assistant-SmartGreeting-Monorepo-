[CmdletBinding()]
param(
    [string]$NewVersion = "",
    [switch]$BumpPatch,
    [switch]$SkipTests,
    [switch]$Force
)

$ErrorActionPreference = "Stop"

$root = Resolve-Path "$PSScriptRoot\.."
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   SmartGreeting One-Command Developer Update Workflow" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Update Version if requested
$propsPath = Join-Path $root "Directory.Build.props"
if ($BumpPatch -or $NewVersion) {
    if (Test-Path $propsPath) {
        $content = Get-Content $propsPath -Raw
        if ($NewVersion) {
            $updatedVersion = $NewVersion
        } else {
            # Parse existing version and increment patch
            if ($content -match '<VersionPrefix>(\d+)\.(\d+)\.(\d+)</VersionPrefix>') {
                $major = [int]$matches[1]
                $minor = [int]$matches[2]
                $patch = [int]$matches[3] + 1
                $updatedVersion = "$major.$minor.$patch"
            } else {
                $updatedVersion = "1.0.1"
            }
        }

        Write-Host "[+] Bumping extension version to: $updatedVersion" -ForegroundColor Yellow
        $content = $content -replace '<VersionPrefix>.*?</VersionPrefix>', "<VersionPrefix>$updatedVersion</VersionPrefix>"
        $content = $content -replace '<AssemblyVersion>.*?</AssemblyVersion>', "<AssemblyVersion>$updatedVersion.0</AssemblyVersion>"
        $content = $content -replace '<FileVersion>.*?</FileVersion>', "<FileVersion>$updatedVersion.0</FileVersion>"
        Set-Content -Path $propsPath -Value $content -NoNewline
    }
}

# 2. Execute Build & Packaging
Write-Host "[1/3] Building solution and verifying unit tests..." -ForegroundColor Cyan
$buildArgs = @{}
if ($SkipTests) { $buildArgs["SkipTests"] = $true }
& "$PSScriptRoot\build.ps1" @buildArgs

# 3. Check Visual Studio Running Process
$runningVs = Get-Process devenv -ErrorAction SilentlyContinue
if ($runningVs) {
    Write-Host ""
    Write-Host "⚠️  Visual Studio is running." -ForegroundColor Yellow
    Write-Host "   To update the extension safely without file lock errors:" -ForegroundColor Yellow
    Write-Host "   1. Close Visual Studio." -ForegroundColor White
    Write-Host "   2. Proceed with installation." -ForegroundColor White
}

# 4. Invoke Installer
Write-Host "[2/3] Preparing VSIX installer..." -ForegroundColor Cyan
$installArgs = @{}
if ($Force) { $installArgs["Force"] = $true }
& "$PSScriptRoot\install.ps1" @installArgs

Write-Host "[3/3] Ready!" -ForegroundColor Green
Write-Host ""
Write-Host "==========================================================" -ForegroundColor Green
Write-Host "   Extension updated successfully!" -ForegroundColor Green
Write-Host "   Restart Visual Studio to load your updated extension." -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
