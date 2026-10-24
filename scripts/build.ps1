[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [switch]$SkipTests,
    [switch]$SkipPackage
)

$ErrorActionPreference = "Stop"

$root = Resolve-Path "$PSScriptRoot\.."
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   SmartGreeting Solution Build & Test Pipeline" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Resolve .NET SDK & ensure DOTNET_ROOT is configured
$userDotnetDir = Join-Path $env:LOCALAPPDATA "dotnet"
$userDotnetExe = Join-Path $userDotnetDir "dotnet.exe"

if (Test-Path $userDotnetExe) {
    $env:DOTNET_ROOT = $userDotnetDir
    $env:PATH = "$userDotnetDir;" + $env:PATH
    $dotnetCmd = $userDotnetExe
} elseif (Get-Command dotnet -ErrorAction SilentlyContinue) {
    $dotnetCmd = "dotnet"
} else {
    throw ".NET SDK was not found. Please install the .NET 8 SDK."
}

$sdkVersion = & $dotnetCmd --version
Write-Host "[+] Using .NET SDK: $sdkVersion" -ForegroundColor Green

# 2. Build Solution
$slnPath = Join-Path $root "SmartGreeting.sln"
Write-Host "[+] Building solution ($Configuration)..." -ForegroundColor Cyan
& $dotnetCmd build $slnPath -c $Configuration --nologo -nr:false /p:UseSharedCompilation=false
if ($LASTEXITCODE -ne 0) {
    throw "Build failed with exit code $LASTEXITCODE."
}
Write-Host "[OK] Build succeeded with 0 errors!" -ForegroundColor Green

# 3. Run Unit Tests
if (-not $SkipTests) {
    $testProj = Join-Path $root "tests\SmartGreeting.Tests\SmartGreeting.Tests.csproj"
    Write-Host "[+] Running Unit Tests..." -ForegroundColor Cyan
    & $dotnetCmd test $testProj -c $Configuration --no-build --nologo
    if ($LASTEXITCODE -ne 0) {
        throw "Unit tests failed with exit code $LASTEXITCODE."
    }
    Write-Host "[OK] All unit tests passed!" -ForegroundColor Green
}

# 4. Invoke Packaging
if (-not $SkipPackage) {
    $packageScript = Join-Path $PSScriptRoot "package.ps1"
    & $packageScript -Configuration $Configuration
}
