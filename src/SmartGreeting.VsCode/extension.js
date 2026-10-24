const vscode = require('vscode');

// --- Greeting Messages (Clean, short, punchy) ---
const GREETINGS = {
  Morning: {
    headline: "Good Morning, {Name}!",
    sub: "Fresh start, fresh mind. Ready to build something remarkable today?",
    icon: "☀️",
    accent: "#f59e0b",
    glow: "rgba(245, 158, 11, 0.2)"
  },
  Afternoon: {
    headline: "Good Afternoon, {Name}!",
    sub: "Midday momentum is in your favor. Stay in the zone and conquer your sprint.",
    icon: "🌤️",
    accent: "#0ea5e9",
    glow: "rgba(14, 165, 233, 0.2)"
  },
  Evening: {
    headline: "Good Evening, {Name}!",
    sub: "Great progress today! Wrap up loose ends, commit clean code, and finish strong.",
    icon: "🌆",
    accent: "#f97316",
    glow: "rgba(249, 115, 22, 0.2)"
  },
  LateNight: {
    headline: "Late Night Coding, {Name}?",
    sub: "Night silence brings deep focus, but remember your rest. Pace yourself.",
    icon: "🌙",
    accent: "#8b5cf6",
    glow: "rgba(139, 92, 246, 0.2)"
  },
  Weekend: {
    headline: "Weekend Crafting, {Name}! 💻",
    sub: "Building passions and side projects. Enjoy the freedom of the craft today!",
    icon: "🌟",
    accent: "#6366f1",
    glow: "rgba(99, 102, 241, 0.2)"
  }
};

// --- Daily Motivational Thoughts (Concise & impactful) ---
const THOUGHTS = [
  {
    quote: "Simplicity is prerequisite for reliability.",
    author: "Edsger W. Dijkstra",
    takeaway: "The cleanest, most maintainable architecture is usually the simplest one."
  },
  {
    quote: "Talk is cheap. Show me the code.",
    author: "Linus Torvalds",
    takeaway: "Test your assumptions against reality. Let working software speak."
  },
  {
    quote: "Any fool can write code that a computer can understand. Good programmers write code that humans can understand.",
    author: "Martin Fowler",
    takeaway: "Code is read ten times more often than it is written. Make your intent obvious."
  },
  {
    quote: "The only way to do great work is to love what you do.",
    author: "Steve Jobs",
    takeaway: "Approach today's code with craftsmanship and attention to detail."
  },
  {
    quote: "The most dangerous phrase in any language is: 'We've always done it this way.'",
    author: "Grace Hopper",
    takeaway: "Dare to rethink legacy patterns with fresh curiosity."
  },
  {
    quote: "Make it work, make it right, make it fast.",
    author: "Kent Beck",
    takeaway: "First verify correct behavior, then polish structure, then tune bottlenecks."
  },
  {
    quote: "First, solve the problem. Then, write the code.",
    author: "John Johnson",
    takeaway: "Spend 5 minutes clarifying the mental model before typing."
  },
  {
    quote: "The best error message is the one that never shows up.",
    author: "Thomas Fuchs",
    takeaway: "Design defensive interfaces and handle edge cases gracefully."
  },
  {
    quote: "Experience is simply the name we give our mistakes.",
    author: "Oscar Wilde",
    takeaway: "Every bug squashed today is another level unlocked in your engineering skills."
  },
  {
    quote: "Perfection is achieved, not when there is nothing more to add, but when there is nothing left to take away.",
    author: "Antoine de Saint-Exupéry",
    takeaway: "Look at your diff: can you delete dead code or redundant layers? Elegance lives in subtraction."
  }
];

// --- Developer Memes & Relatable Humor ---
const JOKES = [
  {
    setup: "Why do programmers prefer dark mode?",
    punchline: "Because light attracts bugs. 🐛",
    tag: "#DevLife"
  },
  {
    setup: "A QA engineer walks into a bar...",
    punchline: "Orders a beer. Orders 0 beers. Orders 999999 beers. Orders a lizard. First real user asks where the bathroom is — bar bursts into flames. 🔥",
    tag: "#QA"
  },
  {
    setup: "There are only 10 types of people in the world:",
    punchline: "Those who understand binary, and those who don't.",
    tag: "#Binary"
  },
  {
    setup: "Developer: 'It works completely fine on my machine!'",
    punchline: "DevOps: 'Great! We will ship your laptop directly to the cloud data center.' 💻📦",
    tag: "#Docker"
  },
  {
    setup: "The Two Hard Problems in Computer Science:",
    punchline: "1. Cache invalidation\n2. Naming things\n3. Off-by-one errors",
    tag: "#Classic"
  },
  {
    setup: "Debugging emotional stages:",
    punchline: "1. That can't happen.\n2. That doesn't happen on my machine.\n3. Why does that happen?\n4. How did that EVER work in the first place?! 🤯",
    tag: "#Debugging"
  },
  {
    setup: "CSS in a nutshell:",
    punchline: "display: flex; justify-content: center; align-items: center;\n/* Now the div is centered on my neighbor's smart fridge */",
    tag: "#CSS"
  },
  {
    setup: "The Golden Rule of DevOps:",
    punchline: "Never deploy to production at 4:58 PM on a Friday. Unless you enjoy panic weekend Slack calls. 😉",
    tag: "#DevOps"
  }
];

let globalExtensionContext = null;
let activeModalPanel = null;
let statusBarItem = null;
let shownInThisSession = false;

let lastWaterReminderTime = Date.now();
let lastCoffeeReminderTime = Date.now();
let lastPostureReminderTime = Date.now();
let lastLunchNotifiedDate = "";

function getTimeContext() {
  const now = new Date();
  const totalMinutes = now.getHours() * 60 + now.getMinutes();
  const day = now.getDay();

  let period = "LateNight";
  if (totalMinutes >= 5 * 60 && totalMinutes < 12 * 60) period = "Morning";
  else if (totalMinutes >= 12 * 60 && totalMinutes < 17 * 60) period = "Afternoon";
  else if (totalMinutes >= 17 * 60 && totalMinutes < 21 * 60) period = "Evening";

  const isWeekend = day === 0 || day === 6;
  const greetingKey = isWeekend ? "Weekend" : period;

  const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
  const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

  return {
    now,
    period,
    greetingKey,
    dateText: `${days[day]}, ${months[now.getMonth()]} ${now.getDate()}`,
    timeText: now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
    hour: now.getHours(),
    minutes: now.getMinutes()
  };
}

function getWaterCount() {
  if (!globalExtensionContext) return 0;
  const today = new Date().toDateString();
  const storedDate = globalExtensionContext.globalState.get('devCompanion.waterDate');
  if (storedDate !== today) {
    globalExtensionContext.globalState.update('devCompanion.waterDate', today);
    globalExtensionContext.globalState.update('devCompanion.waterCount', 0);
    return 0;
  }
  return globalExtensionContext.globalState.get('devCompanion.waterCount', 0);
}

function incrementWaterCount() {
  if (!globalExtensionContext) return 1;
  const today = new Date().toDateString();
  globalExtensionContext.globalState.update('devCompanion.waterDate', today);
  const updated = getWaterCount() + 1;
  globalExtensionContext.globalState.update('devCompanion.waterCount', updated);
  lastWaterReminderTime = Date.now();
  updateStatusBar();
  return updated;
}

function updateStatusBar() {
  if (!statusBarItem) return;
  const timeCtx = getTimeContext();
  const config = vscode.workspace.getConfiguration('smartGreeting');
  const name = config.get('name', 'Developer') || 'Developer';
  const waterCount = getWaterCount();
  const dailyGoal = config.get('waterDailyGoal', 8);

  const coffeeInterval = config.get('coffeeIntervalMinutes', 90);
  const elapsedCoffeeMins = Math.floor((Date.now() - lastCoffeeReminderTime) / 60000);
  const nextCoffeeMins = Math.max(1, coffeeInterval - elapsedCoffeeMins);

  const icon = timeCtx.period === 'Morning' ? '☀️' : timeCtx.period === 'Afternoon' ? '🌤️' : timeCtx.period === 'Evening' ? '🌆' : '🌙';

  statusBarItem.text = `${icon} DevCompanion: 💧 ${waterCount}/${dailyGoal} | ☕ ${nextCoffeeMins}m`;
  statusBarItem.tooltip = new vscode.MarkdownString(
    `### DevCompanion\n` +
    `Hello, **${name}**!\n\n` +
    `• **💧 Water Today**: ${waterCount} / ${dailyGoal} glasses\n` +
    `• **☕ Next Coffee Break**: in ~${nextCoffeeMins} mins\n` +
    `• **🍱 Lunch Scheduled**: ${config.get('lunchTime', '13:00')}\n\n` +
    `*Click to open DevCompanion.*`
  );
}

// --- Prompt User for Name (First-Time Install & Update) ---
async function promptForUserName(context, isFirstTime = false) {
  const config = vscode.workspace.getConfiguration('smartGreeting');
  const currentName = config.get('name', '');

  const enteredName = await vscode.window.showInputBox({
    title: isFirstTime ? "👋 Welcome to DevCompanion!" : "✏️ Update Developer Name",
    prompt: isFirstTime 
      ? "What is your name? (Used for your personalized greetings and health alerts)"
      : "Enter your name to personalize your greetings and alerts:",
    placeHolder: "e.g. Avinash, Avi, Alex, Sarah...",
    value: isFirstTime ? "" : currentName,
    ignoreFocusOut: true
  });

  if (isFirstTime) {
    await context.globalState.update('devCompanion.hasPromptedName', true);
  }

  if (enteredName !== undefined) {
    const finalName = enteredName.trim() || (isFirstTime ? "Developer" : currentName || "Developer");
    await config.update('name', finalName, vscode.ConfigurationTarget.Global);
    vscode.window.showInformationMessage(
      isFirstTime 
        ? `🎉 Welcome aboard, ${finalName}! DevCompanion is all set.`
        : `✅ Developer name updated to "${finalName}".`
    );
    updateStatusBar();
    if (isFirstTime) {
      triggerGreeting(context, false);
    } else {
      showModalCard('greeting');
    }
    return finalName;
  } else if (isFirstTime) {
    triggerGreeting(context, false);
  }
  return null;
}

// --- Universal Context-Aware Modal Window ---
function showModalCard(mode = 'greeting', customData = {}) {
  const config = vscode.workspace.getConfiguration('smartGreeting');
  const autoCloseSeconds = config.get('autoCloseSeconds', 40);
  const rawName = config.get('name', '');
  const name = (!rawName || !rawName.trim()) ? "Developer" : rawName.trim();
  const timeCtx = getTimeContext();

  const waterCount = getWaterCount();
  const waterGoal = config.get('waterDailyGoal', 8);
  const lunchTime = config.get('lunchTime', '13:00');

  const greetingConfig = GREETINGS[timeCtx.greetingKey] || GREETINGS.Morning;
  const randomThought = THOUGHTS[Math.floor(Math.random() * THOUGHTS.length)];
  const randomJoke = JOKES[Math.floor(Math.random() * JOKES.length)];

  if (activeModalPanel) {
    try { activeModalPanel.dispose(); } catch (e) {}
  }

  const titles = {
    greeting: `DevCompanion — Welcome, ${name}!`,
    water: `💧 Time to Hydrate!`,
    coffee: `☕ Coffee & Tea Pause`,
    lunch: `🍱 Lunch Break Reminder`,
    posture: `🧘 Posture & Eye Stretch`
  };

  const panel = vscode.window.createWebviewPanel(
    'devCompanionModal',
    titles[mode] || 'DevCompanion',
    { viewColumn: vscode.ViewColumn.Active, preserveFocus: false },
    { enableScripts: true, retainContextWhenHidden: true }
  );

  activeModalPanel = panel;
  panel.onDidDispose(() => {
    if (activeModalPanel === panel) activeModalPanel = null;
  });

  panel.webview.onDidReceiveMessage(msg => {
    if (msg.command === 'close') {
      try { panel.dispose(); } catch (e) {}
    } else if (msg.command === 'logWater') {
      const updated = incrementWaterCount();
      panel.webview.postMessage({ type: 'waterUpdated', count: updated, goal: waterGoal });
      vscode.window.showInformationMessage(`💧 Hydration logged: ${updated}/${waterGoal} glasses today!`);
    } else if (msg.command === 'takeBreak') {
      lastCoffeeReminderTime = Date.now();
      updateStatusBar();
      try { panel.dispose(); } catch (e) {}
      vscode.window.showInformationMessage(`☕ Enjoy your break! Step away and refresh.`);
    } else if (msg.command === 'snoozeWater') {
      lastWaterReminderTime = Date.now() - (config.get('waterIntervalMinutes', 45) - 10) * 60 * 1000;
      try { panel.dispose(); } catch (e) {}
    } else if (msg.command === 'snoozeCoffee') {
      lastCoffeeReminderTime = Date.now() - (config.get('coffeeIntervalMinutes', 90) - 15) * 60 * 1000;
      try { panel.dispose(); } catch (e) {}
    } else if (msg.command === 'snoozeLunch') {
      lastLunchNotifiedDate = "";
      try { panel.dispose(); } catch (e) {}
    } else if (msg.command === 'snoozePosture') {
      lastPostureReminderTime = Date.now() - (config.get('postureIntervalMinutes', 30) - 10) * 60 * 1000;
      try { panel.dispose(); } catch (e) {}
    } else if (msg.command === 'switchMode') {
      showModalCard(msg.mode);
    } else if (msg.command === 'updateName') {
      promptForUserName(globalExtensionContext, false);
    } else if (msg.command === 'openSettings') {
      vscode.commands.executeCommand('workbench.action.openSettings', 'smartGreeting');
    }
  });

  panel.webview.html = getRevitalizedHtml({
    mode,
    name,
    timeCtx,
    greetingConfig,
    thought: randomThought,
    joke: randomJoke,
    waterCount,
    waterGoal,
    lunchTime,
    autoCloseSeconds
  });
}

function getRevitalizedHtml(data) {
  const { mode, name, timeCtx, greetingConfig, thought, joke, waterCount, waterGoal, lunchTime, autoCloseSeconds } = data;

  const modeAccents = {
    greeting: greetingConfig.accent,
    water: '#06b6d4',
    coffee: '#f59e0b',
    lunch: '#10b981',
    posture: '#8b5cf6'
  };
  const accent = modeAccents[mode] || greetingConfig.accent;

  return `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>DevCompanion</title>
  <style>
    * { box-sizing: border-box; margin: 0; padding: 0; }
    :root {
      --accent: ${accent};
      --bg: #0c1017;
      --card-bg: rgba(18, 24, 38, 0.92);
      --border: rgba(255, 255, 255, 0.12);
      --text-main: #ffffff;
      --text-muted: #94a3b8;
    }
    body {
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI Variable Text", "Segoe UI", Roboto, sans-serif;
      background: var(--bg);
      color: var(--text-main);
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 24px;
      overflow-x: hidden;
      overflow-y: auto;
    }

    body::before {
      content: '';
      position: fixed;
      top: -15%; left: 10%; width: 80%; height: 80%;
      background: radial-gradient(circle, var(--accent) 0%, transparent 65%);
      opacity: 0.12;
      filter: blur(80px);
      pointer-events: none;
      z-index: 0;
    }

    .modal-container {
      position: relative;
      z-index: 1;
      width: min(880px, 78vw);
      min-height: 52vh;
      background: var(--card-bg);
      backdrop-filter: blur(28px);
      -webkit-backdrop-filter: blur(28px);
      border: 1.5px solid var(--border);
      border-radius: 24px;
      padding: 38px 44px;
      box-shadow: 0 32px 80px rgba(0, 0, 0, 0.75), 0 0 40px rgba(0, 0, 0, 0.5);
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      gap: 28px;
      animation: modalSlide 0.35s cubic-bezier(0.16, 1, 0.3, 1);
    }

    @keyframes modalSlide {
      from { transform: translateY(20px) scale(0.98); opacity: 0; }
      to { transform: translateY(0) scale(1); opacity: 1; }
    }

    .top-bar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      flex-wrap: wrap;
      gap: 12px;
      border-bottom: 1px solid rgba(255, 255, 255, 0.08);
      padding-bottom: 16px;
    }
    .top-left {
      display: flex;
      align-items: center;
      gap: 10px;
    }
    .badge {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 6px 14px;
      border-radius: 20px;
      font-size: 13px;
      font-weight: 700;
      letter-spacing: 0.4px;
      background: rgba(255, 255, 255, 0.07);
      border: 1px solid rgba(255, 255, 255, 0.12);
      color: #fff;
    }
    .badge.accent-badge {
      background: var(--accent);
      color: #0b0f17;
      border-color: var(--accent);
    }
    .top-right {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    .timer-badge {
      font-size: 13px;
      padding: 6px 12px;
      border-radius: 10px;
      background: rgba(255, 255, 255, 0.06);
      color: var(--text-muted);
      display: flex;
      align-items: center;
      gap: 6px;
      font-variant-numeric: tabular-nums;
    }
    .btn-close {
      width: 34px;
      height: 34px;
      border-radius: 50%;
      background: rgba(255, 255, 255, 0.08);
      border: 1px solid rgba(255, 255, 255, 0.14);
      color: #fff;
      font-size: 15px;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.2s ease;
    }
    .btn-close:hover {
      background: rgba(255, 255, 255, 0.18);
      transform: scale(1.05);
    }

    .content-area {
      display: flex;
      flex-direction: column;
      gap: 22px;
    }

    .hero-title {
      font-size: clamp(28px, 3.4vw, 36px);
      font-weight: 800;
      line-height: 1.25;
      color: #ffffff;
      letter-spacing: -0.5px;
    }
    .hero-title span.accent-name {
      color: var(--accent);
    }
    .btn-edit-name {
      background: rgba(255, 255, 255, 0.08);
      border: 1px solid rgba(255, 255, 255, 0.15);
      color: #fff;
      border-radius: 8px;
      font-size: 14px;
      padding: 4px 10px;
      cursor: pointer;
      vertical-align: middle;
      margin-left: 8px;
      transition: all 0.2s ease;
    }
    .btn-edit-name:hover {
      background: rgba(255, 255, 255, 0.22);
      border-color: var(--accent);
      transform: scale(1.08);
    }

    .hero-sub {
      font-size: 17.5px;
      line-height: 1.6;
      color: var(--text-muted);
      max-width: 95%;
    }

    .spotlight-box {
      background: rgba(255, 255, 255, 0.035);
      border: 1px solid rgba(255, 255, 255, 0.1);
      border-radius: 18px;
      padding: 24px 28px;
      display: flex;
      flex-direction: column;
      gap: 16px;
      position: relative;
    }

    .tab-pills {
      display: inline-flex;
      background: rgba(0, 0, 0, 0.35);
      border: 1px solid rgba(255, 255, 255, 0.1);
      border-radius: 12px;
      padding: 4px;
      width: fit-content;
      gap: 4px;
    }
    .tab-btn {
      background: transparent;
      border: none;
      color: var(--text-muted);
      padding: 7px 16px;
      border-radius: 8px;
      font-size: 13.5px;
      font-weight: 700;
      cursor: pointer;
      transition: all 0.2s ease;
    }
    .tab-btn.active {
      background: rgba(255, 255, 255, 0.12);
      color: #fff;
    }

    .spotlight-text {
      font-size: 21px;
      line-height: 1.55;
      color: #f8fafc;
      font-style: italic;
    }
    .spotlight-author {
      font-size: 15px;
      font-weight: 700;
      color: var(--accent);
      margin-top: 4px;
    }
    .spotlight-takeaway {
      font-size: 14.5px;
      color: #cbd5e1;
      padding-top: 8px;
      border-top: 1px dashed rgba(255, 255, 255, 0.1);
      line-height: 1.5;
    }

    .hydration-tracker {
      display: flex;
      flex-direction: column;
      gap: 16px;
      background: rgba(6, 182, 212, 0.08);
      border: 1.5px solid rgba(6, 182, 212, 0.25);
      border-radius: 18px;
      padding: 26px 30px;
    }
    .glasses-row {
      display: flex;
      gap: 12px;
      font-size: 28px;
      flex-wrap: wrap;
    }
    .hydration-stat {
      font-size: 22px;
      font-weight: 800;
      color: #22d3ee;
    }

    .stretch-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: 16px;
    }
    .stretch-card {
      background: rgba(255, 255, 255, 0.04);
      border: 1px solid rgba(255, 255, 255, 0.1);
      border-radius: 14px;
      padding: 18px;
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
    .stretch-card-title {
      font-size: 15px;
      font-weight: 750;
      color: #a78bfa;
    }
    .stretch-card-desc {
      font-size: 13.5px;
      line-height: 1.5;
      color: #cbd5e1;
    }

    .bottom-bar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      flex-wrap: wrap;
      gap: 16px;
      border-top: 1px solid rgba(255, 255, 255, 0.08);
      padding-top: 18px;
    }
    .bottom-left-info {
      font-size: 14px;
      color: var(--text-muted);
      display: flex;
      align-items: center;
      gap: 8px;
    }
    .bottom-actions {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .btn-primary {
      background: var(--accent);
      color: #0b0f17;
      border: none;
      padding: 13px 30px;
      border-radius: 12px;
      font-size: 15px;
      font-weight: 800;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 8px;
      transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
      box-shadow: 0 4px 18px rgba(0, 0, 0, 0.3);
    }
    .btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 24px rgba(0, 0, 0, 0.45);
      opacity: 0.94;
    }
    .btn-secondary {
      background: rgba(255, 255, 255, 0.07);
      border: 1px solid rgba(255, 255, 255, 0.14);
      color: #fff;
      padding: 12px 22px;
      border-radius: 12px;
      font-size: 14px;
      font-weight: 650;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 6px;
      transition: all 0.2s ease;
    }
    .btn-secondary:hover {
      background: rgba(255, 255, 255, 0.14);
      border-color: rgba(255, 255, 255, 0.25);
    }

    .progress-bar {
      position: absolute;
      bottom: 0;
      left: 0;
      height: 3px;
      background: var(--accent);
      width: 100%;
      border-radius: 0 0 24px 24px;
      transition: width 1s linear;
    }
  </style>
</head>
<body>

  <div class="modal-container" id="modalCard">
    <!-- Top Bar -->
    <div class="top-bar">
      <div class="top-left">
        <span class="badge accent-badge">${mode === 'water' ? '💧 Hydration' : mode === 'coffee' ? '☕ Break' : mode === 'lunch' ? '🍱 Lunch' : mode === 'posture' ? '🧘 Wellness' : greetingConfig.icon + ' DevCompanion'}</span>
        <span class="badge">📅 ${timeCtx.dateText}</span>
        <span class="badge">⏰ ${timeCtx.timeText}</span>
      </div>

      <div class="top-right">
        <div class="timer-badge" id="timerBadge">
          <span>⏱️</span>
          <span id="timerText">${autoCloseSeconds > 0 ? `${autoCloseSeconds}s` : 'Pinned'}</span>
        </div>
        <button class="btn-close" id="closeBtn" title="Dismiss">✕</button>
      </div>
    </div>

    <!-- Mode-Specific Relevant Content -->
    ${mode === 'greeting' ? `
    <!-- GREETING / STARTUP VIEW -->
    <div class="content-area">
      <div>
        <h1 class="hero-title">${greetingConfig.headline.replace("{Name}", `<span class="accent-name">${name}</span>`)} <button class="btn-edit-name" id="editNameBtn" title="Change your name">✏️ Change</button></h1>
        <p class="hero-sub" style="margin-top: 8px;">${greetingConfig.sub}</p>
      </div>

      <!-- Spotlight: Choose Motivation, Humor, OR Creator without clutter -->
      <div class="spotlight-box">
        <div class="tab-pills">
          <button class="tab-btn active" id="tabThought">🌟 Daily Thought</button>
          <button class="tab-btn" id="tabJoke">😂 Dev Humor</button>
          <button class="tab-btn" id="tabCreator">👨‍💻 Creator</button>
        </div>

        <div id="thoughtView">
          <div class="spotlight-text">“${thought.quote}”</div>
          <div class="spotlight-author">— ${thought.author}</div>
          <div class="spotlight-takeaway">💡 <strong>Insight:</strong> ${thought.takeaway}</div>
        </div>

        <div id="jokeView" style="display: none;">
          <div style="font-size: 19px; font-weight: 700; color: #fff; margin-bottom: 8px;">${joke.setup}</div>
          <div style="font-size: 17.5px; color: #fbbf24; line-height: 1.5; white-space: pre-line;">${joke.punchline}</div>
          <div style="font-size: 12px; color: var(--text-muted); margin-top: 8px;">${joke.tag}</div>
        </div>

        <div id="creatorView" style="display: none;">
          <div style="font-size: 20px; font-weight: 800; color: #fff; margin-bottom: 6px;">Avinash Shinde</div>
          <div style="font-size: 14.5px; color: var(--text-muted); margin-bottom: 14px;">Creator of DevCompanion • Dedicated to developer wellness &amp; smart tooling</div>
          <div style="display: flex; flex-direction: column; gap: 10px;">
            <a href="https://www.linkedin.com/in/avinash-shinde-7628b91a5/" target="_blank" style="display: inline-flex; align-items: center; gap: 8px; font-size: 15px; color: #60a5fa; text-decoration: none; font-weight: 600;">
              🔗 <span>LinkedIn:</span> <span style="text-decoration: underline;">https://www.linkedin.com/in/avinash-shinde-7628b91a5/</span>
            </a>
            <a href="https://github.com/Shinde-Avinash" target="_blank" style="display: inline-flex; align-items: center; gap: 8px; font-size: 15px; color: #34d399; text-decoration: none; font-weight: 600;">
              🌐 <span>GitHub:</span> <span style="text-decoration: underline;">https://github.com/Shinde-Avinash</span>
            </a>
          </div>
        </div>
      </div>
    </div>

    <div class="bottom-bar">
      <div class="bottom-left-info">
        <span>💧 Water: <strong>${waterCount}/${waterGoal}</strong> glasses</span>
        <span>•</span>
        <span>🍱 Lunch: <strong>${lunchTime}</strong></span>
      </div>
      <div class="bottom-actions">
        <button class="btn-secondary" id="changeNameBtn">✏️ Name</button>
        <button class="btn-secondary" id="settingsBtn">⚙️ Settings</button>
        <button class="btn-primary" id="startCodingBtn">Start Coding 🚀</button>
      </div>
    </div>
    ` : mode === 'water' ? `
    <!-- DEDICATED HYDRATION REMINDER VIEW -->
    <div class="content-area">
      <div>
        <h1 class="hero-title">Time to Hydrate, <span class="accent-name">${name}</span>! 💧</h1>
        <p class="hero-sub" style="margin-top: 8px;">You've been in deep coding focus. Drink a fresh glass of water to keep your mental acuity, focus, and energy at their peak.</p>
      </div>

      <div class="hydration-tracker">
        <div class="hydration-stat" id="waterStatText">${waterCount} of ${waterGoal} Glasses Logged (${waterCount * 250} ml)</div>
        <div class="glasses-row" id="glassesRow">
          ${Array.from({ length: waterGoal }, (_, i) => i < waterCount ? '🥛' : '⚪').join(' ')}
        </div>
        <div style="font-size: 14px; color: #94a3b8;">Daily target: ${waterGoal * 250} ml (8 glasses). Staying hydrated prevents afternoon brain fog!</div>
      </div>
    </div>

    <div class="bottom-bar">
      <div class="bottom-left-info">
        <span>💧 Keep up the great routine!</span>
      </div>
      <div class="bottom-actions">
        <button class="btn-secondary" id="snoozeWaterBtn">Snooze 10m ⏳</button>
        <button class="btn-primary" id="drinkWaterBtn" style="background: #06b6d4; color: #0b0f17;">+ Drink a Glass 💧</button>
        <button class="btn-secondary" id="startCodingBtn">Done Coding 🚀</button>
      </div>
    </div>
    ` : mode === 'coffee' ? `
    <!-- DEDICATED COFFEE / TEA BREAK VIEW -->
    <div class="content-area">
      <div>
        <h1 class="hero-title">Coffee & Tea Pause, <span class="accent-name">${name}</span>! ☕</h1>
        <p class="hero-sub" style="margin-top: 8px;">You've completed 90 minutes of intensive coding. Step away from the monitor, stretch your legs, and grab a warm drink to reset your thinking.</p>
      </div>

      <div class="spotlight-box" style="border-left: 4px solid #f59e0b;">
        <div style="font-size: 18px; font-weight: 700; color: #fff;">💡 Why step away?</div>
        <div style="font-size: 15.5px; color: #cbd5e1; line-height: 1.55;">
          The brain's default mode network consolidates architectural decisions while you rest. Solutions to tough bugs almost always appear during a quick walk or tea break.
        </div>
      </div>
    </div>

    <div class="bottom-bar">
      <div class="bottom-left-info">
        <span>☕ Recharging will make your next sprint 2x faster.</span>
      </div>
      <div class="bottom-actions">
        <button class="btn-secondary" id="snoozeCoffeeBtn">Snooze 15m ⏳</button>
        <button class="btn-primary" id="takeBreakBtn" style="background: #f59e0b; color: #0b0f17;">Taking Break ☕</button>
        <button class="btn-secondary" id="startCodingBtn">Back to Code 🚀</button>
      </div>
    </div>
    ` : mode === 'lunch' ? `
    <!-- DEDICATED LUNCH BREAK VIEW -->
    <div class="content-area">
      <div>
        <h1 class="hero-title">Lunch Break Time, <span class="accent-name">${name}</span>! 🍱</h1>
        <p class="hero-sub" style="margin-top: 8px;">Fuel your body and rest your eyes. Never eat at your keyboard — taking a real meal break restores cognitive energy for the entire afternoon.</p>
      </div>

      <div class="spotlight-box" style="border-left: 4px solid #10b981;">
        <div style="font-size: 18px; font-weight: 700; color: #fff;">🥗 Healthy Habits for Developers:</div>
        <div style="font-size: 15.5px; color: #cbd5e1; line-height: 1.55;">
          • Step away from all digital screens for at least 20 minutes.<br>
          • Pair proteins and complex carbs for stable blood sugar.<br>
          • Return refreshed with 100% clarity.
        </div>
      </div>
    </div>

    <div class="bottom-bar">
      <div class="bottom-left-info">
        <span>🍱 Enjoy your meal!</span>
      </div>
      <div class="bottom-actions">
        <button class="btn-secondary" id="snoozeLunchBtn">Remind in 15m ⏳</button>
        <button class="btn-primary" id="startCodingBtn" style="background: #10b981; color: #0b0f17;">Enjoying Lunch 🍱</button>
      </div>
    </div>
    ` : `
    <!-- DEDICATED POSTURE / STRETCH VIEW -->
    <div class="content-area">
      <div>
        <h1 class="hero-title">Ergonomic Posture & Eye Check 🧘</h1>
        <p class="hero-sub" style="margin-top: 8px;">Take 30 seconds right now to reset your spine, relax your shoulders, and relieve ocular strain.</p>
      </div>

      <div class="stretch-grid">
        <div class="stretch-card">
          <div class="stretch-card-title">👀 20-20-20 Eye Rest</div>
          <div class="stretch-card-desc">Look at an object 20 feet away for 20 seconds. Relaxes your optical focusing muscles immediately.</div>
        </div>
        <div class="stretch-card">
          <div class="stretch-card-title">🧘 Shoulder Blade Pinch</div>
          <div class="stretch-card-desc">Sit tall, unround your shoulders, and squeeze your shoulder blades together for 10 seconds.</div>
        </div>
        <div class="stretch-card">
          <div class="stretch-card-title">🔄 Gentle Neck Roll</div>
          <div class="stretch-card-desc">Perform 5 slow, smooth circular rolls in each direction to release cervical tension.</div>
        </div>
      </div>
    </div>

    <div class="bottom-bar">
      <div class="bottom-left-info">
        <span>🧘 A healthy spine builds long-term engineering stamina.</span>
      </div>
      <div class="bottom-actions">
        <button class="btn-secondary" id="snoozePostureBtn">Snooze 10m ⏳</button>
        <button class="btn-primary" id="startCodingBtn" style="background: #8b5cf6; color: #0b0f17;">Stretched & Ready! ✨</button>
      </div>
    </div>
    `}

    <!-- Countdown Progress Bar -->
    ${autoCloseSeconds > 0 ? `<div class="progress-bar" id="progressBar"></div>` : ''}
  </div>

  <script>
    const vscode = acquireVsCodeApi();

    // Tab switcher for Greeting mode
    const tabThought = document.getElementById('tabThought');
    const tabJoke = document.getElementById('tabJoke');
    const tabCreator = document.getElementById('tabCreator');
    const thoughtView = document.getElementById('thoughtView');
    const jokeView = document.getElementById('jokeView');
    const creatorView = document.getElementById('creatorView');

    function activateTab(tabToActive, viewToShow) {
      [tabThought, tabJoke, tabCreator].forEach(t => t && t.classList.remove('active'));
      [thoughtView, jokeView, creatorView].forEach(v => v && (v.style.display = 'none'));
      if (tabToActive) tabToActive.classList.add('active');
      if (viewToShow) viewToShow.style.display = 'block';
    }

    if (tabThought && thoughtView) {
      tabThought.addEventListener('click', () => activateTab(tabThought, thoughtView));
    }
    if (tabJoke && jokeView) {
      tabJoke.addEventListener('click', () => activateTab(tabJoke, jokeView));
    }
    if (tabCreator && creatorView) {
      tabCreator.addEventListener('click', () => activateTab(tabCreator, creatorView));
    }

    // Button event dispatchers
    const startCodingBtn = document.getElementById('startCodingBtn');
    if (startCodingBtn) startCodingBtn.addEventListener('click', () => vscode.postMessage({ command: 'close' }));

    const closeBtn = document.getElementById('closeBtn');
    if (closeBtn) closeBtn.addEventListener('click', () => vscode.postMessage({ command: 'close' }));

    const settingsBtn = document.getElementById('settingsBtn');
    if (settingsBtn) settingsBtn.addEventListener('click', () => vscode.postMessage({ command: 'openSettings' }));

    const editNameBtn = document.getElementById('editNameBtn');
    if (editNameBtn) editNameBtn.addEventListener('click', () => vscode.postMessage({ command: 'updateName' }));

    const changeNameBtn = document.getElementById('changeNameBtn');
    if (changeNameBtn) changeNameBtn.addEventListener('click', () => vscode.postMessage({ command: 'updateName' }));

    const drinkWaterBtn = document.getElementById('drinkWaterBtn');
    if (drinkWaterBtn) drinkWaterBtn.addEventListener('click', () => vscode.postMessage({ command: 'logWater' }));

    const snoozeWaterBtn = document.getElementById('snoozeWaterBtn');
    if (snoozeWaterBtn) snoozeWaterBtn.addEventListener('click', () => vscode.postMessage({ command: 'snoozeWater' }));

    const takeBreakBtn = document.getElementById('takeBreakBtn');
    if (takeBreakBtn) takeBreakBtn.addEventListener('click', () => vscode.postMessage({ command: 'takeBreak' }));

    const snoozeCoffeeBtn = document.getElementById('snoozeCoffeeBtn');
    if (snoozeCoffeeBtn) snoozeCoffeeBtn.addEventListener('click', () => vscode.postMessage({ command: 'snoozeCoffee' }));

    const snoozeLunchBtn = document.getElementById('snoozeLunchBtn');
    if (snoozeLunchBtn) snoozeLunchBtn.addEventListener('click', () => vscode.postMessage({ command: 'snoozeLunch' }));

    const snoozePostureBtn = document.getElementById('snoozePostureBtn');
    if (snoozePostureBtn) snoozePostureBtn.addEventListener('click', () => vscode.postMessage({ command: 'snoozePosture' }));

    // Listen for water update
    window.addEventListener('message', event => {
      const msg = event.data;
      if (msg.type === 'waterUpdated') {
        const stat = document.getElementById('waterStatText');
        const row = document.getElementById('glassesRow');
        if (stat) stat.textContent = msg.count + ' of ' + msg.goal + ' Glasses Logged (' + (msg.count * 250) + ' ml)';
        if (row) {
          let icons = '';
          for (let i = 0; i < msg.goal; i++) icons += (i < msg.count ? '🥛 ' : '⚪ ');
          row.textContent = icons;
        }
      }
    });

    // Auto-Close Timer with Hover Pause
    const totalSecs = ${autoCloseSeconds};
    let remaining = totalSecs;
    let isHovered = false;

    if (totalSecs > 0) {
      const timerText = document.getElementById('timerText');
      const progressBar = document.getElementById('progressBar');
      const card = document.getElementById('modalCard');

      card.addEventListener('mouseenter', () => { isHovered = true; });
      card.addEventListener('mouseleave', () => { isHovered = false; });

      const interval = setInterval(() => {
        if (isHovered) {
          if (timerText) timerText.textContent = 'Paused';
          return;
        }
        remaining--;
        if (timerText) timerText.textContent = remaining + 's';
        if (progressBar) progressBar.style.width = Math.max(0, (remaining / totalSecs) * 100) + '%';

        if (remaining <= 0) {
          clearInterval(interval);
          vscode.postMessage({ command: 'close' });
        }
      }, 1000);
    }
  </script>
</body>
</html>`;
}

// --- Background Wellness Reminders Engine ---
function checkWellnessReminders() {
  const config = vscode.workspace.getConfiguration('smartGreeting');
  if (!config.get('enabled', true)) return;

  const now = Date.now();
  const nowDate = new Date();
  const todayStr = nowDate.toDateString();
  const displayStyle = config.get('displayStyle', 'floatingCard');

  // 1. Water / Hydration Reminder
  if (config.get('waterReminder', true)) {
    const waterIntervalMs = config.get('waterIntervalMinutes', 45) * 60 * 1000;
    if (now - lastWaterReminderTime >= waterIntervalMs) {
      lastWaterReminderTime = now;
      if (displayStyle === 'floatingCard' || displayStyle === 'both') {
        showModalCard('water');
      } else {
        const count = getWaterCount();
        const goal = config.get('waterDailyGoal', 8);
        vscode.window.showInformationMessage(
          `💧 Hydration Break: Time to drink water! (${count}/${goal} glasses today)`,
          "Drank Water 💧", "Snooze 10m"
        ).then(sel => {
          if (sel === "Drank Water 💧") incrementWaterCount();
        });
      }
    }
  }

  // 2. Coffee & Tea Break
  if (config.get('coffeeReminder', true)) {
    const coffeeIntervalMs = config.get('coffeeIntervalMinutes', 90) * 60 * 1000;
    if (now - lastCoffeeReminderTime >= coffeeIntervalMs) {
      lastCoffeeReminderTime = now;
      if (displayStyle === 'floatingCard' || displayStyle === 'both') {
        showModalCard('coffee');
      } else {
        vscode.window.showInformationMessage(
          "☕ Coffee/Tea Break: 90 mins coded. Step away from your desk and recharge!",
          "Taking Break ☕", "Snooze 15m"
        );
      }
    }
  }

  // 3. Lunch Reminder
  if (config.get('lunchReminder', true)) {
    const lunchSetting = config.get('lunchTime', '13:00');
    const [lHour, lMin] = lunchSetting.split(':').map(Number);
    const curHour = nowDate.getHours();
    const curMin = nowDate.getMinutes();

    if (curHour === lHour && curMin >= lMin && curMin <= lMin + 10 && lastLunchNotifiedDate !== todayStr) {
      lastLunchNotifiedDate = todayStr;
      if (displayStyle === 'floatingCard' || displayStyle === 'both') {
        showModalCard('lunch');
      } else {
        vscode.window.showInformationMessage(
          "🍱 Lunch Break! Time to step away from your keyboard and eat.",
          "Going to Lunch 🍱", "Remind in 15m"
        );
      }
    }
  }

  // 4. Posture Check
  if (config.get('postureReminder', true)) {
    const postureIntervalMs = config.get('postureIntervalMinutes', 30) * 60 * 1000;
    if (now - lastPostureReminderTime >= postureIntervalMs) {
      lastPostureReminderTime = now;
      if (displayStyle === 'floatingCard' || displayStyle === 'both') {
        showModalCard('posture');
      } else {
        vscode.window.showInformationMessage(
          "🧘 Posture Check: Sit tall, unround shoulders, and rest your eyes with 20-20-20!",
          "Stretched! ✨", "Snooze 10m"
        );
      }
    }
  }

  updateStatusBar();
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

  shownInThisSession = true;
  showModalCard('greeting');
}

function activate(context) {
  globalExtensionContext = context;

  // Status Bar Item
  statusBarItem = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Right, 100);
  statusBarItem.command = 'smartGreeting.showGreeting';
  updateStatusBar();
  statusBarItem.show();
  context.subscriptions.push(statusBarItem);

  // Periodic Reminder Timer (runs every 60 seconds)
  const timer = setInterval(() => {
    checkWellnessReminders();
  }, 60000);

  context.subscriptions.push({
    dispose: () => clearInterval(timer)
  });

  // Commands
  context.subscriptions.push(
    vscode.commands.registerCommand('smartGreeting.showGreeting', () => {
      triggerGreeting(context, true);
    }),
    vscode.commands.registerCommand('smartGreeting.updateName', () => {
      promptForUserName(context, false);
    }),
    vscode.commands.registerCommand('smartGreeting.logWater', () => {
      showModalCard('water');
    }),
    vscode.commands.registerCommand('smartGreeting.takeBreak', () => {
      showModalCard('coffee');
    }),
    vscode.commands.registerCommand('smartGreeting.showStretches', () => {
      showModalCard('posture');
    }),
    vscode.commands.registerCommand('smartGreeting.openSettings', () => {
      vscode.commands.executeCommand('workbench.action.openSettings', 'smartGreeting');
    }),
    vscode.commands.registerCommand('smartGreeting.showAbout', () => {
      vscode.window.showInformationMessage(
        "DevCompanion v1.3.0 — Clean, relevant smart greetings & wellness reminders for VS Code. Created by Avinash Shinde.",
        "Change Name ✏️", "Configure Settings"
      ).then(sel => {
        if (sel === "Change Name ✏️") {
          promptForUserName(context, false);
        } else if (sel === "Configure Settings") {
          vscode.commands.executeCommand('workbench.action.openSettings', 'smartGreeting');
        }
      });
    })
  );

  // First-Time Install Onboarding: Prompt for name if not set yet!
  const hasPrompted = context.globalState.get('devCompanion.hasPromptedName', false);
  const config = vscode.workspace.getConfiguration('smartGreeting');
  const currentName = config.get('name', '');

  if (!hasPrompted && (!currentName || currentName === 'Avi')) {
    setTimeout(async () => {
      await promptForUserName(context, true);
    }, 1000);
  } else {
    // Normal startup trigger
    setTimeout(() => {
      triggerGreeting(context, false);
    }, 1200);
  }
}

function deactivate() {
  if (activeModalPanel) {
    try { activeModalPanel.dispose(); } catch (e) {}
  }
}

module.exports = {
  activate,
  deactivate
};
