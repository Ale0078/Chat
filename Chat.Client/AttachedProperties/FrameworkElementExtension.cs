using System.Windows;
using System.Windows.Input;

using Chat.Client.ViewDatas;

namespace Chat.Client.AttachedProperties
{
    public static class FrameworkElementExtension
    {
        private static readonly DependencyProperty ActualWidthInitializedProperty;

        public static readonly DependencyProperty ActualWidthProperty;
        public static readonly DependencyProperty DoesOpacityChangeVisibilityProperty;
        public static readonly DependencyProperty IsKeyboardFocusSettedByMouseUpProperty;
        public static readonly DependencyProperty AnimationStoryboardProperty;
        public static readonly DependencyProperty ControlToFindParentToAnimationStoryboardProperty;
        public static readonly DependencyProperty DoesAnimationStartToAnimationStoryboardProperty;

        static FrameworkElementExtension()
        {
            ActualWidthInitializedProperty = DependencyProperty.RegisterAttached(
                name: "ActualWidthInitialized",
                propertyType: typeof(bool),
                ownerType: typeof(FrameworkElementExtension),
                defaultMetadata: new FrameworkPropertyMetadata(false));

            ActualWidthProperty = DependencyProperty.RegisterAttached(
                name: "ActualWidth",
                propertyType: typeof(double),
                ownerType: typeof(FrameworkElementExtension),
                defaultMetadata: new FrameworkPropertyMetadata(
                    defaultValue: double.NaN,
                    propertyChangedCallback: OnActualWidthChangedCallback));

            DoesOpacityChangeVisibilityProperty = DependencyProperty.RegisterAttached(
                name: "DoesOpacityChangeVisibility",
                propertyType: typeof(bool),
                ownerType: typeof(FrameworkElementExtension),
                defaultMetadata: new FrameworkPropertyMetadata(
                    defaultValue: false,
                    propertyChangedCallback: OnDoesOpacityChangeVisibilityChangedCallback));

            IsKeyboardFocusSettedByMouseUpProperty = DependencyProperty.RegisterAttached(
                name: "IsKeyboardFocusSettedByMouseUp",
                propertyType: typeof(bool),
                ownerType: typeof(FrameworkElementExtension),
                defaultMetadata: new FrameworkPropertyMetadata(
                    propertyChangedCallback: OnIsKeyboardFocusSettedByMouseUpChangedCallback));

            AnimationStoryboardProperty = DependencyProperty.RegisterAttached(
                name: "AnimationStoryboard",
                propertyType: typeof(AnimationSource),
                ownerType: typeof(FrameworkElementExtension),
                defaultMetadata: new FrameworkPropertyMetadata(
                    propertyChangedCallback: OnAnimationStoryboardPropertyChanged));

            ControlToFindParentToAnimationStoryboardProperty = DependencyProperty.RegisterAttached(
                name: "ControlToFindParentToAnimationStoryboard",
                propertyType: typeof(FrameworkElement),
                ownerType: typeof(FrameworkElementExtension),
                defaultMetadata: new FrameworkPropertyMetadata(
                    propertyChangedCallback: OnControlToFindParentToAnimationStoryboardChanged));

            DoesAnimationStartToAnimationStoryboardProperty = DependencyProperty.RegisterAttached(
                name: "DoesAnimationStartToAnimationStoryboard",
                propertyType: typeof(bool),
                ownerType: typeof(FrameworkElementExtension),
                defaultMetadata: new FrameworkPropertyMetadata(
                    propertyChangedCallback: OnDoesAnimationStartToAnimationStoryboardChanged));
        }

        private static void OnActualWidthChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            FrameworkElement control = d as FrameworkElement;

            if (control is null)
            {
                return;
            }

            if (!(bool)control.GetValue(ActualWidthInitializedProperty))
            {
                control.SetValue(ActualWidthInitializedProperty, true);

                control.SizeChanged += OnActualWidthChanged;
            }
        }

        private static void OnDoesOpacityChangeVisibilityChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            FrameworkElement control = d as FrameworkElement;

            if (control is null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                FrameworkElement.OpacityProperty.OverrideMetadata(
                    forType: control.GetType(),
                    typeMetadata: new FrameworkPropertyMetadata(
                        propertyChangedCallback: OnOpacityChanged));
            }
            else 
            {
                FrameworkElement.OpacityProperty.OverrideMetadata(
                    forType: control.GetType(),
                    typeMetadata: FrameworkElement.OpacityProperty.DefaultMetadata);
            }
        }

        private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            FrameworkElement control = d as FrameworkElement;

            if (control is null)
            {
                return;
            }

            if ((double)e.NewValue <= 0)
            {
                control.Visibility = Visibility.Collapsed;
            }
            else 
            {
                control.Visibility = Visibility.Visible;
            }
        }

        private static void OnIsKeyboardFocusSettedByMouseUpChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            FrameworkElement control = d as FrameworkElement;

            if (control is null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                control.MouseUp -= SetKeyboardFocus;
                control.MouseUp += SetKeyboardFocus;
            }
            else 
            {
                control.MouseUp -= SetKeyboardFocus;
            }
        }

        private static void SetKeyboardFocus(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(sender as IInputElement);
        }

        private static void OnAnimationStoryboardPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            FrameworkElement control = d as FrameworkElement;

            if (control is null)
            {
                return;
            }

            if (GetControlToFindParentToAnimationStoryboard(control) is not null)
            {
                ((AnimationSource)e.NewValue).ControlToFindParent = GetControlToFindParentToAnimationStoryboard(control);
            }

            ((AnimationSource)e.NewValue).DoesAnimationStart = GetDoesAnimationStartToAnimationStoryboard(control);
        }

        private static void OnControlToFindParentToAnimationStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            FrameworkElement control = d as FrameworkElement;

            if (control is null || GetAnimationStoryboard(control) is null)
            {
                return;
            }

            GetAnimationStoryboard(control).ControlToFindParent = e.NewValue as FrameworkElement;
        }

        private static void OnDoesAnimationStartToAnimationStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            FrameworkElement control = d as FrameworkElement;

            if (control is null || GetAnimationStoryboard(control) is null)
            {
                return;
            }

            GetAnimationStoryboard(control).DoesAnimationStart = (bool)e.NewValue;
        }

        private static void OnActualWidthChanged(object sender, RoutedEventArgs e) =>
            SetActualWidth((FrameworkElement)sender, ((FrameworkElement)sender).ActualWidth);

        public static double GetActualWidth(DependencyObject obj) =>
            (double)obj.GetValue(ActualWidthProperty);

        public static void SetActualWidth(DependencyObject obj, double value) =>
            obj.SetValue(ActualWidthProperty, value);

        public static bool GetDoesOpacityChangeVisibility(DependencyObject obj) =>
            (bool)obj.GetValue(DoesOpacityChangeVisibilityProperty);

        public static void SetDoesOpacityChangeVisibility(DependencyObject obj, bool value) =>
            obj.SetValue(DoesOpacityChangeVisibilityProperty, value);

        public static bool GetIsKeyboardFocusSettedByMouseUp(DependencyObject obj) =>
            (bool)obj.GetValue(IsKeyboardFocusSettedByMouseUpProperty);

        public static void SetIsKeyboardFocusSettedByMouseUp(DependencyObject obj, bool value) =>
            obj.SetValue(IsKeyboardFocusSettedByMouseUpProperty, value);

        public static AnimationSource GetAnimationStoryboard(DependencyObject obj) =>
            (AnimationSource)obj.GetValue(AnimationStoryboardProperty);

        public static void SetAnimationStoryboard(DependencyObject obj, AnimationSource value) =>
            obj.SetValue(AnimationStoryboardProperty, value);

        public static FrameworkElement GetControlToFindParentToAnimationStoryboard(DependencyObject obj) =>
            (FrameworkElement)obj.GetValue(ControlToFindParentToAnimationStoryboardProperty);

        public static void SetControlToFindParentToAnimationStoryboard(DependencyObject obj, FrameworkElement value) =>
            obj.SetValue(ControlToFindParentToAnimationStoryboardProperty, value);

        public static bool GetDoesAnimationStartToAnimationStoryboard(DependencyObject obj) =>
            (bool)obj.GetValue(DoesAnimationStartToAnimationStoryboardProperty);

        public static void SetDoesAnimationStartToAnimationStoryboard(DependencyObject obj, bool value) =>
            obj.SetValue(DoesAnimationStartToAnimationStoryboardProperty, value);
    } 
}