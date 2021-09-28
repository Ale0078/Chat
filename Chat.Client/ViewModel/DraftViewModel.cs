using System;

namespace Chat.Client.ViewModel
{
    public class DraftViewModel : ViewModelBase
    {
        private string _message;
        private DateTime _startTypingTime;

        public string Message 
        {
            get => _message;
            set 
            {
                _message = value;

                OnPropertyChanged();
            }
        }

        public DateTime StartTypingTime 
        {
            get => _startTypingTime;
            set 
            {
                _startTypingTime = value;

                OnPropertyChanged();
            }
        }
    }
}
