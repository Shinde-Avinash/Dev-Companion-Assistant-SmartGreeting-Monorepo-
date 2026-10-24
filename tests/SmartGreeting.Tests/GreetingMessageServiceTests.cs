using System;
using System.Linq;
using SmartGreeting.Core.Models;
using SmartGreeting.Core.Services;
using Xunit;

namespace SmartGreeting.Tests
{
    public class GreetingMessageServiceTests
    {
        private readonly GreetingMessageService _service;
        private readonly GreetingSettings _settings;

        public GreetingMessageServiceTests()
        {
            _service = new GreetingMessageService(seed: 42);
            _settings = new GreetingSettings
            {
                Name = "Avi",
                LateNightMessages = true,
                WeekendMessages = true
            };
        }

        [Fact]
        public void Personalization_Formats_Configured_Name()
        {
            var msg = new GreetingMessage
            {
                HeadlineTemplate = "Good Morning, {Name}!"
            };

            string formatted = msg.FormatHeadline("Avi");
            Assert.Equal("Good Morning, Avi!", formatted);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Personalization_Falls_Back_To_Developer_When_Empty(string? emptyName)
        {
            var msg = new GreetingMessage
            {
                HeadlineTemplate = "Good Morning, {Name}!"
            };

            string formatted = msg.FormatHeadline(emptyName);
            Assert.Equal("Good Morning, Developer!", formatted);
        }

        [Fact]
        public void SelectMessage_Prevents_Immediate_Consecutive_Duplicate()
        {
            var context = new TimeContext
            {
                DateTime = new DateTime(2026, 10, 21, 9, 0, 0),
                Period = TimePeriod.Morning
            };

            var first = _service.SelectMessage(context, _settings, lastMessageId: null);
            var second = _service.SelectMessage(context, _settings, lastMessageId: first.Id);

            Assert.NotNull(first);
            Assert.NotNull(second);
            Assert.NotEqual(first.Id, second.Id);
        }

        [Fact]
        public void SelectMessage_Picks_Monday_Message_On_Monday()
        {
            var context = new TimeContext
            {
                DateTime = new DateTime(2026, 10, 19, 9, 0, 0), // Monday
                Period = TimePeriod.Morning
            };

            var msg = _service.SelectMessage(context, _settings);
            Assert.NotNull(msg);
        }

        [Fact]
        public void SelectMessage_Picks_Friday_Message_On_Friday()
        {
            var context = new TimeContext
            {
                DateTime = new DateTime(2026, 10, 23, 14, 0, 0), // Friday
                Period = TimePeriod.Afternoon
            };

            var msg = _service.SelectMessage(context, _settings);
            Assert.NotNull(msg);
        }

        [Fact]
        public void SelectMessage_Falls_Back_From_LateNight_When_Disabled()
        {
            var disabledLateNightSettings = new GreetingSettings
            {
                Name = "Avi",
                LateNightMessages = false
            };

            var context = new TimeContext
            {
                DateTime = new DateTime(2026, 10, 21, 23, 0, 0),
                Period = TimePeriod.LateNight
            };

            var msg = _service.SelectMessage(context, disabledLateNightSettings);
            Assert.NotNull(msg);
            Assert.Equal(TimePeriod.Evening, msg.Period);
        }
    }
}
