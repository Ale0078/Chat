using System.Windows;
using System.Windows.Input;

namespace Chat.Client.AttachedProperties
{
    public static class FrameworkElementExtension
    {
        private static readonly DependencyProperty ActualWidthInitializedProperty;

        public static readonly DependencyProperty ActualWidthProperty;
        public static readonly DependencyProperty DoesOpacityChangeVisibilityProperty;
        public static readonly DependencyProperty IsKeyboardFocusSettedByMouseUpProperty;

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
    } 
}