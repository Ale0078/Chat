using System;
using System.Windows.Input;

using Chat.Client.Commands;

namespace Chat.Client.ViewModel.Base
{
    public abstract class MessageViewModelBase : ViewModelBase
    {
        private Guid _id;
        private string _fromUserId;
        private string _message;
        private byte[] _byteFile;
        private bool _isFromCurrentUser;
        private DateTime _sendingTime;
        private bool _isEditing;
        private bool _isEdit;
        private bool _isRead;

        private ICommand _editMessage;

        public Guid Id
        {
            get => _id;
            set
            {
                _id = value;

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

        public string Message
        {
            get => _message;
            set
            {
                _message = value;

                OnPropertyChanged();
            }
        }

        public byte[] ByteFile
        {
            get => _byteFile;
            set
            {
                _byteFile = value;

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

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;

                OnPropertyChanged();
            }
        }

        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                _isEdit = value;

                OnPropertyChanged();
            }
        }

        public bool IsRead 
        {
            get => _isRead;
            set 
            {
                _isRead = value;

                OnPropertyChanged();
            }
        }

        public ICommand EditMessage => _editMessage ?? (_editMessage = new RelayCommand(
            execute: ExecuteEditMessage));

        private void ExecuteEditMessage(object isEditing)
        {
            IsEditing = (bool)isEditing;
        }
    }
}
