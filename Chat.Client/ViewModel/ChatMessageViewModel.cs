using System;
using System.Threading.Tasks;

namespace Chat.Client.ViewModel
{
    public class ChatMessageViewModel : ViewModelBase
    {
        private Guid _id;
        private Guid _chtaId;
        private string _fromUserId;
        private string _toUserId;
        private string _message;
        private bool _isFromCurrentUser;
        private DateTime _sendingTime;

        public Guid Id
        {
            get => _id;
            set
            {
                _id = value;

                OnPropertyChanged();
            }
        }

        public Guid ChatId
        {
            get => _chtaId;
            set
            {
                _chtaId = value;

                OnPropertyChanged();
            }
        }

        public string FromUserId
        {
            get => _fromUserId;
            set
            {
                _fromUserId = value;

                OnPropertyChanged();
            }
        }

        public string ToUserId
        {
            get => _toUserId;
            set
            {
                _toUserId = value;

                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set 
            {
                _message = value;

                OnPropertyChanged();
            }
        }

        public bool IsFromCurrentUser 
        {
            get => _isFromCurrentUser;
            set 
            {
                _isFromCurrentUser = value;

                OnPropertyChanged();
            }
        }

        public DateTime SendingTime 
        {
            get => _sendingTime;
            set 
            {
                _sendingTime = value;

                OnPropertyChanged();
            }
        }
    }
}
