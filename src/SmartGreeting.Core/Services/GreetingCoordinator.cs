using System;
using SmartGreeting.Core.Models;

namespace SmartGreeting.Core.Services
{
    public interface IGreetingCoordinator
    {
        bool ShouldShowGreeting(bool isManualTrigger, bool isSolutionOpen = false);
        GreetingPayload PrepareGreeting(bool isManualTrigger);
    }

    public class GreetingPayload
    {
        public bool ShouldDisplay { get; set; }
        public TimeContext Context { get; set; } = null!;
        public GreetingMessage Message { get; set; } = null!;
        public GreetingSettings Settings { get; set; } = null!;
        public string FormattedHeadline { get; set; } = string.Empty;
        public string FormattedBody { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    public class GreetingCoordinator : IGreetingCoordinator
    {
        private readonly ISettingsService _settingsService;
        private readonly ITimeContextService _timeContextService;
        private readonly IMessageService _messageService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILoggerService _logger;

        private bool _shownInCurrentSession = false;

        public GreetingCoordinator(
            ISettingsService settingsService,
            ITimeContextService timeContextService,
            IMessageService messageService,
            IDateTimeProvider? dateTimeProvider = null,
            ILoggerService? logger = null)
        {
            _settingsService = settingsService;
            _timeContextService = timeContextService;
            _messageService = messageService;
            _dateTimeProvider = dateTimeProvider ?? new SystemDateTimeProvider();
            _logger = logger ?? new FileLoggerService();
        }

        public bool ShouldShowGreeting(bool isManualTrigger, bool isSolutionOpen = false)
        {
            var settings = _settingsService.CurrentSettings;

            if (isManualTrigger)
            {
                return true;
            }

            if (!settings.Enabled)
            {
                _logger.LogInfo("SmartGreeting is disabled in settings. Skipping.");
                return false;
            }

            if (isSolutionOpen && !settings.ShowOnSolutionOpen)
            {
                return false;
            }

            if (!isSolutionOpen && !settings.ShowOnStartup)
            {
                return false;
            }

            switch (settings.Frequency)
            {
                case GreetingFrequency.ManualOnly:
                    return false;

                case GreetingFrequency.EveryStartup:
                    return true;

                case GreetingFrequency.OncePerSession:
                    return !_shownInCurrentSession;

                case GreetingFrequency.OncePerDay:
                    var lastTimeUtc = _settingsService.GetLastGreetingTimeUtc();
                    if (!lastTimeUtc.HasValue)
                    {
                        return true;
                    }
                    var today = _dateTimeProvider.UtcNow.Date;
                    var lastDate = lastTimeUtc.Value.Date;
                    return today > lastDate;

                default:
                    return true;
            }
        }

        public GreetingPayload PrepareGreeting(bool isManualTrigger)
        {
            var settings = _settingsService.CurrentSettings;
            var context = _timeContextService.GetCurrentContext(settings);

            if (!isManualTrigger && !ShouldShowGreeting(isManualTrigger))
            {
                return new GreetingPayload
                {
                    ShouldDisplay = false,
                    Reason = "Suppressed by frequency or configuration."
                };
            }

            string? lastMessageId = _settingsService.GetLastMessageId();
            var message = _messageService.SelectMessage(context, settings, lastMessageId);

            _shownInCurrentSession = true;
            _settingsService.RecordGreetingShown(message.Id);

            string headline = message.FormatHeadline(settings.Name);

            _logger.LogInfo($"Prepared greeting: '{headline}' for period {context.Period} (Manual: {isManualTrigger})");

            return new GreetingPayload
            {
                ShouldDisplay = true,
                Context = context,
                Message = message,
                Settings = settings,
                FormattedHeadline = headline,
                FormattedBody = message.Body,
                Reason = "Success"
            };
        }
    }
}
