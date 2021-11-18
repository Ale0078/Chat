using System.Windows.Controls;

namespace Chat.Client.Services.Interfaces
{
    public interface IScrollController
    {
        void SetScrollViewer(ScrollViewer currentScroll);

        void ScrollToEnd();

        void ScrollToFirstUnreadMessage();

        void SetVerticalOffset(double offset);

        double GetVerticalOffset();

        bool IsScrollSetted();

        bool IsScrollToEnd();
    }
}