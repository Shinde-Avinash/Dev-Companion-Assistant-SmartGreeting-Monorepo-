[CmdletBinding()]
param(
    [string]$OutputDir = "artifacts"
)

$ErrorActionPreference = "Stop"

$root = Resolve-Path "$PSScriptRoot\.."
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   SmartGreeting VS Code VSIX Packaging Pipeline" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Output directory
$artifactsPath = Join-Path $root $OutputDir
if (!(Test-Path $artifactsPath)) {
    New-Item -ItemType Directory -Path $artifactsPath -Force | Out-Null
}

$vsixTargetFile = Join-Path $artifactsPath "SmartGreeting.vsix"
$vscodeVsixTargetFile = Join-Path $artifactsPath "SmartGreeting-VSCode.vsix"

# 2. Create Staging Directory
$stagingDir = Join-Path $env:TEMP "SmartGreeting_VSCode_VSIX_Staging_$(Get-Random)"
if (Test-Path $stagingDir) {
    Remove-Item -Path $stagingDir -Recurse -Force
}
New-Item -ItemType Directory -Path $stagingDir -Force | Out-Null
$extensionDir = Join-Path $stagingDir "extension"
New-Item -ItemType Directory -Path $extensionDir -Force | Out-Null

try {
    # 3. Copy files to extension/
    $srcDir = Join-Path $root "src\SmartGreeting.VsCode"
    Copy-Item -Path "$srcDir\package.json" -Destination $extensionDir -Force
    Copy-Item -Path "$srcDir\extension.js" -Destination $extensionDir -Force

    if (Test-Path (Join-Path $root "README.md")) {
        Copy-Item -Path (Join-Path $root "README.md") -Destination (Join-Path $extensionDir "README.md") -Force
    }
    if (Test-Path (Join-Path $root "LICENSE")) {
        Copy-Item -Path (Join-Path $root "LICENSE") -Destination (Join-Path $extensionDir "LICENSE") -Force
    }

    $iconPath = Join-Path $root "src\SmartGreeting.VsCode\icon.png"
    if (!(Test-Path $iconPath)) {
        $iconPath = Join-Path $root "src\SmartGreeting.Vsix\Resources\icon.png"
    }
    if (Test-Path $iconPath) {
        Copy-Item -Path $iconPath -Destination (Join-Path $extensionDir "icon.png") -Force
    }

    # 4. Generate extension.vsixmanifest at root of VSIX
    $manifestXml = '<?xml version="1.0" encoding="utf-8"?>
<PackageManifest Version="2.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2011" xmlns:d="http://schemas.microsoft.com/developer/vsx-schema-design/2011">
  <Metadata>
    <Identity Id="smartgreeting" Version="1.0.0" Language="en-US" Publisher="avi" />
    <DisplayName>SmartGreeting — Intelligent Greetings for VS Code</DisplayName>
    <Description xml:space="preserve">Modern, context-aware greetings with customizable working hours, day-of-week intelligence, anti-repetition, and Windows 11 Fluent design.</Description>
    <Icon>extension/icon.png</Icon>
  </Metadata>
  <Installation>
    <InstallationTarget Id="Microsoft.VisualStudio.Code" />
  </Installation>
  <Dependencies />
  <Assets>
    <Asset Type="Microsoft.VisualStudio.Code.Manifest" Path="extension/package.json" Addressable="true" />
    <Asset Type="Microsoft.VisualStudio.Services.Content.Details" Path="extension/README.md" Addressable="true" />
    <Asset Type="Microsoft.VisualStudio.Services.Content.License" Path="extension/LICENSE" Addressable="true" />
  </Assets>
</PackageManifest>'
    [System.IO.File]::WriteAllText((Join-Path $stagingDir "extension.vsixmanifest"), $manifestXml)

    # 5. Generate [Content_Types].xml at root of VSIX
    $contentTypesXml = '<?xml version="1.0" encoding="utf-8"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="vsixmanifest" ContentType="text/xml" />
  <Default Extension="json" ContentType="application/json" />
  <Default Extension="js" ContentType="application/javascript" />
  <Default Extension="md" ContentType="text/markdown" />
  <Default Extension="txt" ContentType="text/plain" />
  <Default Extension="png" ContentType="image/png" />
</Types>'
    [System.IO.File]::WriteAllText((Join-Path $stagingDir "[Content_Types].xml"), $contentTypesXml)

    # 6. Create VSIX Archive (ZIP compliant)
    Add-Type -AssemblyName System.IO.Compression.FileSystem

    # Create SmartGreeting.vsix
    if (Test-Path $vsixTargetFile) { Remove-Item $vsixTargetFile -Force }
    [System.IO.Compression.ZipFile]::CreateFromDirectory($stagingDir, $vsixTargetFile, [System.IO.Compression.CompressionLevel]::Optimal, $false)

    # Also keep explicit copy SmartGreeting-VSCode.vsix
    Copy-Item -Path $vsixTargetFile -Destination $vscodeVsixTargetFile -Force

    $sizeKb = [Math]::Round((Get-Item $vsixTargetFile).Length / 1KB, 1)
    Write-Host "==========================================================" -ForegroundColor Green
    Write-Host "   VS Code VSIX Package Successfully Generated!" -ForegroundColor Green
    Write-Host "   Target: $vsixTargetFile ($sizeKb KB)" -ForegroundColor Green
    Write-Host "   Explicit: $vscodeVsixTargetFile ($sizeKb KB)" -ForegroundColor Green
    Write-Host "==========================================================" -ForegroundColor Green
}
finally {
    if (Test-Path $stagingDir) {
        Remove-Item -Path $stagingDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}
