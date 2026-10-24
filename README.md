# Dev Companion Assistant (SmartGreeting Monorepo)

> **All-in-one developer companion for Visual Studio Code & Visual Studio (2022/2019): Context-aware greetings, motivational thoughts, dev humor, plus scheduled hydration, coffee, lunch, and ergonomic posture wellness reminders.**

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![VS Code Marketplace](https://img.shields.io/badge/VS%20Code%20Marketplace-v1.3.1-blue.svg)](https://marketplace.visualstudio.com/items?itemName=AvinashShinde.dev-companion-assistant)
[![Platform](https://img.shields.io/badge/Platform-VS%20Code%20%7C%20Visual%20Studio-purple.svg)]()
[![Offline First](https://img.shields.io/badge/Network-100%25%20Offline-success.svg)]()

---
<img width="1314" height="626" alt="image" src="https://github.com/user-attachments/assets/1bb973b8-104e-48b5-a8ac-a862d7f97651" />

## 👨‍💻 Creator & Community

Created with ❤️ by **Avinash Shinde**. Connect, collaborate, and explore:

- 🔗 **LinkedIn**: [https://www.linkedin.com/in/avinash-shinde-7628b91a5/](https://www.linkedin.com/in/avinash-shinde-7628b91a5/)
- 🌐 **GitHub**: [https://github.com/Shinde-Avinash](https://github.com/Shinde-Avinash)

Feel free to star the repository and connect on LinkedIn!

---

## 📦 Monorepo Overview

This repository hosts both flavors of Dev Companion:

| Edition | Directory | Distribution / Package | Target Environment |
| :--- | :--- | :--- | :--- |
| **VS Code Extension** | `src/SmartGreeting.VsCode/` | `artifacts/SmartGreeting.vsix` (VS Code Marketplace) | VS Code 1.75.0+ (Windows, macOS, Linux, Web) |
| **Visual Studio IDE Extension** | `src/SmartGreeting.Vsix/` | `artifacts/SmartGreeting.vsix` (Visual Studio VSIX) | Visual Studio 2022 & 2019 (Community, Pro, Enterprise) |
| **Shared Core & Models** | `src/SmartGreeting.Core/` | .NET Standard 2.0 / .NET 8.0 Class Library | Cross-platform business logic & services |
| **Preview Studio** | `src/SmartGreeting.Preview/` | Windows WPF / net8.0-windows Executable | Standalone live design & testing harness |
| **Unit Test Suite** | `tests/SmartGreeting.Tests/` | 30 xUnit automated test specifications | CI / CD validation of time & frequency rules |

---

## ✨ Features & Capabilities

1. **Smart Context-Aware Greetings**: Personalized greetings based on your local clock (Morning ☀️, Afternoon 🌤️, Evening 🌆, Late Night 🌙, and Weekend Warrior 💻).
2. **Hydration Reminders (Water Breaks)**: Periodically reminds you to drink water (every 45 mins) and tracks your daily glasses goal (8 glasses / 2 Liters).
3. **Coffee & Tea Breaks**: Encourages taking a short 5-minute break after 90 minutes of continuous coding.
4. **Lunch Reminder**: Alerts you at lunch hour (default `13:00`) so you never skip meals while debugging.
5. **Ergonomic Posture & Eye Checks**: Prompts you every 30 minutes with the 20-20-20 eye rest rule, shoulder blade squeezes, and gentle neck rolls.
6. **Daily Motivational Thoughts & Dev Humor**: Curated quotes and clean developer jokes to brighten your workday.
7. **Personalized Name Onboarding**: Asks for your name on first install and greets you personally. Change your name anytime with a single click.
8. **100% Offline-First**: Zero external telemetry, zero API keys, and zero cloud hosting required.

---

## 🚀 Quick Start — Visual Studio Code

### Method 1: Install from VS Code Marketplace
Search for **`Dev Companion Assistant`** by **Avinash Shinde** in the Extensions view (`Ctrl+Shift+X` / `Cmd+Shift+X`), then click **Install**.

### Method 2: Install VSIX Package Locally
```bash
code --install-extension artifacts/SmartGreeting.vsix
```

### Build & Package VS Code VSIX
```powershell
powershell -ExecutionPolicy Bypass -File "scripts/package-vscode.ps1"
```

---

## 🛠️ Quick Start — Visual Studio (2022 / 2019)

### 1. Build & Test
```powershell
.\scripts\build.ps1 -Configuration Release
```

### 2. Install VSIX Package
Double-click `artifacts\SmartGreeting.vsix` or execute:
```powershell
.\scripts\install.ps1
```

### 3. One-Command Developer Workflow
```powershell
.\scripts\update.ps1 -BumpPatch
```

---

## ⚙️ Configuration Settings

Customize behavior in **Settings (`Ctrl+,`) → Extensions → DevCompanion**:

| Setting | Default | Description |
| :--- | :--- | :--- |
| `smartGreeting.name` | `"Avi"` | Your name for personalized greetings and health alerts. |
| `smartGreeting.displayStyle` | `"floatingCard"` | Modal card, standard toast notification, or both. |
| `smartGreeting.autoCloseSeconds` | `45` | Auto-dismiss duration in seconds (0 = keep open). |
| `smartGreeting.waterReminder` | `true` | Enable periodic water hydration alerts. |
| `smartGreeting.waterIntervalMinutes`| `45` | Minutes between hydration alerts. |
| `smartGreeting.waterDailyGoal` | `8` | Target glasses of water per day. |
| `smartGreeting.coffeeReminder` | `true` | Enable coffee & tea break alerts. |
| `smartGreeting.coffeeIntervalMinutes`| `90` | Minutes between coffee break alerts. |
| `smartGreeting.lunchReminder` | `true` | Enable lunch break reminder. |
| `smartGreeting.lunchTime` | `"13:00"` | Scheduled lunch time in 24-hr format. |
| `smartGreeting.postureReminder` | `true` | Enable posture & eye rest checks. |
| `smartGreeting.postureIntervalMinutes`| `30` | Minutes between posture checks. |

---

## 📁 Repository Structure

```text
Smart Greeting Assistant/
├── .gitignore                          # Comprehensive Git ignore rules
├── LICENSE                             # MIT License
├── README.md                           # Monorepo architecture & quick start
│
├── artifacts/
│   ├── SmartGreeting.vsix              # Installable VSIX package (VS Code & Visual Studio)
│   └── SmartGreeting-VSCode.vsix       # Explicit VS Code package copy
│
├── scripts/
│   ├── build.ps1                       # Compiles C# solution & runs unit tests
│   ├── package-vscode.ps1              # Validates & packages VS Code VSIX
│   ├── install.ps1                     # Automated local VSIX installer
│   └── update.ps1                      # Auto-bump patch & one-command reload
│
├── src/
│   ├── SmartGreeting.VsCode/           # Visual Studio Code Extension
│   │   ├── extension.js                # Core extension lifecycle, timers & webview UI
│   │   ├── icon.png                    # 3D Glassmorphic extension icon (128x128)
│   │   ├── package.json                # VS Code extension manifest & contributions
│   │   └── README.md                   # Marketplace listing page documentation
│   │
│   ├── SmartGreeting.Core/             # .NET Standard 2.0 / .NET 8 Business Logic
│   │   ├── Models/                     # GreetingMessage, Settings, TimeContext, Enums
│   │   └── Services/                   # DateTimeProvider, Logger, TimeContext, Message
│   │
│   ├── SmartGreeting.UI/               # Windows 11 Fluent UI (WPF)
│   │   ├── GreetingViewModel.cs        # Adaptive themes & animation bindings
│   │   ├── GreetingWindow.xaml         # Modern floating card UI
│   │   └── GreetingWindow.xaml.cs      # Smooth slide/fade animations
│   │
│   ├── SmartGreeting.Vsix/             # Visual Studio IDE Extension (2022/2019)
│   │   ├── source.extension.vsixmanifest# VSIX v3 manifest targeting VS 2022 & 2019
│   │   ├── SmartGreetingPackage.cs     # Extension async initialization
│   │   └── UI/                         # Options Control & About Window (with Creator tab)
│   │
│   └── SmartGreeting.Preview/          # Standalone Runner
│       └── MainWindow.xaml             # Interactive testing studio for greeting card
│
└── tests/
    └── SmartGreeting.Tests/            # xUnit Test Suite (30 unit tests)
```

---

## 🔬 Unit Tests

All time logic, boundary seconds, frequency limiting, anti-repetition, and name personalizations are covered by 30 automated xUnit tests in `tests/SmartGreeting.Tests`:

```powershell
.\scripts\build.ps1 -Configuration Release
```

Output:
```text
Passed!  - Failed: 0, Passed: 30, Skipped: 0, Total: 30 - SmartGreeting.Tests.dll
```

---

## 🛡️ License

This project is licensed under the [MIT License](LICENSE). Free and open-source forever.
