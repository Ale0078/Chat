using System;
using System.Windows.Input;

using Chat.Client.Commands;
using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class ChatMessageViewModel : MessageViewModelBase
    {
        private Guid _chtaId;
        private string _toUserId;

        private ICommand _editMessage;

        public Guid ChatId
        {
            get => _chtaId;
            set
            {
                _chtaId = value;

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

        public ICommand EditMessage => _editMessage ?? (_editMessage = new RelayCommand(
            execute: ExecuteEditMessage));

        private void ExecuteEditMessage(object isEditing) 
        {
            IsEditing = (bool)isEditing;
        }
    }
}
