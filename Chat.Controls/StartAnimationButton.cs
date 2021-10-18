using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Chat.Controls
{
    public class StartAnimationButton : Button
    {
        private readonly Storyboard _animation;

        public static readonly DependencyProperty TypeControlNameScopeProperty;
        public static readonly DependencyProperty TimeLineAnimationProperty;

        public StartAnimationButton()
        {
            _animation = new Storyboard();
        }

        static StartAnimationButton() 
        {
            TypeControlNameScopeProperty = DependencyProperty.Register(
                name: nameof(TypeControlNameScope),
                propertyType: typeof(Type),
                ownerType: typeof(StartAnimationButton));

            TimeLineAnimationProperty = DependencyProperty.Register(
                name: nameof(TimeLineAnimation),
                propertyType: typeof(Timeline),
                ownerType: typeof(StartAnimationButton),
                typeMetadata: new PropertyMetadata(
                    propertyChangedCallback: OnTimelineAnimationChanged));
        }

        public Type TypeControlNameScope
        {
            get => (Type)GetValue(TypeControlNameScopeProperty);
            set => SetValue(TypeControlNameScopeProperty, value);
        }

        public Timeline TimeLineAnimation 
        {
            get => (Timeline)GetValue(TimeLineAnimationProperty);
            set => SetValue(TimeLineAnimationProperty, value);
        }

        protected override void OnClick()
        {
            base.OnClick();

            DependencyObject window = Parent;

            while (window.GetType().FullName != TypeControlNameScope.FullName)
            {
                window = VisualTreeHelper.GetParent(window);
            }

            _animation.Begin(window as FrameworkElement);
        }

        private static void OnTimelineAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            StartAnimationButton animationButton = d as StartAnimationButton;

            animationButton._animation.Children.Remove((Timeline)e.OldValue);
            animationButton._animation.Children.Add((Timeline)e.NewValue);
        }
    }
}
