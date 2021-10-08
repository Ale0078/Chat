using System.Windows;
using System.Windows.Controls;

namespace Chat.Client.AttachedProperties
{
    public static class ScrollViewerExtension
    {
        public static readonly DependencyProperty IsScrollToEndProperty;

        static ScrollViewerExtension()
        {
            IsScrollToEndProperty = DependencyProperty.RegisterAttached(
                name: "IsScrollToEnd",
                propertyType: typeof(bool),
                ownerType: typeof(ScrollViewerExtension),
                defaultMetadata: new PropertyMetadata(false, IsSrollToEndChanged));
        }

        public static bool GetIsScrollToEnd(DependencyObject obj) =>
            (bool)obj.GetValue(IsScrollToEndProperty);

        public static void SetIsScrollToEnd(DependencyObject obj, bool value) =>
            obj.SetValue(IsScrollToEndProperty, value);

        private static void IsSrollToEndChanged(object sender, DependencyPropertyChangedEventArgs e) 
        {
            ScrollViewer scroll = sender as ScrollViewer;

            if (scroll is null)
            {
                return;
            }

            if (e.NewValue is not null && (bool)e.NewValue)
            {
                scroll.ScrollToEnd();
            }
        }
    }
}
