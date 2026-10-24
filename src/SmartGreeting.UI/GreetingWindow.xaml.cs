using System;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using SmartGreeting.Core.Models;
using SmartGreeting.Core.Services;

namespace SmartGreeting.UI
{
    public partial class GreetingWindow : Window
    {
        private readonly DispatcherTimer _autoDismissTimer;
        private readonly DispatcherTimer _progressTimer;
        private readonly int _totalDurationMs;
        private int _elapsedMs = 0;
        private bool _isClosing = false;
        private bool _isPaused = false;
        private readonly GreetingViewModel _viewModel;

        public GreetingWindow(GreetingPayload payload)
        {
            InitializeComponent();

            _viewModel = new GreetingViewModel();
            _viewModel.ApplyPayload(payload);
            _viewModel.RequestClose += BeginCloseAnimation;
            DataContext = _viewModel;

            _totalDurationMs = Math.Max(3000, payload.Settings.AutoDismissSeconds * 1000);

            _autoDismissTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _autoDismissTimer.Tick += OnDismissTick;

            _progressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _progressTimer.Tick += OnProgressTick;

            Loaded += OnWindowLoaded;
            MouseEnter += (s, e) => _isPaused = true;
            MouseLeave += (s, e) => _isPaused = false;

            if (payload.Settings.Sound)
            {
                PlayGreetingSound();
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            PositionWindowBottomRight();
            AnimateEntrance();

            _autoDismissTimer.Start();
            _progressTimer.Start();
        }

        private void PositionWindowBottomRight()
        {
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - ActualWidth - 16;
            Top = workArea.Bottom - ActualHeight - 16;
        }

        private void AnimateEntrance()
        {
            var fadeIn = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(350))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var slideUp = new DoubleAnimation(24.0, 0.0, TimeSpan.FromMilliseconds(350))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            RootContainer.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            SlideTransform.BeginAnimation(TranslateTransform.YProperty, slideUp);
        }

        private void OnProgressTick(object? sender, EventArgs e)
        {
            if (_isPaused || _isClosing) return;

            _elapsedMs += 50;
            double remainingPercent = Math.Max(0.0, 100.0 - ((double)_elapsedMs / _totalDurationMs * 100.0));
            _viewModel.Progress = remainingPercent;

            if (_elapsedMs >= _totalDurationMs)
            {
                _progressTimer.Stop();
                _autoDismissTimer.Stop();
                BeginCloseAnimation();
            }
        }

        private void OnDismissTick(object? sender, EventArgs e)
        {
            // Progress tick handles timing
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            BeginCloseAnimation();
        }

        private void OnStartCodingClicked(object sender, RoutedEventArgs e)
        {
            BeginCloseAnimation();
        }

        public void BeginCloseAnimation()
        {
            if (_isClosing) return;
            _isClosing = true;

            _progressTimer.Stop();
            _autoDismissTimer.Stop();

            var fadeOut = new DoubleAnimation(RootContainer.Opacity, 0.0, TimeSpan.FromMilliseconds(220))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var slideDown = new DoubleAnimation(0.0, 15.0, TimeSpan.FromMilliseconds(220))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOut.Completed += (s, e) =>
            {
                try
                {
                    Close();
                }
                catch
                {
                    // Ignore window already closed
                }
            };

            RootContainer.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            SlideTransform.BeginAnimation(TranslateTransform.YProperty, slideDown);
        }

        private static void PlayGreetingSound()
        {
            try
            {
                SystemSounds.Asterisk.Play();
            }
            catch
            {
                // Ignore audio play errors
            }
        }

        public static void ShowGreeting(GreetingPayload payload)
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
