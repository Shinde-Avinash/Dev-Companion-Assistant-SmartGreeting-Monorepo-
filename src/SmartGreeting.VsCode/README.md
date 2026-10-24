# Dev Companion Assistant

> **All-in-one developer companion: Context-aware greetings, motivational thoughts, dev humor, plus scheduled water, coffee, lunch, and posture wellness reminders.**

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.txt)
[![Version](https://img.shields.io/badge/Version-1.3.1-brightgreen.svg)]()
[![Platform](https://img.shields.io/badge/VS%20Code-%5E1.75.0-purple.svg)]()
[![Offline First](https://img.shields.io/badge/Network-100%25%20Offline-success.svg)]()

---

## 👨‍💻 Creator & Community

Created with ❤️ by **Avinash Shinde**. Connect, collaborate, and explore:

- 🔗 **LinkedIn**: [https://www.linkedin.com/in/avinash-shinde-7628b91a5/](https://www.linkedin.com/in/avinash-shinde-7628b91a5/)
- 🌐 **GitHub**: [https://github.com/Shinde-Avinash](https://github.com/Shinde-Avinash)

Feel free to star the project on GitHub and connect on LinkedIn!

---

## ✨ Why DevCompanion?

As developers, we often enter deep flow states for hours without standing up, resting our eyes, or drinking water. **DevCompanion** acts as your caring personal co-pilot right inside Visual Studio Code:

1. **Smart Context-Aware Greetings**: Personalized greetings based on your local clock (Morning ☀️, Afternoon 🌤️, Evening 🌆, Late Night 🌙, and Weekend Warrior 💻).
2. **Hydration Reminders (Water Breaks)**: Periodically reminds you to drink water (every 45 mins) and tracks your daily glasses goal (8 glasses / 2 Liters).
3. **Coffee & Tea Breaks**: Encourages taking a short 5-minute break after 90 minutes of continuous coding.
4. **Lunch Reminder**: Alerts you at lunch hour (default `13:00`) so you never skip meals while debugging.
5. **Ergonomic Posture & Eye Checks**: Prompts you every 30 minutes with the 20-20-20 eye rest rule, shoulder squeezes, and gentle neck rolls.
6. **Daily Motivational Thoughts & Dev Humor**: Curated quotes and clean developer jokes to brighten your workday.
7. **Personalized Name Onboarding**: Asks for your name on first install and greets you personally. Change your name anytime with a single click.

---

## 🚀 Quick Start & Usage

1. Open VS Code — DevCompanion automatically welcomes you with your daily dashboard.
2. Click the status bar icon in the bottom right corner (`💧 0/8 | ☕ 90m`) to open the DevCompanion dashboard anytime.
3. Access commands via Command Palette (`Ctrl+Shift+P` / `Cmd+Shift+P`):
   - `DevCompanion: Show Dashboard & Greeting`
   - `DevCompanion: Log Glass of Water 💧`
   - `DevCompanion: Start Coffee/Tea Break ☕`
   - `DevCompanion: Quick Ergonomic Stretch Guide 🧘`
   - `DevCompanion: Change Your Name ✏️`
   - `DevCompanion: Configure Settings ⚙️`

---

## ⚙️ Configuration Settings

Customize all behavior in **Settings (`Ctrl+,`) → Extensions → DevCompanion**:

| Setting | Default | Description |
| :--- | :--- | :--- |
| `smartGreeting.name` | `"Avi"` | Your name for personalized greetings and health alerts. |
| `smartGreeting.displayStyle` | `"floatingCard"` | Modal card or standard toast notification. |
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

## 🛡️ License

Licensed under the [MIT License](LICENSE.txt). Free and open-source forever.
