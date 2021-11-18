using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Chat.Client.ViewModel;
using Chat.Client.ViewModel.Base;

namespace Chat.Client.AttachedProperties
{
    public static class ScrollViewerExtension
    {
        public static readonly DependencyProperty IsSrollSetterToScrollControllerProperty;
        public static readonly DependencyProperty IsCheckUnreadMessagesProperty;

        static ScrollViewerExtension()
        {
            IsSrollSetterToScrollControllerProperty = DependencyProperty.RegisterAttached(
                name: "IsSrollSetterToScrollController",
                propertyType: typeof(bool),
                ownerType: typeof(ScrollViewerExtension),
                defaultMetadata: new PropertyMetadata(false, IsSrollSetterToScrollControllerChanged));

            IsCheckUnreadMessagesProperty = DependencyProperty.RegisterAttached(
                name: "IsCheckUnreadMessages",
                propertyType: typeof(bool),
                ownerType: typeof(ScrollViewerExtension),
                defaultMetadata: new PropertyMetadata(false, IsCheckUnreadMessagesChanged));
        }

        public static bool GetIsSrollSetterToScrollController(DependencyObject obj) =>
            (bool)obj.GetValue(IsSrollSetterToScrollControllerProperty);

        public static void SetIsSrollSetterToScrollController(DependencyObject obj, bool value) =>
            obj.SetValue(IsSrollSetterToScrollControllerProperty, value);

        public static bool GetIsCheckUnread(DependencyObject obj) =>
            (bool)obj.GetValue(IsCheckUnreadMessagesProperty);

        public static void SetIsCheckUnreadMessages(DependencyObject obj, bool value) =>
            obj.SetValue(IsCheckUnreadMessagesProperty, value);

        private static void IsSrollSetterToScrollControllerChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scroll = sender as ScrollViewer;

            ChatViewModel chat = scroll.DataContext as ChatViewModel;

            if (scroll is null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                chat?.ScrollController.SetScrollViewer(scroll);
            }
            else 
            {
                chat?.ScrollController.SetScrollViewer(null);
            }
        }

        private static void IsCheckUnreadMessagesChanged(object sender, DependencyPropertyChangedEventArgs e) 
        {
            ScrollViewer scroll = sender as ScrollViewer;

            if (scroll is null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                scroll.ScrollChanged += OnScrollChanged;
            }
            else 
            {
                scroll.ScrollChanged -= OnScrollChanged;
            }
            
        }

        private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scroll = sender as ScrollViewer;

            Panel panel = VisualTreeHelper.GetChild(scroll.Content as DependencyObject, 0) as Panel;

            if (panel.Children.Count != 0)
            {
                CheckAllVisibliMessagesFromPanel(
                    panelToGetChilds: panel,
                    startHeight: e.VerticalOffset,
                    endHeight: e.VerticalOffset + e.ViewportHeight);
            }
        }

        private static void CheckAllVisibliMessagesFromPanel(Panel panelToGetChilds, double startHeight, double endHeight) 
        {
            double passedHeight = 0;

            foreach (ContentPresenter content in panelToGetChilds.Children) 
            {
                passedHeight += content.ActualHeight;

                if (passedHeight < startHeight)
                {
                    continue;
                }

                MessageViewModelBase message = content.Content as MessageViewModelBase;

                if (passedHeight - content.ActualHeight >= startHeight
                    && passedHeight <= endHeight
                    && !message.IsFromCurrentUser
                    && !message.IsRead)
                {
                    message.IsRead = true;
                }

                if (passedHeight > endHeight)
                {
                    break;
                }
            }
        }
    }
}
