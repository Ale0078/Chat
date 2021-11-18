using System.Windows.Controls;

using Chat.Client.Services.Interfaces;

namespace Chat.Client.Services.Abstracts
{
    public abstract class ScrollControllerBase : IScrollController
    {
        protected ScrollViewer _currentScroll;

        public virtual void SetScrollViewer(ScrollViewer currentScroll) 
        {
            _currentScroll = currentScroll;
        }

        public virtual void SetVerticalOffset(double offset) 
        {
            _currentScroll?.ScrollToVerticalOffset(offset);
        }

        public virtual double GetVerticalOffset() 
        {
            return _currentScroll.VerticalOffset;
        }

        public virtual bool IsScrollSetted() 
        {
            return _currentScroll is not null;
        }

        public virtual bool IsScrollToEnd() 
        {
            return _currentScroll?.VerticalOffset + _currentScroll?.ViewportHeight == _currentScroll?.ExtentHeight;
        }

        public abstract void ScrollToEnd();

        public abstract void ScrollToFirstUnreadMessage();
    }
}
