using System;
using SmartGreeting.Core.Models;

namespace SmartGreeting.Core.Services
{
    public interface ITimeContextService
    {
        TimeContext GetCurrentContext(GreetingSettings settings);
        TimeContext Evaluate(DateTime dateTime, GreetingSettings settings);
    }

    public class TimeContextService : ITimeContextService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public TimeContextService(IDateTimeProvider? dateTimeProvider = null)
        {
            _dateTimeProvider = dateTimeProvider ?? new SystemDateTimeProvider();
        }

        public TimeContext GetCurrentContext(GreetingSettings settings)
        {
            return Evaluate(_dateTimeProvider.Now, settings);
        }

        public TimeContext Evaluate(DateTime dateTime, GreetingSettings settings)
        {
            TimeSpan timeOfDay = dateTime.TimeOfDay;
            TimePeriod period = ResolveTimePeriod(timeOfDay);
            WorkContext workContext = ResolveWorkContext(dateTime, period, settings);

            string headerLabel = period switch
            {
                TimePeriod.Morning => "☀️ GOOD MORNING",
                TimePeriod.Afternoon => "🌤️ GOOD AFTERNOON",
                TimePeriod.Evening => "🌆 GOOD EVENING",
                TimePeriod.LateNight => "🌙 LATE NIGHT",
                _ => "✨ SMART GREETING"
            };

            string iconGlyph = period switch
            {
                TimePeriod.Morning => "☀️",
                TimePeriod.Afternoon => "🌤️",
                TimePeriod.Evening => "🌆",
                TimePeriod.LateNight => "🌙",
                _ => "✨"
            };

            return new TimeContext
            {
                DateTime = dateTime,
                Period = period,
                WorkContext = workContext,
                HeaderLabel = headerLabel,
                IconGlyph = iconGlyph
            };
        }

        public static TimePeriod ResolveTimePeriod(TimeSpan timeOfDay)
        {
            // Morning:    05:00 <= t < 12:00
            // Afternoon:  12:00 <= t < 17:00
            // Evening:    17:00 <= t < 21:00
            // Late Night: 21:00 <= t < 24:00 OR 00:00 <= t < 05:00

            TimeSpan t0500 = new TimeSpan(5, 0, 0);
            TimeSpan t1200 = new TimeSpan(12, 0, 0);
            TimeSpan t1700 = new TimeSpan(17, 0, 0);
            TimeSpan t2100 = new TimeSpan(21, 0, 0);

            if (timeOfDay >= t0500 && timeOfDay < t1200)
            {
                return TimePeriod.Morning;
            }
            if (timeOfDay >= t1200 && timeOfDay < t1700)
            {
                return TimePeriod.Afternoon;
            }
            if (timeOfDay >= t1700 && timeOfDay < t2100)
            {
                return TimePeriod.Evening;
            }

            return TimePeriod.LateNight;
        }

        public static WorkContext ResolveWorkContext(DateTime dateTime, TimePeriod period, GreetingSettings settings)
        {
            // Check weekend first
            if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return WorkContext.Weekend;
            }

            // Late night check
            if (period == TimePeriod.LateNight)
            {
                return WorkContext.LateNight;
            }

            TimeSpan timeOfDay = dateTime.TimeOfDay;
            TimeSpan start = settings.WorkingHoursStart;
            TimeSpan end = settings.WorkingHoursEnd;

            // Handle standard working day (e.g. 09:00 - 18:00)
            if (start < end)
            {
                if (timeOfDay < start)
                {
                    return WorkContext.BeforeWork;
                }
                if (timeOfDay >= start && timeOfDay < end)
                {
                    return WorkContext.DuringWork;
                }
                return WorkContext.AfterWork;
            }
            else
            {
                // Overnight working shift
                if (timeOfDay >= start || timeOfDay < end)
                {
                    return WorkContext.DuringWork;
                }
                return WorkContext.AfterWork;
            }
        }
    }
}
