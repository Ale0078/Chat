using System;
using System.Collections.ObjectModel;

using Chat.Models;

namespace Chat.Client.ViewModel
{
    public class ChatMemberViewModel : ViewModelBase
    {
        private string _id;
        private string _name;
        private string _connectionId;
        private Guid _chatId;
        private byte[] _photo;
        private DateTime _disconnectTime;
        private bool _isAdmin;
        private bool _isLogin;
        private bool _isBlocked;
        private bool _isClientBlockedByMember;

        public string Message { get; set; }
        public ObservableCollection<ChatMessageModel> Messages { get; set; }

        public string Id 
        {
            get => _id;
            set 
            {
                _id = value;

                OnPropertyChanged();
            }
        }

        public string Name 
        {
            get => _name;
            set 
            {
                _name = value;

                OnPropertyChanged();
            }
        }

        public string ConnectionId 
        {
            get => _connectionId;
            set 
            {
                _connectionId = value;

                OnPropertyChanged();
            }
        }

        public Guid ChatId 
        {
            get => _chatId;
            set 
            {
                _chatId = value;

                OnPropertyChanged();
            }
        }

        public byte[] Photo 
        {
            get => _photo;
            set 
            {
                _photo = value;

                OnPropertyChanged();
            }
        }

        public DateTime DisconnectTime 
        {
            get => _disconnectTime;
            set 
            {
                _disconnectTime = value;

                OnPropertyChanged();
            }
        }

        public bool IsAdmin 
        {
            get => _isAdmin;
            set 
            {
                _isAdmin = value;

                OnPropertyChanged();
            }
        }

        public bool IsLogin
        {
            get => _isLogin;
            set 
            {
                _isLogin = value;

                OnPropertyChanged();
            }
        }

        public bool IsBlocked 
        {
            get => _isBlocked;
            set 
            {
                _isBlocked = value;

                OnPropertyChanged();
            }
        }

        public bool IsClientBlockedByMember 
        {
            get => _isClientBlockedByMember;
            set 
            {
                _isClientBlockedByMember = value;

                OnPropertyChanged();
            }
        }
    }
}
