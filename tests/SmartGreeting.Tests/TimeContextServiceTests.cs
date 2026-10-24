using System;
using SmartGreeting.Core.Models;
using SmartGreeting.Core.Services;
using Xunit;

namespace SmartGreeting.Tests
{
    public class TimeContextServiceTests
    {
        private readonly TimeContextService _service;
        private readonly GreetingSettings _settings;

        public TimeContextServiceTests()
        {
            _service = new TimeContextService();
            _settings = new GreetingSettings
            {
                WorkingHoursStart = new TimeSpan(9, 0, 0),
                WorkingHoursEnd = new TimeSpan(18, 0, 0)
            };
        }

        [Theory]
        [InlineData(5, 0, 0, TimePeriod.Morning)]
        [InlineData(11, 59, 59, TimePeriod.Morning)]
        [InlineData(12, 0, 0, TimePeriod.Afternoon)]
        [InlineData(16, 59, 59, TimePeriod.Afternoon)]
        [InlineData(17, 0, 0, TimePeriod.Evening)]
        [InlineData(20, 59, 59, TimePeriod.Evening)]
        [InlineData(21, 0, 0, TimePeriod.LateNight)]
        [InlineData(23, 59, 59, TimePeriod.LateNight)]
        [InlineData(0, 0, 0, TimePeriod.LateNight)]
        [InlineData(4, 59, 59, TimePeriod.LateNight)]
        public void Evaluate_Returns_Correct_TimePeriod_Boundaries(int hour, int minute, int second, TimePeriod expectedPeriod)
        {
            // Given: a Wednesday with the specified time
            DateTime dt = new DateTime(2026, 10, 21, hour, minute, second);

            // When:
            TimeContext context = _service.Evaluate(dt, _settings);

            // Then:
            Assert.Equal(expectedPeriod, context.Period);
        }

        [Fact]
        public void Evaluate_Detects_Monday_Correctly()
        {
            // Monday 2026-10-19 at 09:30 AM
            DateTime monday = new DateTime(2026, 10, 19, 9, 30, 0);
            TimeContext context = _service.Evaluate(monday, _settings);

            Assert.True(context.IsMonday);
            Assert.False(context.IsFriday);
            Assert.False(context.IsWeekend);
        }

        [Fact]
        public void Evaluate_Detects_Friday_Correctly()
        {
            // Friday 2026-10-23 at 14:00 PM
            DateTime friday = new DateTime(2026, 10, 23, 14, 0, 0);
            TimeContext context = _service.Evaluate(friday, _settings);

            Assert.True(context.IsFriday);
            Assert.False(context.IsMonday);
            Assert.False(context.IsWeekend);
        }

        [Theory]
        [InlineData(2026, 10, 24)] // Saturday
        [InlineData(2026, 10, 25)] // Sunday
        public void Evaluate_Detects_Weekend_Correctly(int year, int month, int day)
        {
            DateTime weekendDate = new DateTime(year, month, day, 11, 0, 0);
            TimeContext context = _service.Evaluate(weekendDate, _settings);

            Assert.True(context.IsWeekend);
            Assert.Equal(WorkContext.Weekend, context.WorkContext);
        }

        [Fact]
        public void Evaluate_Detects_BeforeWork_On_Weekday()
        {
            // Tuesday at 07:30 AM (working hours start at 09:00 AM)
            DateTime dt = new DateTime(2026, 10, 20, 7, 30, 0);
            TimeContext context = _service.Evaluate(dt, _settings);

            Assert.Equal(WorkContext.BeforeWork, context.WorkContext);
        }

        [Fact]
        public void Evaluate_Detects_DuringWork_On_Weekday()
        {
            // Tuesday at 14:15 PM (working hours 09:00 - 18:00)
            DateTime dt = new DateTime(2026, 10, 20, 14, 15, 0);
            TimeContext context = _service.Evaluate(dt, _settings);

            Assert.Equal(WorkContext.DuringWork, context.WorkContext);
        }

        [Fact]
        public void Evaluate_Detects_AfterWork_On_Weekday()
        {
            // Tuesday at 19:00 PM (working hours end at 18:00, evening period)
            DateTime dt = new DateTime(2026, 10, 20, 19, 0, 0);
            TimeContext context = _service.Evaluate(dt, _settings);

            Assert.Equal(WorkContext.AfterWork, context.WorkContext);
        }

        [Fact]
        public void Evaluate_Custom_Working_Hours()
        {
            // Custom hours: 10:30 to 19:30
            var customSettings = new GreetingSettings
            {
                WorkingHoursStart = new TimeSpan(10, 30, 0),
                WorkingHoursEnd = new TimeSpan(19, 30, 0)
            };

            DateTime dt1000 = new DateTime(2026, 10, 20, 10, 0, 0);
            DateTime dt1100 = new DateTime(2026, 10, 20, 11, 0, 0);
            DateTime dt2000 = new DateTime(2026, 10, 20, 20, 0, 0);

            Assert.Equal(WorkContext.BeforeWork, _service.Evaluate(dt1000, customSettings).WorkContext);
            Assert.Equal(WorkContext.DuringWork, _service.Evaluate(dt1100, customSettings).WorkContext);
            Assert.Equal(WorkContext.AfterWork, _service.Evaluate(dt2000, customSettings).WorkContext);
        }
    }
}
