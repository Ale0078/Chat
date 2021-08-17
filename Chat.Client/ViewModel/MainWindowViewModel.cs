using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Chat.Client.Services;
using Chat.Client.Commands;
using Chat.Models;

namespace Chat.Client.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const int MIN_USER_NAME_LENGTH = 3;

        private readonly ChatService _service;

        private string _userName;
        private bool _isLogin;
        private User _selectedUser;
        private ObservableCollection<User> _otherUsers;
        
        private ICommand _login;
        private ICommand _logout;
        private ICommand _reciveMessage;
        private ICommand _connect;

        public MainWindowViewModel()
        {
            _service = new ChatService();
            _otherUsers = new ObservableCollection<User>();

            _service.Login += LoginEvenHandler;
            _service.Logout += LogoutEventHandler;
            _service.ReciveMessage += ReciveMessageEventHandler;
        }

        public string UserName 
        {
            get => _userName;
            set 
            {
                _userName = value;

                OnPropertyChanged();
            }
        }

        public User SelectedUser 
        {
            get => _selectedUser;
            set 
            {
                _selectedUser = value;

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

        public ObservableCollection<User> OtherUsers
        {
            get => _otherUsers;
            set
            {
                _otherUsers = value;

                OnPropertyChanged();
            }
        }

        public ICommand Connect => _connect ?? (_connect = new RelayCommandAsync(ConnectExecute));

        public ICommand Login => _login ?? (_login = new RelayCommandAsync(
            execute: LoginExecute,
            canExecute: LoginCanExecute));

        public ICommand Logout => _logout ?? (_logout = new RelayCommandAsync(LogoutExecute));

        public ICommand ReciveMessage => _reciveMessage ?? (_reciveMessage = new RelayCommandAsync(ReciveMessageExecute));

        private async Task ConnectExecute(object parameter) =>
            await _service.Connect();

        private async Task LoginExecute(object userName) 
        {
            var users = await _service.UserLogin((string)userName);

            if (users is not null)
            {
                foreach (User user in users)
                {
                    User addedUser = new()
                    {
                        Id = user.Id,
                        IsAdmin = user.IsAdmin,
                        IsLogin = user.IsLogin,
                        Name = user.Name
                    };

                    OtherUsers.Add(addedUser);
                }

                UserName = (string)userName;
                IsLogin = true;
            }
        }

        private bool LoginCanExecute(object parametr) =>
            !string.IsNullOrEmpty((string)parametr) && ((string)parametr).Length > MIN_USER_NAME_LENGTH;

        private async Task LogoutExecute(object parametr) 
        {
            await _service.UserLogout(UserName);
        }

        private async Task ReciveMessageExecute(object parametr) 
        {
            if (SelectedUser is null)
            {
                return;
            }

            ChatMessage sendedMessage = await _service.UserReciveMessage(
                fromUserName: UserName,
                toUserId: SelectedUser.Id,
                message: (string)parametr);

            sendedMessage.IsFromCurrentUser = true;

            SelectedUser.Messages.Add(sendedMessage);
        }

        private void LoginEvenHandler(User newUser) 
        {
            User addedUser = OtherUsers
                .Where(user => user.Name == newUser.Name)
                .FirstOrDefault();

            if (addedUser is not null)
            {
                return;
            }

            addedUser = new User
            {
                Id = newUser.Id,
                IsAdmin = newUser.IsAdmin,
                IsLogin = newUser.IsLogin,
                Name = newUser.Name
            };

            OtherUsers.Add(addedUser);
        }

        private void LogoutEventHandler(User oldUser) 
        {
            User removedUser = OtherUsers
                .Where(user => user.Name == oldUser.Name)
                .FirstOrDefault();

            if (removedUser is null)
            {
                return;
            }

            OtherUsers.Remove(removedUser);
        }

        private void ReciveMessageEventHandler(ChatMessage message) 
        {
            User sender = OtherUsers
                .Where(user => user.Id == message.FromUserId)
                .FirstOrDefault();

            if (sender is null)
            {
                return;
            }

            sender.Messages.Add(message);
        }
    }
}
