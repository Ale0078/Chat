using System;

using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class GroupMessageViewModel : MessageViewModelBase
    {
        private Guid _groupId;
        private UserViewModelBase _lastMessageSender;
        private bool _isItLastMessageFromSender;

        public Guid GroupId 
        {
            get => _groupId;
            set 
            {
                _groupId = value;

                OnPropertyChanged();
            }
        }

        public UserViewModelBase LastMessageSender 
        {
            get => _lastMessageSender;
            set 
            {
                _lastMessageSender = value;

                OnPropertyChanged();
            }
        }

        public bool IsItLastMessageFromSender 
        {
            get => _isItLastMessageFromSender;
            set 
            {
                _isItLastMessageFromSender = value;

                OnPropertyChanged();
            }
        }
    }
}
