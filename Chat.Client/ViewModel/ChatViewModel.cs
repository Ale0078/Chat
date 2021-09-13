using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

using Chat.Client.Services;
using Chat.Client.Commands;
using Chat.Models;

namespace Chat.Client.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly ChatService _chatService;
        private readonly RegistrationChatService _registrationChatService;

        private string _id;
        private string _userName;
        private string _message;
        private Visibility _buttonToSendVisibility;
        private UserModel _currentUser;
        private ObservableCollection<UserModel> _users;

        private ICommand _connect;
        private ICommand _sendMessage;

        public ChatViewModel(ChatService chatService, RegistrationChatService registrationChatService)
        {
            _chatService = chatService;
            _registrationChatService = registrationChatService;

            _buttonToSendVisibility = Visibility.Collapsed;

            SetEvents();
        }

        public string Id 
        {
            get => _id;
            set 
            {
                _id = value;

                OnPropertyChanged();
            }
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

        public string Message 
        {
            get => _message;
            set 
            {
                _message = value;

                if (_message == string.Empty
                    || string.IsNullOrEmpty(_message)
                    || string.IsNullOrWhiteSpace(_message))
                {
                    ButtonToSendVisibility = Visibility.Collapsed;
                }
                else
                {
                    ButtonToSendVisibility = Visibility.Visible;
                }

                OnPropertyChanged();
            }
        }

        public Visibility ButtonToSendVisibility 
        {
            get => _buttonToSendVisibility;
            set 
            {
                _buttonToSendVisibility = value;

                OnPropertyChanged();
            }
        }

        public UserModel CurrentUser 
        {
            get => _currentUser;
            set 
            {
                _currentUser = value;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserModel> Users 
        {
            get => _users;
            set 
            {
                _users = value;

                OnPropertyChanged();
            }
        }

        public ICommand Connect => _connect ?? (_connect = new RelayCommandAsync(
            execute: ExecuteConnectEvent));

        private async Task ExecuteConnectEvent(object parametr) 
        {
            await _chatService.Connect();
        }

        public ICommand SendMessage => _sendMessage ?? (_sendMessage = new RelayCommandAsync(
            execute: ExecuteSendMessage,
            canExecute: CanExecuteSendMessage));

        private async Task ExecuteSendMessage(object parametr) 
        {
            ChatMessageModel sendingMessage = await _chatService.ReciveMessageUser(
                chatId: CurrentUser.ChatId,
                fromUserId: Id,
                toUserId: CurrentUser.Id,
                message: Message.TrimStart().TrimEnd(),
                connectionId: Users.First(user => user.Name == CurrentUser.Name).ConnectionId);

            Message = string.Empty;

            sendingMessage.IsFromCurrentUser = true;

            CurrentUser.Messages.Add(sendingMessage);
        }

        private bool CanExecuteSendMessage(object parametr) 
        {
            return !(string.IsNullOrWhiteSpace(Message) || string.IsNullOrEmpty(Message));
        }

        private void SetEvents()
        {
            _chatService.ConnectUser += ConnectUserEventHandler;
            _chatService.Logout += LogoutEventHandler;
            _chatService.ReciveMessage += ReciveMessageEventHandler;
            _chatService.SendConnectionsIdToCallerEvent += SendConnectionsIdToCallerEventHandler;

            _registrationChatService.RegisterUserToOthersServerHandler += RegisterUserToOthersServerEventHandler;
            _registrationChatService.SendUsersToCallerServerHandler += SendUsersToCallerServerEventHandler;
            _registrationChatService.SendUserToCallerServerHandler += SendUserToCallerServerEventHandler;
        }

        private void ConnectUserEventHandler(string userName, string connectionId) 
        {
            UserModel connectedUser = Users.First(user =>
            {
                return user.Name == userName;
            });

            connectedUser.IsLogin = true;
            connectedUser.ConnectionId = connectionId;
        }

        private void LogoutEventHandler(string userName) 
        {
            UserModel logoutedUser = Users.First(user =>
            {
                return user.Name == userName;
            });

            logoutedUser.ConnectionId = string.Empty;
            logoutedUser.IsLogin = false;
        }

        private void ReciveMessageEventHandler(ChatMessageModel message) 
        {
            UserModel userSender = Users.First(user =>
            {
                return user.ChatId.Equals(message.ChatId);
            });

            userSender.Messages.Add(message);
        }

        private void SendConnectionsIdToCallerEventHandler(IEnumerable<UserConnection> connections) 
        {
            foreach (UserConnection connection in connections)
            {
                UserModel user = Users.First(model =>
                {
                    return model.Name == connection.UserName;
                });

                user.ConnectionId = connection.ConnectionId;
                user.IsLogin = true;
            }
        }

        private void RegisterUserToOthersServerEventHandler(FullUserModel newUser)
        {
            ChatModel chat = newUser.Chats.Find(chatModel =>
            {
                return chatModel.FirstUserId.Equals(Id) || chatModel.SecondUserId.Equals(Id);
            });

            ObservableCollection<ChatMessageModel> messages = new ObservableCollection<ChatMessageModel>(chat.Messages);

            Users.Add(new UserModel
            {
                Id = newUser.Id,
                ChatId = chat.Id,
                Messages = messages,
                Name = newUser.Name
            });
        }

        private void SendUsersToCallerServerEventHandler(IEnumerable<UserModel> users) 
        {
            Users = new ObservableCollection<UserModel>(users);
        }

        private void SendUserToCallerServerEventHandler(FullUserModel user) 
        {
            Id = user.Id;
            UserName = user.Name;
        }
    }
}
