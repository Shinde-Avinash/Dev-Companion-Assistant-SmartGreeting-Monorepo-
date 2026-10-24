[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$OutputDir = "artifacts"
)

$ErrorActionPreference = "Stop"

$root = Resolve-Path "$PSScriptRoot\.."
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   SmartGreeting VSIX Packaging Pipeline" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Resolve Artifacts Output Directory
$artifactsPath = Join-Path $root $OutputDir
if (!(Test-Path $artifactsPath)) {
    New-Item -ItemType Directory -Path $artifactsPath -Force | Out-Null
}

$vsixTargetFile = Join-Path $artifactsPath "SmartGreeting.vsix"

# 2. Extract Version from Directory.Build.props
$propsPath = Join-Path $root "Directory.Build.props"
$version = "1.0.0"
if (Test-Path $propsPath) {
    [xml]$propsXml = Get-Content $propsPath
    if ($propsXml.Project.PropertyGroup.VersionPrefix) {
        $version = $propsXml.Project.PropertyGroup.VersionPrefix.Trim()
    }
}
Write-Host "[+] Packaging SmartGreeting Version: $version" -ForegroundColor Green

# 3. Create Staging Directory
$stagingDir = Join-Path $env:TEMP "SmartGreeting_VSIX_Staging_$(Get-Random)"
if (Test-Path $stagingDir) {
    Remove-Item -Path $stagingDir -Recurse -Force
}
New-Item -ItemType Directory -Path $stagingDir -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $stagingDir "Resources") -Force | Out-Null

try {
    # 4. Copy Binaries
    $binDir = Join-Path $root "src\SmartGreeting.Vsix\bin\$Configuration\net8.0-windows"
    if (!(Test-Path $binDir)) {
        $binDir = Join-Path $root "src\SmartGreeting.Vsix\bin\Debug\net8.0-windows"
    }

    if (!(Test-Path $binDir)) {
        throw "Could not locate compiled binaries in $binDir. Please run build.ps1 first."
    }

    Write-Host "[+] Collecting binaries from $binDir..." -ForegroundColor Gray
    Copy-Item -Path "$binDir\SmartGreeting.dll" -Destination $stagingDir -Force
    if (Test-Path "$binDir\SmartGreeting.pdb") {
        Copy-Item -Path "$binDir\SmartGreeting.pdb" -Destination $stagingDir -Force
    }
    if (Test-Path "$binDir\SmartGreeting.Core.dll") {
        Copy-Item -Path "$binDir\SmartGreeting.Core.dll" -Destination $stagingDir -Force
    }
    if (Test-Path "$binDir\SmartGreeting.UI.dll") {
        Copy-Item -Path "$binDir\SmartGreeting.UI.dll" -Destination $stagingDir -Force
    }

    # 5. Copy & Update Manifest
    $manifestSource = Join-Path $root "src\SmartGreeting.Vsix\source.extension.vsixmanifest"
    [xml]$manifestXml = Get-Content $manifestSource
    $manifestXml.PackageManifest.Metadata.Identity.SetAttribute("Version", $version)
    $stagedManifest = Join-Path $stagingDir "extension.vsixmanifest"
    $manifestXml.Save($stagedManifest)

    # 6. Copy Pkgdef and OPC Content Types
    Copy-Item -Path (Join-Path $root "src\SmartGreeting.Vsix\SmartGreeting.pkgdef") -Destination $stagingDir -Force
    Copy-Item -LiteralPath (Join-Path $root "src\SmartGreeting.Vsix\[Content_Types].xml") -Destination (Join-Path $stagingDir "[Content_Types].xml") -Force

    # 7. Copy Assets & License
    $resSource = Join-Path $root "src\SmartGreeting.Vsix\Resources"
    if (Test-Path "$resSource\icon.png") {
        Copy-Item -Path "$resSource\icon.png" -Destination (Join-Path $stagingDir "Resources\icon.png") -Force
    }
    if (Test-Path "$resSource\preview.png") {
        Copy-Item -Path "$resSource\preview.png" -Destination (Join-Path $stagingDir "Resources\preview.png") -Force
    }
    if (Test-Path (Join-Path $root "LICENSE")) {
        Copy-Item -Path (Join-Path $root "LICENSE") -Destination (Join-Path $stagingDir "LICENSE") -Force
    }

    # 8. Create VSIX Package (ZIP compliant with OPC)
    if (Test-Path $vsixTargetFile) {
        Remove-Item -Path $vsixTargetFile -Force
    }

    Write-Host "[+] Creating OPC VSIX archive at $vsixTargetFile..." -ForegroundColor Gray
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory($stagingDir, $vsixTargetFile, [System.IO.Compression.CompressionLevel]::Optimal, $false)

    $fileInfo = Get-Item $vsixTargetFile
    $sizeKb = [Math]::Round($fileInfo.Length / 1KB, 1)

    Write-Host "==========================================================" -ForegroundColor Green
    Write-Host "   VSIX Package Successfully Generated!" -ForegroundColor Green
    Write-Host "   Path: $vsixTargetFile ($sizeKb KB)" -ForegroundColor Green
    Write-Host "==========================================================" -ForegroundColor Green
}
finally {
    if (Test-Path $stagingDir) {
        Remove-Item -Path $stagingDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}
