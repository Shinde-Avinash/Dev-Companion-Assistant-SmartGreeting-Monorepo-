using System;

namespace SmartGreeting.Core.Models
{
    public class TimeContext
    {
        public DateTime DateTime { get; set; }
        public TimePeriod Period { get; set; }
        public WorkContext WorkContext { get; set; }
        public DayOfWeek DayOfWeek => DateTime.DayOfWeek;
        public bool IsWeekend => DayOfWeek == DayOfWeek.Saturday || DayOfWeek == DayOfWeek.Sunday;
        public bool IsMonday => DayOfWeek == DayOfWeek.Monday;
        public bool IsFriday => DayOfWeek == DayOfWeek.Friday;
        public string HeaderLabel { get; set; } = string.Empty;
        public string IconGlyph { get; set; } = "✨";

        public override string ToString()
        {
            return $"{DateTime:yyyy-MM-dd HH:mm:ss} | {Period} | {WorkContext} | {DayOfWeek}";
        }
    }
}
