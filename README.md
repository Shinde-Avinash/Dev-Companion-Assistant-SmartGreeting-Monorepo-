# SmartGreeting — Intelligent Greetings for Visual Studio

> **Modern, context-aware greetings for Visual Studio with customizable working hours, day-of-week intelligence, anti-repetition, and Windows 11 Fluent design.**

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build & Test](https://img.shields.io/badge/Unit%20Tests-30%20Passed-brightgreen.svg)]()
[![Platform](https://img.shields.io/badge/Visual%20Studio-2022%20%7C%202019-purple.svg)]()
[![Offline First](https://img.shields.io/badge/Network-100%25%20Offline-success.svg)]()

---

## ⚡ Executive Summary — No Hosting / No Marketplace

**SmartGreeting** is intentionally engineered for **private, local use**.

- **Zero Cloud / Zero Hosting**: No web server, REST API, Firebase, Supabase, Azure, AWS, or database required.
- **No Marketplace Dependency**: You do not need a Visual Studio Marketplace publisher account, gallery listing, or public repository.
- **100% Offline-First**: Once built and installed, the extension operates completely self-contained on your workstation.
- **Production-Grade Architecture**: Designed using clean separation of concerns, testable time abstractions, modern WPF Fluent UI, and official Visual Studio extension integration.

---

## 🚀 Quick Start — Local Installation

The build process produces a single installable package:

```text
artifacts/SmartGreeting.vsix
```

### Recommended Workflow (Standard User)

```text
1. Run .\scripts\build.ps1
2. Navigate to artifacts\
3. Double-click SmartGreeting.vsix
4. The Visual Studio Extension Installer opens
5. Select your Visual Studio version (Community, Professional, or Enterprise)
6. Click Install / Modify
7. Restart Visual Studio
```

---

## 🔄 One-Command Developer Update Workflow

When making modifications to the extension, updating is as simple as running a single PowerShell command:

```powershell
.\scripts\update.ps1
```

Or to automatically bump the patch version (e.g., `1.0.0` → `1.0.1`):

```powershell
.\scripts\update.ps1 -BumpPatch
```

### What `update.ps1` does automatically:
1. *(Optional)* Bumps version in `Directory.Build.props` and `extension.vsixmanifest`.
2. Compiles `SmartGreeting.sln` with 0 warnings/errors.
3. Executes the complete unit test suite (30 automated tests).
4. Packages the updated binaries into `artifacts/SmartGreeting.vsix` conforming to Open Packaging Conventions (OPC).
5. Detects if Visual Studio is currently running (notifies you to save work to avoid assembly file locks).
6. Invokes the official Visual Studio VSIX installer.

---

## ⚠️ Important: Why Not Manually Copy to `%LocalAppData%\...\Extensions`?

You may wonder whether you can simply copy the compiled folder into:
```text
%LocalAppData%\Microsoft\VisualStudio\<version>\Extensions\
```

### The Technical Reality:
1. **Private Registry Merging (`privateregistry.bin`)**: Modern Visual Studio versions (2017, 2019, 2022) do not discover extensions by merely scanning directories. Extensions must be merged into Visual Studio's isolated private registry.
2. **PKGDEF Processing**: Visual Studio reads `.pkgdef` files during extension installation to register menus, command GUIDs, and `Tools -> Options` pages. Raw directory copying skips pkgdef compilation.
3. **Configuration Cache**: Visual Studio checks `extensions.configurationChanged`. Without a registered timestamp, raw directory copies are silently ignored or cause startup warning banners.

### The Supported Solution:
Use **`VSIXInstaller.exe`** (via `SmartGreeting.vsix` or `.\scripts\install.ps1`). It handles registry merging, isolated sandboxing, clean updates, and uninstallation safely without modifying Visual Studio's core installation directory.

---

## 🕒 Smart Greeting Logic & Time Periods

SmartGreeting reads your local system clock through an injectable `IDateTimeProvider`:

| Time Window | Period | Visual Theme | Example Headline & Message |
| :--- | :--- | :--- | :--- |
| **05:00 AM – 11:59 AM** | Morning | ☀️ Sunrise Amber Gold | *"Good Morning, Avi! ☀️ Ready to turn ideas into great code?"* |
| **12:00 PM – 04:59 PM** | Afternoon | 🌤️ Daylight Azure Blue | *"Good Afternoon, Avi! 🌤️ Keep the momentum going!"* |
| **05:00 PM – 08:59 PM** | Evening | 🌆 Sunset Coral / Violet | *"Good Evening, Avi! 🌆 Let's finish strong!"* |
| **09:00 PM – 04:59 AM** | Late Night | 🌙 Midnight Indigo Purple | *"Still coding at this hour, Avi? 🌙 Don't forget to take a break."* |

> *Late-night messages are thoughtfully crafted to be warm, friendly, and encouraging rather than judgmental.*

---

## 💼 Working Hours Awareness (`TimeContextService`)

Working hours are fully configurable (default: `09:00 AM` to `06:00 PM`):

- **`BeforeWork`**: Early bird sessions before your configured work hours.
- **`DuringWork`**: Daytime focus sessions within your scheduled hours.
- **`AfterWork`**: Evening wrap-up sessions after your work hours end.
- **`LateNight`**: Deep night coding sessions.
- **`Weekend`**: Saturday and Sunday passion coding.

All time calculations are decoupled from the UI in `TimeContextService` and tested against strict boundary seconds (`05:00:00`, `11:59:59`, `12:00:00`, `16:59:59`, `17:00:00`, `20:59:59`, `21:00:00`, `23:59:59`, `00:00:00`, `04:59:59`).

---

## 📅 Day-of-Week Intelligence

SmartGreeting dynamically injects tailored messages on key days:

- **Monday Motivation**:
  > *"Monday already, Avi? 😅 Let's make it productive and set a winning pace for the week."*
- **Friday Vibes**:
  > *"It's Friday, Avi! 🚀 Finish strong, merge cleanly, and prepare for a great weekend!"*
- **Weekend Warrior**:
  > *"Weekend coding, Avi? Respect. 💻 Working on passions and side projects? Enjoy the craft!"*

---

## 🎨 Modern Fluent Design UI

Visual Studio's standard `MessageBox.Show(...)` is prohibited. SmartGreeting renders a floating card inspired by **Windows 11 Fluent Design**:

- **Aesthetics**: Curved corners (`CornerRadius="16"`), acrylic dark gradient, subtle Gaussian drop shadow, clean Segoe UI Variable typography.
- **Animations**:
  - Smooth slide-up entrance (`TranslateTransform` + `DoubleAnimation`).
  - Smooth opacity fade-in.
  - Interactive auto-dismiss countdown bar at the bottom.
  - **Smart Pause**: Hovering the mouse over the card pauses the timer so you can read at your leisure.
  - Smooth slide-down fade-out exit.
- **High-DPI Ready**: Fully vector-rendered with `UseLayoutRounding="True"` and `ClearType` rendering.

---

## 🛠️ Settings (`Tools -> Options -> SmartGreeting`)

Configurable directly from within Visual Studio:

```text
Tools
  └── Options
        └── SmartGreeting
              ├── Enable SmartGreeting (default: true)
              ├── Your Name (default: "Avi" — falls back to "Developer" if empty)
              ├── Working Hours Start (default: 09:00)
              ├── Working Hours End (default: 18:00)
              ├── Show on Visual Studio Startup (default: true)
              ├── Show on Solution Open (default: false)
              ├── Greeting Frequency:
              │     ├── Once per Visual Studio session (default)
              │     ├── Once per day
              │     ├── Every Visual Studio startup
              │     └── Manual only
              ├── Late Night Messages (default: true)
              ├── Weekend Messages (default: true)
              ├── Card Animations (default: true)
              ├── Sound Effects (default: false)
              └── Auto-Dismiss Seconds (default: 8)
```

Settings are persisted locally at:
```text
%LOCALAPPDATA%\SmartGreeting\settings.json
```

---

## 📌 Visual Studio Commands

SmartGreeting registers standard Visual Studio menu commands:

```text
Extensions
  └── SmartGreeting
        ├── Show Greeting   (Command: SmartGreeting.ShowGreeting)
        ├── Settings        (Opens Configuration UI)
        └── About           (Displays Version & Feature Info)
```

You can trigger a greeting at any time using **Extensions → SmartGreeting → Show Greeting**.

---

## 🧪 Interactive Preview Studio (`SmartGreeting.Preview`)

Don't want to wait for Visual Studio to open just to test the UI? Run our standalone preview application:

```powershell
$env:DOTNET_ROOT = "$env:LOCALAPPDATA\dotnet"
& "src\SmartGreeting.Preview\bin\Release\net8.0-windows\SmartGreeting.Preview.exe"
```

The Preview Studio provides instant one-click buttons to test:
- ☀️ Morning Greeting
- 🌤️ Afternoon Greeting
- 🌆 Evening Greeting
- 🌙 Late Night Greeting
- ☕ Monday Motivation
- 🚀 Friday Vibes
- 💻 Weekend Warrior
- ⏱️ Live System Clock Greeting

---

## 🔬 Unit Tests

All time logic, boundary seconds, frequency limiting, anti-repetition, and name personalizations are covered by 30 automated xUnit tests in `tests/SmartGreeting.Tests`:

```powershell
.\scripts\build.ps1 -Configuration Release
```

Output:
```text
Passed!  - Failed: 0, Passed: 30, Skipped: 0, Total: 30, Duration: 35 ms - SmartGreeting.Tests.dll
```

### Tested Scenarios:
1. `05:00:00` → Morning
2. `11:59:59` → Morning
3. `12:00:00` → Afternoon
4. `16:59:59` → Afternoon
5. `17:00:00` → Evening
6. `20:59:59` → Evening
7. `21:00:00` → LateNight
8. `23:59:59` → LateNight
9. `00:00:00` → LateNight
10. `04:59:59` → LateNight
11. Monday, Friday, Saturday, Sunday detection
12. Custom working hours (`BeforeWork`, `DuringWork`, `AfterWork`)
13. Personalization with name `"Avi"` vs empty fallback `"Developer"`
14. Anti-repetition (prevents identical consecutive message)
15. Frequency limits (`OncePerSession`, `OncePerDay`, `ManualOnly`)

---

## 📁 Repository Structure

```text
SmartGreeting/
├── SmartGreeting.sln                   # Visual Studio Solution
├── Directory.Build.props               # Central version management (1.0.0)
├── LICENSE                             # MIT License
├── README.md                           # Documentation
│
├── artifacts/
│   └── SmartGreeting.vsix             # Production VSIX Extension Package
│
├── src/
│   ├── SmartGreeting.Core/             # Business Logic & Models (.NET Standard 2.0 / .NET 8)
│   │   ├── Models/
│   │   │   ├── Enums.cs                # TimePeriod, WorkContext, Frequency, Theme
│   │   │   ├── GreetingMessage.cs      # Message structure & personalization
│   │   │   ├── GreetingSettings.cs     # Settings & defaults
│   │   │   └── TimeContext.cs          # Time context evaluation model
│   │   └── Services/
│   │       ├── IDateTimeProvider.cs    # Clock abstraction for testability
│   │       ├── ILoggerService.cs       # Fail-safe local file logging
│   │       ├── ITimeContextService.cs  # Time boundaries & working hours logic
│   │       ├── IMessageService.cs      # Dynamic message library & anti-repetition
│   │       ├── ISettingsService.cs     # JSON settings persistence
│   │       └── IGreetingCoordinator.cs # Coordination & frequency rules
│   │
│   ├── SmartGreeting.UI/               # Windows 11 Fluent UI (WPF)
│   │   ├── GreetingViewModel.cs        # Adaptive themes & animation bindings
│   │   ├── GreetingWindow.xaml         # Modern floating card XAML
│   │   └── GreetingWindow.xaml.cs      # Smooth slide/fade animations & auto-dismiss
│   │
│   ├── SmartGreeting.Vsix/             # Visual Studio Extension & Manifest
│   │   ├── source.extension.vsixmanifest# VSIX v3 manifest targeting VS 2022 & 2019
│   │   ├── SmartGreeting.pkgdef        # VS private registry registration
│   │   ├── [Content_Types].xml         # Open Packaging Conventions
│   │   ├── SmartGreetingPackage.cs     # Extension entry point & async lifecycle
│   │   ├── Commands/                   # ShowGreeting, Settings, About commands
│   │   ├── UI/                         # Options Control & About Window
│   │   └── Resources/                  # icon.png, preview.png
│   │
│   └── SmartGreeting.Preview/          # Standalone Preview Runner
│       ├── MainWindow.xaml             # Interactive testing studio
│       └── MainWindow.xaml.cs
│
├── tests/
│   └── SmartGreeting.Tests/            # xUnit Test Suite (30 tests)
│       ├── TimeContextServiceTests.cs
│       ├── GreetingMessageServiceTests.cs
│       └── GreetingCoordinatorTests.cs
│
└── scripts/
    ├── build.ps1                       # Builds solution & executes tests
    ├── package.ps1                     # Packages OPC VSIX archive
    ├── install.ps1                     # Local installation & VS process check
    └── update.ps1                      # One-command developer update workflow
```

---

## 🪵 Diagnostics & Logging

SmartGreeting is built to **never block or crash Visual Studio**. All errors and diagnostics are written to:

```text
%LOCALAPPDATA%\SmartGreeting\Logs\smartgreeting.log
```

---

## 🛡️ License

This project is licensed under the [MIT License](LICENSE).
