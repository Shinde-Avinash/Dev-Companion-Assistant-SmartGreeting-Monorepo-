using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SmartGreeting.Core.Models;
using SmartGreeting.Core.Services;
using SmartGreeting.UI;
using SmartGreeting.Vsix.UI;

namespace SmartGreeting.Vsix
{
    public static class PackageGuids
    {
        public const string PackageGuidString = "6e5c9a72-74d1-4db9-96d5-45118b6e3f20";
        public static readonly Guid PackageGuid = new Guid(PackageGuidString);

        public const string CommandSetGuidString = "9b31d8e6-e176-4c48-8dfa-8a1a45749f55";
        public static readonly Guid CommandSet = new Guid(CommandSetGuidString);

        public const int ShowGreetingCommandId = 0x0100;
        public const int SettingsCommandId = 0x0101;
        public const int AboutCommandId = 0x0102;
    }

    public class SmartGreetingPackage
    {
        private static SmartGreetingPackage? _instance;
        private readonly ISettingsService _settingsService;
        private readonly ITimeContextService _timeContextService;
        private readonly IMessageService _messageService;
        private readonly IGreetingCoordinator _coordinator;
        private readonly ILoggerService _logger;

        public static SmartGreetingPackage Instance => _instance ??= new SmartGreetingPackage();

        public SmartGreetingPackage()
        {
            _logger = new FileLoggerService();
            _settingsService = new LocalSettingsService(_logger);
            _timeContextService = new TimeContextService();
            _messageService = new GreetingMessageService();
            _coordinator = new GreetingCoordinator(_settingsService, _timeContextService, _messageService, null, _logger);
        }

        public void Initialize()
        {
            try
            {
                _logger.LogInfo("SmartGreeting extension package initialized.");

                // Trigger greeting asynchronously after slight pause to ensure IDE is responsive
                Task.Run(async () =>
                {
                    await Task.Delay(1200);
                    OnIdeStartup();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during SmartGreeting initialization.", ex);
            }
        }

        public void OnIdeStartup()
        {
            try
            {
                if (_coordinator.ShouldShowGreeting(isManualTrigger: false, isSolutionOpen: false))
                {
                    var payload = _coordinator.PrepareGreeting(isManualTrigger: false);
                    DisplayGreetingOnUi(payload);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during IDE startup greeting.", ex);
            }
        }

        public void OnSolutionOpened()
        {
            try
            {
                if (_coordinator.ShouldShowGreeting(isManualTrigger: false, isSolutionOpen: true))
                {
                    var payload = _coordinator.PrepareGreeting(isManualTrigger: false);
                    DisplayGreetingOnUi(payload);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during solution open greeting.", ex);
            }
        }

        public void ShowGreetingManual()
        {
            try
            {
                _logger.LogInfo("Manual greeting triggered by user.");
                var payload = _coordinator.PrepareGreeting(isManualTrigger: true);
                DisplayGreetingOnUi(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error executing manual greeting.", ex);
            }
        }

        public void ShowSettings()
        {
            try
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    var window = new Window
                    {
                        Title = "SmartGreeting Settings",
                        Content = new SmartGreetingOptionsControl(),
                        Width = 520,
                        Height = 580,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Background = System.Windows.Media.Brushes.White
                    };
                    window.ShowDialog();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error opening settings window.", ex);
            }
        }

        public void ShowAbout()
        {
            try
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    var about = new AboutWindow();
                    about.ShowDialog();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error opening about window.", ex);
            }
        }

        private void DisplayGreetingOnUi(GreetingPayload payload)
        {
            if (payload == null || !payload.ShouldDisplay) return;

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                var window = new GreetingWindow(payload);
                window.Show();
            });
        }
    }
}
