using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Chat.Client.Services.Abstracts;
using Chat.Client.ViewModel.Base;

namespace Chat.Client.Services
{
    public class ScrollController : ScrollControllerBase
    {
        public override void ScrollToEnd()
        {
            _currentScroll?.ScrollToEnd();
        }

        public override void ScrollToFirstUnreadMessage()
        {
            if (_currentScroll is null)
            {
                return;
            }

            StackPanel panel = VisualTreeHelper.GetChild(_currentScroll.Content as DependencyObject, 0) as StackPanel;

            if (panel.Children.Count == 0)
            {
                return;
            }

            double passedHeight = 0;

            foreach (ContentPresenter content in panel.Children)
            {
                MessageViewModelBase message = content.Content as MessageViewModelBase;

                passedHeight += content.ActualHeight;

                if (!message.IsRead && !message.IsFromCurrentUser)
                {
                    break;
                }
            }

            if (passedHeight > _currentScroll.ActualHeight)
            {
                _currentScroll.ScrollToVerticalOffset(passedHeight - _currentScroll.ActualHeight);
            }
        }
    }
}
