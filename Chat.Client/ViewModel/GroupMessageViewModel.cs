using System;

using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class GroupMessageViewModel : MessageViewModelBase
    {
        private Guid _groupId;

        public Guid GroupId 
        {
            get => _groupId;
            set 
            {
                _groupId = value;

                OnPropertyChanged();
            }
        }
    }
}
