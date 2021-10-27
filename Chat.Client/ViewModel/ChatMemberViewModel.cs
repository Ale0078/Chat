using System;

using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class ChatMemberViewModel : MemberViewModelBase
    {
        private string _id;
        private string _connectionId;
        private Guid _chatId;
        private DateTime _disconnectTime;
        private bool _isAdmin;
        private bool _isLogin;
        private bool _isBlocked;
        private bool _isMuted;
        private bool _isClientBlockedByMember;
        private bool _isTyping;
        private bool _isSelectedToAddToNewGruop;

        public ChatMemberViewModel() : base()
        { }

        public string Id 
        {
            get => _id;
            set 
            {
                _id = value;

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

        public bool IsMuted 
        {
            get => _isMuted;
            set 
            {
                _isMuted = value;

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

        public bool IsTyping 
        {
            get => _isTyping;
            set 
            {
                _isTyping = value;

                OnPropertyChanged();
            }
        }

        public bool IsSelectedToAddToNewGruop 
        {
            get => _isSelectedToAddToNewGruop;
            set 
            {
                _isSelectedToAddToNewGruop = value;

                OnPropertyChanged();
            }
        }
    }
}
