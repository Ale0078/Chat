using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;

using Chat.Client.Services;
using Chat.Client.Commands;
using Chat.Models;

namespace Chat.Client.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const int MIN_USER_NAME_LENGTH = 3;
        private string _tokenName = "Alex"; //ToDo: delete test

        private ChatService _service;

        private string _userName;
        private bool _isLogin;
        private UserState _state;
        private UserModel _selectedUser;
        private ObservableCollection<UserModel> _otherUsers;
        
        private ICommand _login;
        private ICommand _logout;
        private ICommand _reciveMessage;
        private ICommand _connect;

        public MainWindowViewModel()// ToDo: delete test
        {
            //_service = new ChatService();
            _otherUsers = new ObservableCollection<UserModel>();

            _state = UserState.NoRegistered;
            //_service.Login += LoginEvenHandler;
            //_service.Logout += LogoutEventHandler;
            //_service.ReciveMessage += ReciveMessageEventHandler;
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

        public UserModel SelectedUser 
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

        public UserState State 
        {
            get => _state;
            set 
            {
                _state = value;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserModel> OtherUsers
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

        private async Task ConnectExecute(object parameter) //ToDo: delete test
        {
            HubConnection tokenConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chat_register")
                .Build();

            await tokenConnection.StartAsync();



            _service = new ChatService(await tokenConnection.InvokeAsync<string>("Login", _tokenName, null));


            _service.Login += LoginEvenHandler;
            _service.Logout += LogoutEventHandler;
            _service.ReciveMessage += ReciveMessageEventHandler;

            await _service.Connect();

            await tokenConnection.DisposeAsync();

        }


        private async Task LoginExecute(object userName) 
        {
            var users = await _service.LoginUser((string)userName);

            if (users is not null)
            {
                foreach (UserModel user in users)
                {
                    UserModel addedUser = new()
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
            await _service.LogoutUser(UserName);
        }

        private async Task ReciveMessageExecute(object parametr) 
        {
            if (SelectedUser is null)
            {
                return;
            }

            ChatMessageModel sendedMessage = await _service.ReciveMessageUser(
                fromUserName: UserName,
                toUserId: SelectedUser.Id,
                message: (string)parametr);

            sendedMessage.IsFromCurrentUser = true;

            SelectedUser.Messages.Add(sendedMessage);
        }

        private void LoginEvenHandler(UserModel newUser) 
        {
            UserModel addedUser = OtherUsers
                .Where(user => user.Name == newUser.Name)
                .FirstOrDefault();

            if (addedUser is not null)
            {
                return;
            }

            addedUser = new UserModel
            {
                Id = newUser.Id,
                IsAdmin = newUser.IsAdmin,
                IsLogin = newUser.IsLogin,
                Name = newUser.Name
            };

            OtherUsers.Add(addedUser);
        }

        private void LogoutEventHandler(UserModel oldUser) 
        {
            UserModel removedUser = OtherUsers
                .Where(user => user.Name == oldUser.Name)
                .FirstOrDefault();

            if (removedUser is null)
            {
                return;
            }

            OtherUsers.Remove(removedUser);
        }

        private void ReciveMessageEventHandler(ChatMessageModel message) 
        {
            UserModel sender = OtherUsers
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
