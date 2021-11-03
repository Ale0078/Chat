using System;

using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class ChatMessageViewModel : MessageViewModelBase
    {
        private Guid _chtaId;
        private string _toUserId;

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
    }
}
