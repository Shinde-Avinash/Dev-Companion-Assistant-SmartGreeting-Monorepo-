using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using SmartGreeting.Core.Models;
using SmartGreeting.Core.Services;
using SmartGreeting.UI;

namespace SmartGreeting.Preview
{
    public partial class MainWindow : Window
    {
        private readonly ISettingsService _settingsService;
        private readonly ITimeContextService _timeContextService;
        private readonly IMessageService _messageService;
        private readonly IGreetingCoordinator _coordinator;

        public MainWindow()
        {
            InitializeComponent();

            _settingsService = new LocalSettingsService();
            _timeContextService = new TimeContextService();
            _messageService = new GreetingMessageService();
            _coordinator = new GreetingCoordinator(_settingsService, _timeContextService, _messageService);

            LoadCurrentSettingsToUI();
        }

        private void LoadCurrentSettingsToUI()
        {
            var settings = _settingsService.CurrentSettings;
            NameInput.Text = settings.Name;
            StartHourInput.Text = settings.WorkingHoursStart.ToString(@"hh\:mm");
            EndHourInput.Text = settings.WorkingHoursEnd.ToString(@"hh\:mm");
            DismissSecondsInput.Text = settings.AutoDismissSeconds.ToString();
            SoundCheckBox.IsChecked = settings.Sound;
            LateNightCheckBox.IsChecked = settings.LateNightMessages;
            WeekendCheckBox.IsChecked = settings.WeekendMessages;
        }

        private GreetingSettings GetSettingsFromUI()
        {
            var settings = _settingsService.CurrentSettings.Clone();
            settings.Name = string.IsNullOrWhiteSpace(NameInput.Text) ? "Developer" : NameInput.Text.Trim();

            if (TimeSpan.TryParse(StartHourInput.Text, out var start))
            {
                settings.WorkingHoursStart = start;
            }
            if (TimeSpan.TryParse(EndHourInput.Text, out var end))
            {
                settings.WorkingHoursEnd = end;
            }
            if (int.TryParse(DismissSecondsInput.Text, out var seconds))
            {
                settings.AutoDismissSeconds = Math.Clamp(seconds, 3, 60);
            }

            settings.Sound = SoundCheckBox.IsChecked == true;
            settings.LateNightMessages = LateNightCheckBox.IsChecked == true;
            settings.WeekendMessages = WeekendCheckBox.IsChecked == true;

            return settings;
        }

        private void TriggerSpecificGreeting(DateTime mockDateTime, string label)
        {
            var settings = GetSettingsFromUI();
            var context = _timeContextService.Evaluate(mockDateTime, settings);
            var message = _messageService.SelectMessage(context, settings);

            var payload = new GreetingPayload
            {
                ShouldDisplay = true,
                Context = context,
                Message = message,
                Settings = settings,
                FormattedHeadline = message.FormatHeadline(settings.Name),
                FormattedBody = message.Body,
                Reason = "Manual Preview Trigger"
            };

            var window = new GreetingWindow(payload);
            window.Show();

            StatusTextBlock.Text = $"Triggered: {label} ({mockDateTime:yyyy-MM-dd HH:mm}) -> {payload.FormattedHeadline}";
        }

        private void OnLiveSystemTimeClicked(object sender, RoutedEventArgs e)
        {
            var payload = _coordinator.PrepareGreeting(isManualTrigger: true);
            var window = new GreetingWindow(payload);
            window.Show();
            StatusTextBlock.Text = $"Triggered live greeting based on current local clock: {payload.FormattedHeadline}";
        }

        private void OnMorningClicked(object sender, RoutedEventArgs e)
        {
            // 08:30 AM Wednesday
            TriggerSpecificGreeting(new DateTime(2026, 10, 21, 8, 30, 0), "Morning Sunrise");
        }

        private void OnAfternoonClicked(object sender, RoutedEventArgs e)
        {
            // 14:15 PM Wednesday
            TriggerSpecificGreeting(new DateTime(2026, 10, 21, 14, 15, 0), "Afternoon Daylight");
        }

        private void OnEveningClicked(object sender, RoutedEventArgs e)
        {
            // 18:45 PM Wednesday
            TriggerSpecificGreeting(new DateTime(2026, 10, 21, 18, 45, 0), "Evening Sunset");
        }

        private void OnLateNightClicked(object sender, RoutedEventArgs e)
        {
            // 23:20 PM Wednesday
            TriggerSpecificGreeting(new DateTime(2026, 10, 21, 23, 20, 0), "Late Night Focus");
        }

        private void OnMondayClicked(object sender, RoutedEventArgs e)
        {
            // Monday 09:15 AM
            TriggerSpecificGreeting(new DateTime(2026, 10, 19, 9, 15, 0), "Monday Motivation");
        }

        private void OnFridayClicked(object sender, RoutedEventArgs e)
        {
            // Friday 15:45 PM
            TriggerSpecificGreeting(new DateTime(2026, 10, 23, 15, 45, 0), "Friday Celebration");
        }

        private void OnWeekendClicked(object sender, RoutedEventArgs e)
        {
            // Saturday 11:30 AM
            TriggerSpecificGreeting(new DateTime(2026, 10, 24, 11, 30, 0), "Weekend Coding");
        }

        private void OnSaveSettingsClicked(object sender, RoutedEventArgs e)
        {
            var updated = GetSettingsFromUI();
            _settingsService.SaveSettings(updated);
            StatusTextBlock.Text = "Settings successfully saved to %LOCALAPPDATA%\\SmartGreeting\\settings.json";
        }

        private void OnOpenLogsClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SmartGreeting");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                Process.Start(new ProcessStartInfo("explorer.exe", dir) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Unable to open explorer: {ex.Message}";
            }
        }
    }
}
