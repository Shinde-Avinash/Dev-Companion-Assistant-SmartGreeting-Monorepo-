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

    $vscodeReadme = Join-Path $srcDir "README.md"
    if (Test-Path $vscodeReadme) {
        Copy-Item -Path $vscodeReadme -Destination (Join-Path $extensionDir "README.md") -Force
    } elseif (Test-Path (Join-Path $root "README.md")) {
        Copy-Item -Path (Join-Path $root "README.md") -Destination (Join-Path $extensionDir "README.md") -Force
    }
    if (Test-Path (Join-Path $root "LICENSE")) {
        Copy-Item -Path (Join-Path $root "LICENSE") -Destination (Join-Path $extensionDir "LICENSE.txt") -Force
    }

    $iconPath = Join-Path $root "src\SmartGreeting.VsCode\icon.png"
    if (!(Test-Path $iconPath)) {
        $iconPath = Join-Path $root "src\SmartGreeting.Vsix\Resources\icon.png"
    }
    if (Test-Path $iconPath) {
        Copy-Item -Path $iconPath -Destination (Join-Path $extensionDir "icon.png") -Force
    }

    # 4. Generate extension.vsixmanifest at root of VSIX exactly matching VS Code vsce template
    $manifestXml = '<?xml version="1.0" encoding="utf-8"?>
	<PackageManifest Version="2.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2011" xmlns:d="http://schemas.microsoft.com/developer/vsx-schema-design/2011">
		<Metadata>
			<Identity Language="en-US" Id="dev-companion-assistant" Version="1.3.1" Publisher="AvinashShinde"/>
			<DisplayName>Dev Companion Assistant</DisplayName>
			<Description xml:space="preserve">All-in-one developer companion: Context-aware greetings, motivational thoughts, dev humor, plus scheduled water, coffee, lunch, and posture wellness reminders.</Description>
			<Tags>greeting,wellness,water-reminder,coffee-break,lunch-reminder,posture,health,productivity,fluent</Tags>
			<Categories>Other</Categories>
			<GalleryFlags>Public</GalleryFlags>
			
			<Properties>
				<Property Id="Microsoft.VisualStudio.Code.Engine" Value="^1.75.0" />
				<Property Id="Microsoft.VisualStudio.Code.ExtensionDependencies" Value="" />
				<Property Id="Microsoft.VisualStudio.Code.ExtensionPack" Value="" />
				<Property Id="Microsoft.VisualStudio.Code.ExtensionKind" Value="workspace,web" />
				<Property Id="Microsoft.VisualStudio.Code.LocalizedLanguages" Value="" />
				<Property Id="Microsoft.VisualStudio.Code.EnabledApiProposals" Value="" />
				
				<Property Id="Microsoft.VisualStudio.Code.ExecutesCode" Value="true" />
				
				<Property Id="Microsoft.VisualStudio.Services.Links.Source" Value="https://github.com/AvinashShinde/smart-greeting-assistant" />
				<Property Id="Microsoft.VisualStudio.Services.Links.Getstarted" Value="https://github.com/AvinashShinde/smart-greeting-assistant" />
				<Property Id="Microsoft.VisualStudio.Services.Links.GitHub" Value="https://github.com/AvinashShinde/smart-greeting-assistant" />
				<Property Id="Microsoft.VisualStudio.Services.Links.Support" Value="https://github.com/AvinashShinde/smart-greeting-assistant/issues" />
				
				<Property Id="Microsoft.VisualStudio.Services.GitHubFlavoredMarkdown" Value="true" />
				<Property Id="Microsoft.VisualStudio.Services.Content.Pricing" Value="Free"/>
			</Properties>
			<License>extension/LICENSE.txt</License>
			<Icon>extension/icon.png</Icon>
		</Metadata>
		<Installation>
			<InstallationTarget Id="Microsoft.VisualStudio.Code"/>
		</Installation>
		<Dependencies/>
		<Assets>
			<Asset Type="Microsoft.VisualStudio.Code.Manifest" Path="extension/package.json" Addressable="true" />
			<Asset Type="Microsoft.VisualStudio.Services.Content.Details" Path="extension/README.md" Addressable="true" />
			<Asset Type="Microsoft.VisualStudio.Services.Content.License" Path="extension/LICENSE.txt" Addressable="true" />
			<Asset Type="Microsoft.VisualStudio.Services.Icons.Default" Path="extension/icon.png" Addressable="true" />
		</Assets>
	</PackageManifest>'

    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText((Join-Path $stagingDir "extension.vsixmanifest"), $manifestXml, $utf8NoBom)

    # 5. Generate [Content_Types].xml matching vsce format
    $contentTypesXml = '<?xml version="1.0" encoding="utf-8"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension=".js" ContentType="application/javascript"/><Default Extension=".json" ContentType="application/json"/><Default Extension=".md" ContentType="text/markdown"/><Default Extension=".png" ContentType="image/png"/><Default Extension=".txt" ContentType="text/plain"/><Default Extension=".vsixmanifest" ContentType="text/xml"/></Types>
'
    [System.IO.File]::WriteAllText((Join-Path $stagingDir "[Content_Types].xml"), $contentTypesXml, $utf8NoBom)

    # 6. Create VSIX Archive with strict POSIX forward-slash separators (mandatory for Marketplace ZIP parser)
    Add-Type -AssemblyName System.IO.Compression
    Add-Type -AssemblyName System.IO.Compression.FileSystem

    if (Test-Path $vsixTargetFile) { Remove-Item $vsixTargetFile -Force }
    if (Test-Path $vscodeVsixTargetFile) { Remove-Item $vscodeVsixTargetFile -Force }

    $zipStream = [System.IO.File]::Open($vsixTargetFile, [System.IO.FileMode]::Create)
    $archive = New-Object System.IO.Compression.ZipArchive($zipStream, [System.IO.Compression.ZipArchiveMode]::Create)

    # Get all files in staging directory and add them with normalized forward slashes
    $allFiles = Get-ChildItem -Path $stagingDir -Recurse -File
    foreach ($file in $allFiles) {
        $relativePath = $file.FullName.Substring($stagingDir.Length).TrimStart('\', '/')
        $entryName = $relativePath.Replace('\', '/')
        
        $entry = $archive.CreateEntry($entryName, [System.IO.Compression.CompressionLevel]::Optimal)
        $fileStream = [System.IO.File]::OpenRead($file.FullName)
        $entryStream = $entry.Open()
        $fileStream.CopyTo($entryStream)
        $entryStream.Dispose()
        $fileStream.Dispose()
    }

    $archive.Dispose()
    $zipStream.Dispose()

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
