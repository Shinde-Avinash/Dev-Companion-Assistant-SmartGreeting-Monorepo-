using System;

namespace SmartGreeting.Core.Models
{
    public class GreetingSettings
    {
        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "Avi";
        public TimeSpan WorkingHoursStart { get; set; } = new TimeSpan(9, 0, 0); // 09:00 AM
        public TimeSpan WorkingHoursEnd { get; set; } = new TimeSpan(18, 0, 0);   // 06:00 PM
        public bool ShowOnStartup { get; set; } = true;
        public bool ShowOnSolutionOpen { get; set; } = false;
        public GreetingFrequency Frequency { get; set; } = GreetingFrequency.OncePerSession;
        public bool LateNightMessages { get; set; } = true;
        public bool WeekendMessages { get; set; } = true;
        public bool Animations { get; set; } = true;
        public bool Sound { get; set; } = false;
        public GreetingTheme Theme { get; set; } = GreetingTheme.Auto;
        public int AutoDismissSeconds { get; set; } = 8;

        public GreetingSettings Clone()
        {
            return new GreetingSettings
            {
                Enabled = this.Enabled,
                Name = this.Name,
                WorkingHoursStart = this.WorkingHoursStart,
                WorkingHoursEnd = this.WorkingHoursEnd,
                ShowOnStartup = this.ShowOnStartup,
                ShowOnSolutionOpen = this.ShowOnSolutionOpen,
                Frequency = this.Frequency,
                LateNightMessages = this.LateNightMessages,
                WeekendMessages = this.WeekendMessages,
                Animations = this.Animations,
                Sound = this.Sound,
                Theme = this.Theme,
                AutoDismissSeconds = this.AutoDismissSeconds
            };
        }
    }
}
