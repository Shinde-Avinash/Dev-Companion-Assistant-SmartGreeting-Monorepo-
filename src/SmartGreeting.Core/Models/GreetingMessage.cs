using System;

namespace SmartGreeting.Core.Models
{
    public class GreetingMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Title { get; set; } = string.Empty;
        public string HeadlineTemplate { get; set; } = "Hello, {Name}!";
        public string Body { get; set; } = string.Empty;
        public string SubtleMessage { get; set; } = string.Empty;
        public string IconGlyph { get; set; } = "✨";
        public TimePeriod Period { get; set; } = TimePeriod.Morning;
        public string Tag { get; set; } = string.Empty;

        public string FormatHeadline(string? name)
        {
            string displayName = string.IsNullOrWhiteSpace(name) ? "Developer" : name!.Trim();
            return HeadlineTemplate.Replace("{Name}", displayName);
        }
    }
}
