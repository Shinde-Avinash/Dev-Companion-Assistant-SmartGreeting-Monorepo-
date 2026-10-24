using System;
using System.Collections.Generic;
using System.Linq;
using SmartGreeting.Core.Models;

namespace SmartGreeting.Core.Services
{
    public interface IMessageService
    {
        GreetingMessage SelectMessage(TimeContext context, GreetingSettings settings, string? lastMessageId = null);
        IReadOnlyList<GreetingMessage> GetAllMessages();
    }

    public class GreetingMessageService : IMessageService
    {
        private readonly List<GreetingMessage> _messages = new List<GreetingMessage>();
        private readonly Random _random;

        public GreetingMessageService(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            InitializeMessageLibrary();
        }

        public IReadOnlyList<GreetingMessage> GetAllMessages() => _messages.AsReadOnly();

        public GreetingMessage SelectMessage(TimeContext context, GreetingSettings settings, string? lastMessageId = null)
        {
            List<GreetingMessage> candidates = new List<GreetingMessage>();

            // Day-specific overrides or additions
            if (context.IsMonday && settings.WeekendMessages)
            {
                candidates.AddRange(_messages.Where(m => m.Tag == "Monday"));
            }
            else if (context.IsFriday && settings.WeekendMessages)
            {
                candidates.AddRange(_messages.Where(m => m.Tag == "Friday"));
            }
            else if (context.IsWeekend && settings.WeekendMessages)
            {
                candidates.AddRange(_messages.Where(m => m.Tag == "Weekend"));
            }

            // If late night messages disabled, fall back to evening
            TimePeriod targetPeriod = context.Period;
            if (targetPeriod == TimePeriod.LateNight && !settings.LateNightMessages)
            {
                targetPeriod = TimePeriod.Evening;
            }

            // Period candidates
            var periodCandidates = _messages.Where(m => m.Period == targetPeriod && string.IsNullOrEmpty(m.Tag)).ToList();
            candidates.AddRange(periodCandidates);

            if (candidates.Count == 0)
            {
                candidates = _messages.Where(m => m.Period == targetPeriod).ToList();
            }

            if (candidates.Count == 0)
            {
                candidates = _messages.ToList();
            }

            // Anti-repetition: filter out last message ID if we have more than 1 candidate
            if (!string.IsNullOrEmpty(lastMessageId) && candidates.Count > 1)
            {
                var withoutLast = candidates.Where(m => m.Id != lastMessageId).ToList();
                if (withoutLast.Count > 0)
                {
                    candidates = withoutLast;
                }
            }

            int index = _random.Next(candidates.Count);
            return candidates[index];
        }

        private void InitializeMessageLibrary()
        {
            // Morning Messages (05:00 - 11:59)
            _messages.Add(new GreetingMessage
            {
                Id = "morning-1",
                Title = "☀️ GOOD MORNING",
                HeadlineTemplate = "Good Morning, {Name}!",
                Body = "Ready to turn ideas into great code?",
                SubtleMessage = "Every masterwork begins with a single line of clean code.",
                IconGlyph = "☀️",
                Period = TimePeriod.Morning
            });

            _messages.Add(new GreetingMessage
            {
                Id = "morning-2",
                Title = "☀️ GOOD MORNING",
                HeadlineTemplate = "Rise and shine, {Name}!",
                Body = "Fresh day, fresh canvas. What are we building today?",
                SubtleMessage = "Take your time, write with intention, and enjoy the flow.",
                IconGlyph = "☕",
                Period = TimePeriod.Morning
            });

            _messages.Add(new GreetingMessage
            {
                Id = "morning-3",
                Title = "☀️ GOOD MORNING",
                HeadlineTemplate = "Welcome back, {Name}!",
                Body = "Your coffee is hot and the compiler is ready.",
                SubtleMessage = "Pro tip: Clear thoughts make clean abstractions.",
                IconGlyph = "🚀",
                Period = TimePeriod.Morning
            });

            _messages.Add(new GreetingMessage
            {
                Id = "morning-4",
                Title = "☀️ GOOD MORNING",
                HeadlineTemplate = "Good Morning, {Name}!",
                Body = "A great day to solve tricky problems with elegance.",
                SubtleMessage = "Stay curious and embrace each challenge.",
                IconGlyph = "💡",
                Period = TimePeriod.Morning
            });

            // Afternoon Messages (12:00 - 16:59)
            _messages.Add(new GreetingMessage
            {
                Id = "afternoon-1",
                Title = "🌤️ GOOD AFTERNOON",
                HeadlineTemplate = "Good Afternoon, {Name}!",
                Body = "Keep the momentum going!",
                SubtleMessage = "Midday progress fuels evening satisfaction.",
                IconGlyph = "🌤️",
                Period = TimePeriod.Afternoon
            });

            _messages.Add(new GreetingMessage
            {
                Id = "afternoon-2",
                Title = "🌤️ GOOD AFTERNOON",
                HeadlineTemplate = "In the zone, {Name}?",
                Body = "You're making great strides today. Stay focused and steady.",
                SubtleMessage = "Remember to stay hydrated and stretch your shoulders.",
                IconGlyph = "⚡",
                Period = TimePeriod.Afternoon
            });

            _messages.Add(new GreetingMessage
            {
                Id = "afternoon-3",
                Title = "🌤️ GOOD AFTERNOON",
                HeadlineTemplate = "Good Afternoon, {Name}!",
                Body = "Great code takes patience. Keep refactoring and refining.",
                SubtleMessage = "Small optimizations compound into great software.",
                IconGlyph = "🎯",
                Period = TimePeriod.Afternoon
            });

            // Evening Messages (17:00 - 20:59)
            _messages.Add(new GreetingMessage
            {
                Id = "evening-1",
                Title = "🌆 GOOD EVENING",
                HeadlineTemplate = "Good Evening, {Name}!",
                Body = "Let's finish strong!",
                SubtleMessage = "Wrap up loose ends and set yourself up for tomorrow.",
                IconGlyph = "🌆",
                Period = TimePeriod.Evening
            });

            _messages.Add(new GreetingMessage
            {
                Id = "evening-2",
                Title = "🌆 GOOD EVENING",
                HeadlineTemplate = "Sunset coding, {Name}!",
                Body = "Reflect on today's commits. You did meaningful work.",
                SubtleMessage = "Don't forget to push your branches before signing off.",
                IconGlyph = "🌇",
                Period = TimePeriod.Evening
            });

            _messages.Add(new GreetingMessage
            {
                Id = "evening-3",
                Title = "🌆 GOOD EVENING",
                HeadlineTemplate = "Good Evening, {Name}!",
                Body = "Winding down or entering deep work mode?",
                SubtleMessage = "Either way, celebrate the bugs you solved today.",
                IconGlyph = "✨",
                Period = TimePeriod.Evening
            });

            // Late Night Messages (21:00 - 04:59)
            _messages.Add(new GreetingMessage
            {
                Id = "latenight-1",
                Title = "🌙 LATE NIGHT",
                HeadlineTemplate = "Still coding at this hour, {Name}?",
                Body = "Don't forget to take a break. Your health matters most.",
                SubtleMessage = "Night silence brings focus, but sleep brings clarity.",
                IconGlyph = "🌙",
                Period = TimePeriod.LateNight
            });

            _messages.Add(new GreetingMessage
            {
                Id = "latenight-2",
                Title = "🌙 LATE NIGHT",
                HeadlineTemplate = "Burning the midnight oil, {Name}?",
                Body = "Deep night creativity is real, but pace yourself.",
                SubtleMessage = "Save your work, rest your eyes, and recharge soon.",
                IconGlyph = "🌌",
                Period = TimePeriod.LateNight
            });

            _messages.Add(new GreetingMessage
            {
                Id = "latenight-3",
                Title = "🌙 LATE NIGHT",
                HeadlineTemplate = "Quiet hours, {Name} 🎧",
                Body = "Solving mysteries while the world sleeps. Respect the craft.",
                SubtleMessage = "Drink a glass of water and wrap up this feature.",
                IconGlyph = "🦉",
                Period = TimePeriod.LateNight
            });

            // Monday Special
            _messages.Add(new GreetingMessage
            {
                Id = "special-monday-1",
                Title = "☕ MONDAY MOTIVATION",
                HeadlineTemplate = "Monday already, {Name}? 😅",
                Body = "Let's make it productive and set a winning pace for the week.",
                SubtleMessage = "One task at a time. You've got this!",
                IconGlyph = "🎯",
                Period = TimePeriod.Morning,
                Tag = "Monday"
            });

            // Friday Special
            _messages.Add(new GreetingMessage
            {
                Id = "special-friday-1",
                Title = "🚀 FRIDAY VIBES",
                HeadlineTemplate = "It's Friday, {Name}! 🚀",
                Body = "Finish strong, merge cleanly, and prepare for a great weekend!",
                SubtleMessage = "No dangerous deployments on Friday afternoon! 😉",
                IconGlyph = "🎉",
                Period = TimePeriod.Afternoon,
                Tag = "Friday"
            });

            // Weekend Special
            _messages.Add(new GreetingMessage
            {
                Id = "special-weekend-1",
                Title = "💻 WEEKEND WARRIOR",
                HeadlineTemplate = "Weekend coding, {Name}? Respect. 💻",
                Body = "Working on passions and side projects? Enjoy the craft!",
                SubtleMessage = "Don't forget to step outside and take some time away from screens.",
                IconGlyph = "🌟",
                Period = TimePeriod.Afternoon,
                Tag = "Weekend"
            });
        }
    }
}
