using System;

using Chat.Client.ViewModel.Base;

namespace Chat.Client.ViewModel
{
    public class GroupUserViewModel : UserViewModelBase
    {
        private string _id;
        private string _connectionId;
        private DateTime _disconnectTime;


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

        public DateTime DisconnectTime 
        {
            get => _disconnectTime;
            set 
            {
                _disconnectTime = value;

                OnPropertyChanged();
            }
        }
    }
}
