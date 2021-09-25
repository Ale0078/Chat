using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Chat.Controls
{
    public class TimeBlock : TextBlock
    {
        private readonly DispatcherTimer _timer;

        public static readonly DependencyProperty TimeToCountdownProperty;
        public static readonly DependencyProperty TimerIntervalProperty;
        public static readonly DependencyProperty DoesStartTimerProperty;

        static TimeBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeBlock), new FrameworkPropertyMetadata(typeof(TimeBlock)));

            TimeToCountdownProperty = DependencyProperty.Register(
                name: nameof(TimeToCountdown),
                propertyType: typeof(DateTime),
                ownerType: typeof(TimeBlock));

            TimerIntervalProperty = DependencyProperty.Register(
                name: nameof(TimerInterval),
                propertyType: typeof(TimeSpan),
                ownerType: typeof(TimeBlock),
                typeMetadata: new PropertyMetadata(
                    propertyChangedCallback: ChangeTimeInterval));

            DoesStartTimerProperty = DependencyProperty.Register(
                name: nameof(DoesStartTimer),
                propertyType: typeof(bool),
                ownerType: typeof(TimeBlock),
                typeMetadata: new PropertyMetadata(
                    propertyChangedCallback: ChangeDoesStartTimer));
        }

        public TimeBlock()
        {
            _timer = new DispatcherTimer();

            _timer.Tick += OnTick;
        }

        public DateTime TimeToCountdown
        {
            get => (DateTime)GetValue(TimeToCountdownProperty);
            set => SetValue(TimeToCountdownProperty, value);
        }

        public TimeSpan TimerInterval 
        {
            get => (TimeSpan)GetValue(TimerIntervalProperty);
            set => SetValue(TimerIntervalProperty, value);
        }

        public bool DoesStartTimer 
        {
            get => (bool)GetValue(DoesStartTimerProperty);
            set => SetValue(DoesStartTimerProperty, value);
        }

        private static void ChangeTimeInterval(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            TimeBlock time = d as TimeBlock;

            if (time is null)
            {
                return;
            }

            time._timer.Interval = (TimeSpan)e.NewValue;
        }

        private static void ChangeDoesStartTimer(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            TimeBlock time = d as TimeBlock;

            if (time is null)
            {
                return;
            }

            bool doesStartTimer = (bool)e.NewValue;

            if (doesStartTimer)
            {
                time._timer.Start();
            }
            else
            {
                time._timer.Stop();
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            TimeSpan desconnectedTime = DateTime.Now - TimeToCountdown;

            Text = desconnectedTime switch
            {
                { Days: 0, Hours: 0, Minutes: 0 } => "Last seen resently",
                { Days: 0, Hours: 0, Minutes: not 0 } => $"Last seen {desconnectedTime.Minutes} minets ago",
                { Days: 0, Hours: not 0 } => $"Last seen {desconnectedTime.Hours} hours ago",
                { Days: not 0 } => $"Last seen {desconnectedTime.Days} days ago"
            };
        }
    }
}
