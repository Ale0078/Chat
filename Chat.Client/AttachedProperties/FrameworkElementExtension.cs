using System.Windows;
using System.Windows.Controls;

namespace Chat.Client.AttachedProperties
{
    public static class FrameworkElementExtension
    {
        private static readonly DependencyProperty ActualWidthInitializedProperty;

        public static readonly DependencyProperty ActualWidthProperty;

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

        private static void OnActualWidthChanged(object sender, RoutedEventArgs e) =>
            SetActualWidth((FrameworkElement)sender, ((FrameworkElement)sender).ActualWidth);

        public static double GetActualWidth(DependencyObject obj) =>
            (double)obj.GetValue(ActualWidthProperty);

        public static void SetActualWidth(DependencyObject obj, double value) =>
            obj.SetValue(ActualWidthProperty, value);
    } 
}