using System;
using SmartGreeting.Core.Models;
using SmartGreeting.Core.Services;
using Xunit;

namespace SmartGreeting.Tests
{
    public class GreetingCoordinatorTests
    {
        private class MockDateTimeProvider : IDateTimeProvider
        {
            public DateTime Current { get; set; } = new DateTime(2026, 10, 21, 10, 0, 0, DateTimeKind.Utc);
            public DateTime Now => Current.ToLocalTime();
            public DateTime UtcNow => Current;
        }

        private class MockSettingsService : ISettingsService
        {
            public GreetingSettings CurrentSettings { get; set; } = new GreetingSettings();
            public DateTime? LastShownUtc { get; set; }
            public string? LastMessageId { get; set; }

            public GreetingSettings LoadSettings() => CurrentSettings;
            public void SaveSettings(GreetingSettings settings) => CurrentSettings = settings;
            public DateTime? GetLastGreetingTimeUtc() => LastShownUtc;
            public string? GetLastMessageId() => LastMessageId;
            public void RecordGreetingShown(string messageId)
            {
                LastShownUtc = DateTime.UtcNow;
                LastMessageId = messageId;
            }
        }

        [Fact]
        public void ShouldShowGreeting_Suppressed_When_Extension_Disabled()
        {
            var settings = new MockSettingsService();
            settings.CurrentSettings.Enabled = false;

            var coordinator = new GreetingCoordinator(
                settings,
                new TimeContextService(),
                new GreetingMessageService()
            );

            Assert.False(coordinator.ShouldShowGreeting(isManualTrigger: false));
            // Manual trigger should still be permitted
            Assert.True(coordinator.ShouldShowGreeting(isManualTrigger: true));
        }

        [Fact]
        public void ShouldShowGreeting_OncePerSession_Policy()
        {
            var settings = new MockSettingsService();
            settings.CurrentSettings.Frequency = GreetingFrequency.OncePerSession;

            var coordinator = new GreetingCoordinator(
                settings,
                new TimeContextService(),
                new GreetingMessageService()
            );

            // First time in session: true
            Assert.True(coordinator.ShouldShowGreeting(isManualTrigger: false));
            var payload = coordinator.PrepareGreeting(isManualTrigger: false);
            Assert.True(payload.ShouldDisplay);

            // Second time in same session: false
            Assert.False(coordinator.ShouldShowGreeting(isManualTrigger: false));

            // Manual trigger still works
            Assert.True(coordinator.ShouldShowGreeting(isManualTrigger: true));
        }

        [Fact]
        public void ShouldShowGreeting_OncePerDay_Policy()
        {
            var clock = new MockDateTimeProvider
            {
                Current = new DateTime(2026, 10, 21, 10, 0, 0, DateTimeKind.Utc)
            };

            var settings = new MockSettingsService();
            settings.CurrentSettings.Frequency = GreetingFrequency.OncePerDay;
            // Last shown earlier today
            settings.LastShownUtc = new DateTime(2026, 10, 21, 7, 0, 0, DateTimeKind.Utc);

            var coordinator = new GreetingCoordinator(
                settings,
                new TimeContextService(clock),
                new GreetingMessageService(),
                clock
            );

            // Already shown today -> false
            Assert.False(coordinator.ShouldShowGreeting(isManualTrigger: false));

            // Advance clock to next day
            clock.Current = new DateTime(2026, 10, 22, 9, 0, 0, DateTimeKind.Utc);

            // Now should be true!
            Assert.True(coordinator.ShouldShowGreeting(isManualTrigger: false));
        }

        [Fact]
        public void ShouldShowGreeting_ManualOnly_Policy()
        {
            var settings = new MockSettingsService();
            settings.CurrentSettings.Frequency = GreetingFrequency.ManualOnly;

            var coordinator = new GreetingCoordinator(
                settings,
                new TimeContextService(),
                new GreetingMessageService()
            );

            Assert.False(coordinator.ShouldShowGreeting(isManualTrigger: false));
            Assert.True(coordinator.ShouldShowGreeting(isManualTrigger: true));
        }
    }
}
