const vscode = require('vscode');

// --- Message Library ---
const MESSAGES = [
  // Morning (05:00 - 11:59)
  {
    id: "m-1",
    period: "Morning",
    title: "☀️ GOOD MORNING",
    headlineTemplate: "Good Morning, {Name}!",
    body: "Ready to turn ideas into great code?",
    subtle: "Every masterwork begins with a single line of clean code.",
    icon: "☀️",
    accent: "#FFA000",
    gradient: "linear-gradient(135deg, rgba(255, 160, 0, 0.18) 0%, rgba(255, 215, 64, 0.05) 100%)"
  },
  {
    id: "m-2",
    period: "Morning",
    title: "☀️ GOOD MORNING",
    headlineTemplate: "Rise and shine, {Name}!",
    body: "Fresh day, fresh canvas. What are we building today?",
    subtle: "Take your time, write with intention, and enjoy the flow.",
    icon: "☕",
    accent: "#FFB300",
    gradient: "linear-gradient(135deg, rgba(255, 179, 0, 0.18) 0%, rgba(255, 224, 130, 0.05) 100%)"
  },
  {
    id: "m-3",
    period: "Morning",
    title: "☀️ GOOD MORNING",
    headlineTemplate: "Welcome back, {Name}!",
    body: "Your coffee is hot and the editor is ready.",
    subtle: "Pro tip: Clear thoughts make clean abstractions.",
    icon: "🚀",
    accent: "#FFA000",
    gradient: "linear-gradient(135deg, rgba(255, 160, 0, 0.18) 0%, rgba(255, 215, 64, 0.05) 100%)"
  },

  // Afternoon (12:00 - 16:59)
  {
    id: "a-1",
    period: "Afternoon",
    title: "🌤️ GOOD AFTERNOON",
    headlineTemplate: "Good Afternoon, {Name}!",
    body: "Keep the momentum going!",
    subtle: "Midday progress fuels evening satisfaction.",
    icon: "🌤️",
    accent: "#1976D2",
    gradient: "linear-gradient(135deg, rgba(25, 118, 210, 0.18) 0%, rgba(100, 181, 246, 0.05) 100%)"
  },
  {
    id: "a-2",
    period: "Afternoon",
    title: "🌤️ GOOD AFTERNOON",
    headlineTemplate: "In the zone, {Name}?",
    body: "You're making great strides today. Stay focused and steady.",
    subtle: "Remember to stay hydrated and stretch your shoulders.",
    icon: "⚡",
    accent: "#0288D1",
    gradient: "linear-gradient(135deg, rgba(2, 136, 209, 0.18) 0%, rgba(79, 195, 247, 0.05) 100%)"
  },

  // Evening (17:00 - 20:59)
  {
    id: "e-1",
    period: "Evening",
    title: "🌆 GOOD EVENING",
    headlineTemplate: "Good Evening, {Name}!",
    body: "Let's finish strong!",
    subtle: "Wrap up loose ends and set yourself up for tomorrow.",
    icon: "🌆",
    accent: "#F4511E",
    gradient: "linear-gradient(135deg, rgba(244, 81, 30, 0.18) 0%, rgba(255, 138, 101, 0.05) 100%)"
  },
  {
    id: "e-2",
    period: "Evening",
    title: "🌆 GOOD EVENING",
    headlineTemplate: "Sunset coding, {Name}!",
    body: "Reflect on today's commits. You did meaningful work.",
    subtle: "Don't forget to push your branches before signing off.",
    icon: "🌇",
    accent: "#E64A19",
    gradient: "linear-gradient(135deg, rgba(230, 74, 25, 0.18) 0%, rgba(255, 112, 67, 0.05) 100%)"
  },

  // Late Night (21:00 - 04:59)
  {
    id: "ln-1",
    period: "LateNight",
    title: "🌙 LATE NIGHT",
    headlineTemplate: "Still coding at this hour, {Name}?",
    body: "Don't forget to take a break. Your rest matters most.",
    subtle: "Night silence brings focus, but sleep brings clarity.",
    icon: "🌙",
    accent: "#7E57C2",
    gradient: "linear-gradient(135deg, rgba(126, 87, 194, 0.22) 0%, rgba(179, 157, 219, 0.05) 100%)"
  },
  {
    id: "ln-2",
    period: "LateNight",
    title: "🌙 LATE NIGHT",
    headlineTemplate: "Burning the midnight oil, {Name}?",
    body: "Deep night creativity is real, but pace yourself.",
    subtle: "Save your work, rest your eyes, and recharge soon.",
    icon: "🌌",
    accent: "#673AB7",
    gradient: "linear-gradient(135deg, rgba(103, 58, 183, 0.22) 0%, rgba(149, 117, 205, 0.05) 100%)"
  },

  // Monday Special
  {
    id: "sp-monday",
    period: "Morning",
    tag: "Monday",
    title: "☕ MONDAY MOTIVATION",
    headlineTemplate: "Monday already, {Name}? 😅",
    body: "Let's make it productive and set a winning pace for the week.",
    subtle: "One task at a time. You've got this!",
    icon: "🎯",
    accent: "#00ACC1",
    gradient: "linear-gradient(135deg, rgba(0, 172, 193, 0.20) 0%, rgba(77, 208, 225, 0.05) 100%)"
  },

  // Friday Special
  {
    id: "sp-friday",
    period: "Afternoon",
    tag: "Friday",
    title: "🚀 FRIDAY VIBES",
    headlineTemplate: "It's Friday, {Name}! 🚀",
    body: "Finish strong, merge cleanly, and prepare for a great weekend!",
    subtle: "No dangerous deployments on Friday afternoon! 😉",
    icon: "🎉",
    accent: "#43A047",
    gradient: "linear-gradient(135deg, rgba(67, 160, 71, 0.20) 0%, rgba(129, 199, 132, 0.05) 100%)"
  },

  // Weekend Special
  {
    id: "sp-weekend",
    period: "Afternoon",
    tag: "Weekend",
    title: "💻 WEEKEND WARRIOR",
    headlineTemplate: "Weekend coding, {Name}? Respect. 💻",
    body: "Working on passions and side projects? Enjoy the craft!",
    subtle: "Don't forget to step outside and take some time away from screens.",
    icon: "🌟",
    accent: "#AB47BC",
    gradient: "linear-gradient(135deg, rgba(171, 71, 188, 0.20) 0%, rgba(206, 147, 216, 0.05) 100%)"
  }
];

let shownInThisSession = false;

function getTimeContext() {
  const now = new Date();
  const hour = now.getHours();
  const minutes = now.getMinutes();
  const totalMinutes = hour * 60 + minutes;
  const day = now.getDay(); // 0 = Sunday, 1 = Monday, 5 = Friday, 6 = Saturday

  let period = "LateNight";
  if (totalMinutes >= 5 * 60 && totalMinutes < 12 * 60) {
    period = "Morning";
  } else if (totalMinutes >= 12 * 60 && totalMinutes < 17 * 60) {
    period = "Afternoon";
  } else if (totalMinutes >= 17 * 60 && totalMinutes < 21 * 60) {
    period = "Evening";
  }

  return {
    now,
    period,
    isMonday: day === 1,
    isFriday: day === 5,
    isWeekend: day === 0 || day === 6
  };
}

function selectMessage(context, config, lastMessageId) {
  let candidates = [];

  if (context.isMonday && config.get('weekendMessages', true)) {
    candidates.push(...MESSAGES.filter(m => m.tag === "Monday"));
  } else if (context.isFriday && config.get('weekendMessages', true)) {
    candidates.push(...MESSAGES.filter(m => m.tag === "Friday"));
  } else if (context.isWeekend && config.get('weekendMessages', true)) {
    candidates.push(...MESSAGES.filter(m => m.tag === "Weekend"));
  }

  let targetPeriod = context.period;
  if (targetPeriod === "LateNight" && !config.get('lateNightMessages', true)) {
    targetPeriod = "Evening";
  }

  const periodPool = MESSAGES.filter(m => m.period === targetPeriod && !m.tag);
  candidates.push(...periodPool);

  if (candidates.length === 0) {
    candidates = MESSAGES;
  }

  if (lastMessageId && candidates.length > 1) {
    const withoutLast = candidates.filter(m => m.id !== lastMessageId);
    if (withoutLast.length > 0) candidates = withoutLast;
  }

  const selected = candidates[Math.floor(Math.random() * candidates.length)];
  const rawName = config.get('name', 'Avi');
  const displayName = (!rawName || !rawName.trim()) ? "Developer" : rawName.trim();
  const headline = selected.headlineTemplate.replace("{Name}", displayName);

  return {
    ...selected,
    headline,
    displayName
  };
}

function showFloatingCard(message) {
  const panel = vscode.window.createWebviewPanel(
    'smartGreetingCard',
    'SmartGreeting',
    { viewColumn: vscode.ViewColumn.Beside, preserveFocus: true },
    { enableScripts: true }
  );

  panel.webview.html = `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <style>
    * { box-sizing: border-box; margin: 0; padding: 0; }
    body {
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI Variable Text", "Segoe UI", Roboto, sans-serif;
      background: transparent;
      color: var(--vscode-editor-foreground, #eceff4);
      display: flex;
      align-items: center;
      justify-content: center;
      min-height: 100vh;
      padding: 16px;
    }
    .card {
      width: 100%;
      max-width: 440px;
      background: var(--vscode-editor-background, #1e1e24);
      border: 1.5px solid ${message.accent};
      border-radius: 16px;
      padding: 24px 22px;
      box-shadow: 0 16px 36px rgba(0, 0, 0, 0.45);
      background-image: ${message.gradient};
      animation: slideUp 0.35s cubic-bezier(0.16, 1, 0.3, 1);
      position: relative;
      overflow: hidden;
    }
    @keyframes slideUp {
      from { transform: translateY(20px); opacity: 0; }
      to { transform: translateY(0); opacity: 1; }
    }
    .tag-row {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 12px;
    }
    .icon-badge {
      font-size: 14px;
      background: ${message.accent};
      color: #111;
      padding: 3px 8px;
      border-radius: 8px;
      font-weight: bold;
    }
    .tag-title {
      font-size: 11px;
      font-weight: 700;
      color: ${message.accent};
      letter-spacing: 0.5px;
    }
    .headline {
      font-size: 20px;
      font-weight: 600;
      margin-bottom: 8px;
      line-height: 1.25;
      color: var(--vscode-editor-foreground, #fff);
    }
    .body-text {
      font-size: 13.5px;
      opacity: 0.92;
      line-height: 1.45;
      margin-bottom: 14px;
    }
    .subtle-box {
      background: rgba(255, 255, 255, 0.05);
      border-left: 2.5px solid ${message.accent};
      border-radius: 0 8px 8px 0;
      padding: 8px 12px;
      font-size: 12px;
      font-style: italic;
      opacity: 0.82;
      margin-bottom: 18px;
    }
    .footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .brand {
      font-size: 11px;
      opacity: 0.65;
      font-weight: 500;
    }
    .btn-action {
      background: ${message.accent};
      color: #111;
      border: none;
      padding: 6px 14px;
      border-radius: 6px;
      font-weight: 600;
      font-size: 12px;
      cursor: pointer;
    }
    .btn-action:hover { opacity: 0.88; }
    .progress-bar {
      position: absolute;
      bottom: 0;
      left: 0;
      height: 3px;
      background: ${message.accent};
      width: 100%;
      animation: countdown 10s linear forwards;
    }
    @keyframes countdown {
      from { width: 100%; }
      to { width: 0%; }
    }
  </style>
</head>
<body>
  <div class="card">
    <div class="tag-row">
      <span class="icon-badge">${message.icon}</span>
      <span class="tag-title">${message.title}</span>
    </div>
    <div class="headline">${message.headline}</div>
    <div class="body-text">${message.body}</div>
    <div class="subtle-box">${message.subtle}</div>
    <div class="footer">
      <span class="brand">⚡ SmartGreeting</span>
      <button class="btn-action" onclick="window.close()">Start Coding 🚀</button>
    </div>
    <div class="progress-bar"></div>
  </div>
</body>
</html>`;

  setTimeout(() => {
    try { panel.dispose(); } catch (e) {}
  }, 10000);
}

function showToastNotification(message) {
  vscode.window.showInformationMessage(
    `${message.icon} ${message.headline} — ${message.body}`,
    "Start Coding 🚀",
    "Settings ⚙️"
  ).then(selection => {
    if (selection === "Settings ⚙️") {
      vscode.commands.executeCommand('workbench.action.openSettings', 'smartGreeting');
    }
  });
}

function triggerGreeting(contextState, isManual = false) {
  const config = vscode.workspace.getConfiguration('smartGreeting');

  if (!isManual) {
    if (!config.get('enabled', true)) return;
    if (!config.get('showOnStartup', true)) return;

    const frequency = config.get('frequency', 'oncePerSession');
    if (frequency === 'manualOnly') return;
    if (frequency === 'oncePerSession' && shownInThisSession) return;

    if (frequency === 'oncePerDay') {
      const lastShown = contextState.globalState.get('smartGreeting.lastShownDate');
      const today = new Date().toDateString();
      if (lastShown === today) return;
      contextState.globalState.update('smartGreeting.lastShownDate', today);
    }
  }

  const timeCtx = getTimeContext();
  const lastId = contextState.globalState.get('smartGreeting.lastMessageId');
  const msg = selectMessage(timeCtx, config, lastId);

  shownInThisSession = true;
  contextState.globalState.update('smartGreeting.lastMessageId', msg.id);

  const style = config.get('displayStyle', 'floatingCard');
  if (style === 'floatingCard' || style === 'both') {
    showFloatingCard(msg);
  }
  if (style === 'toastNotification' || style === 'both') {
    showToastNotification(msg);
  }
}

function activate(context) {
  // Status Bar Item
  const statusBar = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Right, 100);
  const timeCtx = getTimeContext();
  const config = vscode.workspace.getConfiguration('smartGreeting');
  const name = config.get('name', 'Avi');
  statusBar.text = `${timeCtx.period === 'Morning' ? '☀️' : timeCtx.period === 'Afternoon' ? '🌤️' : timeCtx.period === 'Evening' ? '🌆' : '🌙'} SmartGreeting`;
  statusBar.tooltip = `SmartGreeting: Hello, ${name}! Click to view greeting.`;
  statusBar.command = 'smartGreeting.showGreeting';
  statusBar.show();
  context.subscriptions.push(statusBar);

  // Commands
  context.subscriptions.push(
    vscode.commands.registerCommand('smartGreeting.showGreeting', () => {
      triggerGreeting(context, true);
    }),
    vscode.commands.registerCommand('smartGreeting.openSettings', () => {
      vscode.commands.executeCommand('workbench.action.openSettings', 'smartGreeting');
    }),
    vscode.commands.registerCommand('smartGreeting.showAbout', () => {
      vscode.window.showInformationMessage(
        "SmartGreeting v1.0.0 — Intelligent, offline greetings for VS Code. Created by Avi.",
        "Configure Settings"
      ).then(sel => {
        if (sel === "Configure Settings") {
          vscode.commands.executeCommand('workbench.action.openSettings', 'smartGreeting');
        }
      });
    })
  );

  // Trigger startup greeting after slight pause
  setTimeout(() => {
    triggerGreeting(context, false);
  }, 1200);
}

function deactivate() {}

module.exports = {
  activate,
  deactivate
};
