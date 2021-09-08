using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Chat.Models
{
    public class UserModel : INotifyPropertyChanged//ToDo: delete test
    {
        private bool _isLogin;

        public string Id { get; set; }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public Guid ChatId { get; set; }
        public ObservableCollection<ChatMessageModel> Messages { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLogin 
        {
            get => _isLogin;
            set 
            {
                _isLogin = value;

                OnPropertyChanged();
            }
        }

        public UserModel()
        {
            Messages = new ObservableCollection<ChatMessageModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string property = "") 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
