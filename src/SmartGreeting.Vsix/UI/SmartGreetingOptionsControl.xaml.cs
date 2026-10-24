using System;
using System.Windows.Controls;
using SmartGreeting.Core.Models;
using SmartGreeting.Core.Services;

namespace SmartGreeting.Vsix.UI
{
    public partial class SmartGreetingOptionsControl : UserControl
    {
        private readonly ISettingsService _settingsService;

        public SmartGreetingOptionsControl()
        {
            InitializeComponent();
            _settingsService = new LocalSettingsService();
            LoadSettings();
        }

        public void LoadSettings()
        {
            var settings = _settingsService.LoadSettings();

            EnabledBox.IsChecked = settings.Enabled;
            NameInput.Text = settings.Name;
            StartHourInput.Text = settings.WorkingHoursStart.ToString(@"hh\:mm");
            EndHourInput.Text = settings.WorkingHoursEnd.ToString(@"hh\:mm");
            StartupBox.IsChecked = settings.ShowOnStartup;
            SolutionOpenBox.IsChecked = settings.ShowOnSolutionOpen;
            LateNightBox.IsChecked = settings.LateNightMessages;
            WeekendBox.IsChecked = settings.WeekendMessages;
            AnimationsBox.IsChecked = settings.Animations;
            SoundBox.IsChecked = settings.Sound;
            DismissSecondsInput.Text = settings.AutoDismissSeconds.ToString();

            FrequencyCombo.SelectedIndex = settings.Frequency switch
            {
                GreetingFrequency.OncePerSession => 0,
                GreetingFrequency.OncePerDay => 1,
                GreetingFrequency.EveryStartup => 2,
                GreetingFrequency.ManualOnly => 3,
                _ => 0
            };
        }

        public void SaveSettings()
        {
            var settings = _settingsService.CurrentSettings.Clone();

            settings.Enabled = EnabledBox.IsChecked == true;
            settings.Name = string.IsNullOrWhiteSpace(NameInput.Text) ? "Developer" : NameInput.Text.Trim();

            if (TimeSpan.TryParse(StartHourInput.Text, out var start))
            {
                settings.WorkingHoursStart = start;
            }
            if (TimeSpan.TryParse(EndHourInput.Text, out var end))
            {
                settings.WorkingHoursEnd = end;
            }

            settings.ShowOnStartup = StartupBox.IsChecked == true;
            settings.ShowOnSolutionOpen = SolutionOpenBox.IsChecked == true;
            settings.LateNightMessages = LateNightBox.IsChecked == true;
            settings.WeekendMessages = WeekendBox.IsChecked == true;
            settings.Animations = AnimationsBox.IsChecked == true;
            settings.Sound = SoundBox.IsChecked == true;

            if (int.TryParse(DismissSecondsInput.Text, out var seconds))
            {
                settings.AutoDismissSeconds = Math.Clamp(seconds, 3, 60);
            }

            settings.Frequency = FrequencyCombo.SelectedIndex switch
            {
                0 => GreetingFrequency.OncePerSession,
                1 => GreetingFrequency.OncePerDay,
                2 => GreetingFrequency.EveryStartup,
                3 => GreetingFrequency.ManualOnly,
                _ => GreetingFrequency.OncePerSession
            };

            _settingsService.SaveSettings(settings);
        }
    }
}
