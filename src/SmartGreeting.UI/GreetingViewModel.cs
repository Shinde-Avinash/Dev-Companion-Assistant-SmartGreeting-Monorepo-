using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using SmartGreeting.Core.Models;
using SmartGreeting.Core.Services;

namespace SmartGreeting.UI
{
    public class GreetingViewModel : INotifyPropertyChanged
    {
        private string _title = "☀️ GOOD MORNING";
        private string _headline = "Good Morning, Avi!";
        private string _body = "Ready to turn ideas into great code?";
        private string _subtleMessage = "Every masterwork begins with a single line of clean code.";
        private string _iconGlyph = "☀️";
        private Brush _accentBrush = new SolidColorBrush(Color.FromRgb(255, 179, 0));
        private LinearGradientBrush _cardGradient = CreateDefaultGradient();
        private Brush _textPrimaryBrush = new SolidColorBrush(Color.FromRgb(240, 240, 245));
        private Brush _textSecondaryBrush = new SolidColorBrush(Color.FromRgb(165, 170, 185));
        private Brush _cardBackgroundBrush = new SolidColorBrush(Color.FromArgb(240, 24, 26, 32));
        private Brush _cardBorderBrush = new SolidColorBrush(Color.FromArgb(120, 255, 179, 0));
        private double _progress = 100;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestClose;

        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        public string Headline
        {
            get => _headline;
            set => SetField(ref _headline, value);
        }

        public string Body
        {
            get => _body;
            set => SetField(ref _body, value);
        }

        public string SubtleMessage
        {
            get => _subtleMessage;
            set => SetField(ref _subtleMessage, value);
        }

        public string IconGlyph
        {
            get => _iconGlyph;
            set => SetField(ref _iconGlyph, value);
        }

        public Brush AccentBrush
        {
            get => _accentBrush;
            set => SetField(ref _accentBrush, value);
        }

        public LinearGradientBrush CardGradient
        {
            get => _cardGradient;
            set => SetField(ref _cardGradient, value);
        }

        public Brush TextPrimaryBrush
        {
            get => _textPrimaryBrush;
            set => SetField(ref _textPrimaryBrush, value);
        }

        public Brush TextSecondaryBrush
        {
            get => _textSecondaryBrush;
            set => SetField(ref _textSecondaryBrush, value);
        }

        public Brush CardBackgroundBrush
        {
            get => _cardBackgroundBrush;
            set => SetField(ref _cardBackgroundBrush, value);
        }

        public Brush CardBorderBrush
        {
            get => _cardBorderBrush;
            set => SetField(ref _cardBorderBrush, value);
        }

        public double Progress
        {
            get => _progress;
            set => SetField(ref _progress, value);
        }

        public void ApplyPayload(GreetingPayload payload)
        {
            if (payload == null) return;

            Title = payload.Message.Title;
            Headline = payload.FormattedHeadline;
            Body = payload.FormattedBody;
            SubtleMessage = payload.Message.SubtleMessage;
            IconGlyph = payload.Message.IconGlyph;

            ApplyTheme(payload.Context.Period, payload.Settings.Theme);
        }

        public void ApplyTheme(TimePeriod period, GreetingTheme theme)
        {
            bool isDark = true;
            if (theme == GreetingTheme.Light)
            {
                isDark = false;
            }

            if (isDark)
            {
                TextPrimaryBrush = new SolidColorBrush(Color.FromRgb(245, 247, 250));
                TextSecondaryBrush = new SolidColorBrush(Color.FromRgb(165, 175, 195));
            }
            else
            {
                TextPrimaryBrush = new SolidColorBrush(Color.FromRgb(25, 30, 45));
                TextSecondaryBrush = new SolidColorBrush(Color.FromRgb(95, 105, 125));
            }

            switch (period)
            {
                case TimePeriod.Morning:
                    // Sunrise gold/amber
                    AccentBrush = new SolidColorBrush(Color.FromRgb(255, 179, 0)); // Amber
                    CardBorderBrush = new SolidColorBrush(Color.FromArgb(160, 255, 183, 77));
                    CardBackgroundBrush = isDark
                        ? new SolidColorBrush(Color.FromArgb(245, 26, 22, 18))
                        : new SolidColorBrush(Color.FromArgb(248, 255, 252, 240));
                    CardGradient = CreateLinearGradient(
                        isDark ? Color.FromArgb(40, 255, 179, 0) : Color.FromArgb(40, 255, 224, 130),
                        Colors.Transparent);
                    break;

                case TimePeriod.Afternoon:
                    // Daylight Azure / Cyan
                    AccentBrush = new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Azure Blue
                    CardBorderBrush = new SolidColorBrush(Color.FromArgb(160, 100, 181, 246));
                    CardBackgroundBrush = isDark
                        ? new SolidColorBrush(Color.FromArgb(245, 18, 24, 32))
                        : new SolidColorBrush(Color.FromArgb(248, 242, 248, 255));
                    CardGradient = CreateLinearGradient(
                        isDark ? Color.FromArgb(40, 33, 150, 243) : Color.FromArgb(40, 187, 222, 251),
                        Colors.Transparent);
                    break;

                case TimePeriod.Evening:
                    // Sunset Coral / Violet
                    AccentBrush = new SolidColorBrush(Color.FromRgb(244, 81, 30)); // Sunset Coral
                    CardBorderBrush = new SolidColorBrush(Color.FromArgb(160, 255, 138, 101));
                    CardBackgroundBrush = isDark
                        ? new SolidColorBrush(Color.FromArgb(245, 28, 20, 28))
                        : new SolidColorBrush(Color.FromArgb(248, 255, 244, 246));
                    CardGradient = CreateLinearGradient(
                        isDark ? Color.FromArgb(40, 244, 81, 30) : Color.FromArgb(40, 255, 204, 188),
                        Colors.Transparent);
                    break;

                case TimePeriod.LateNight:
                default:
                    // Midnight Indigo / Purple
                    AccentBrush = new SolidColorBrush(Color.FromRgb(126, 87, 194)); // Deep Purple/Indigo
                    CardBorderBrush = new SolidColorBrush(Color.FromArgb(160, 179, 157, 219));
                    CardBackgroundBrush = isDark
                        ? new SolidColorBrush(Color.FromArgb(245, 18, 16, 30))
                        : new SolidColorBrush(Color.FromArgb(248, 246, 244, 255));
                    CardGradient = CreateLinearGradient(
                        isDark ? Color.FromArgb(45, 126, 87, 194) : Color.FromArgb(40, 209, 196, 233),
                        Colors.Transparent);
                    break;
            }
        }

        public void Close()
        {
            RequestClose?.Invoke();
        }

        private static LinearGradientBrush CreateDefaultGradient()
        {
            return CreateLinearGradient(Color.FromArgb(35, 255, 179, 0), Colors.Transparent);
        }

        private static LinearGradientBrush CreateLinearGradient(Color start, Color end)
        {
            var brush = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(1, 1)
            };
            brush.GradientStops.Add(new GradientStop(start, 0.0));
            brush.GradientStops.Add(new GradientStop(end, 1.0));
            return brush;
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
