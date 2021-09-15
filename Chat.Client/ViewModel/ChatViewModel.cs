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

        private ChatMemberViewModel _currentUser;

        private ICommand _connect;
        private ICommand _sendMessage;

        public ChatViewModel(ChatService chatService, RegistrationChatService registrationChatService)
        {
            _chatService = chatService;
            _registrationChatService = registrationChatService;

            User = new UserViewModel()
            {
                ButtonToSendVisibility = Visibility.Collapsed
            };
            Users = new ObservableCollection<ChatMemberViewModel>();

            SetEvents();
        }

        public UserViewModel User { get; set; }

        public ObservableCollection<ChatMemberViewModel> Users { get; set; }
        
        public ChatMemberViewModel CurrentUser 
        {
            get => _currentUser;
            set 
            {
                _currentUser = value;

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
                fromUserId: User.Id,
                toUserId: CurrentUser.Id,
                message: User.Message.TrimStart().TrimEnd(),
                connectionId: Users.First(user => user.Name == CurrentUser.Name).ConnectionId);

            User.Message = string.Empty;

            sendingMessage.IsFromCurrentUser = true;

            CurrentUser.Messages.Add(sendingMessage);
        }

        private bool CanExecuteSendMessage(object parametr) 
        {
            return !(string.IsNullOrWhiteSpace(User.Message) || string.IsNullOrEmpty(User.Message));
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
            ChatMemberViewModel connectedUser = Users.First(user =>
            {
                return user.Name == userName;
            });

            connectedUser.IsLogin = true;
            connectedUser.ConnectionId = connectionId;
        }

        private void LogoutEventHandler(string userName) 
        {
            ChatMemberViewModel logoutedUser = Users.First(user =>
            {
                return user.Name == userName;
            });

            logoutedUser.ConnectionId = string.Empty;
            logoutedUser.IsLogin = false;
        }

        private void ReciveMessageEventHandler(ChatMessageModel message) 
        {
            ChatMemberViewModel userSender = Users.First(user =>
            {
                return user.ChatId.Equals(message.ChatId);
            });

            userSender.Messages.Add(message);
        }

        private void SendConnectionsIdToCallerEventHandler(IEnumerable<UserConnection> connections) 
        {
            foreach (UserConnection connection in connections)
            {
                ChatMemberViewModel user = Users.First(model =>
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
                return chatModel.FirstUserId.Equals(User.Id) || chatModel.SecondUserId.Equals(User.Id);
            });

            ObservableCollection<ChatMessageModel> messages = new ObservableCollection<ChatMessageModel>(chat.Messages);

            Users.Add(new ChatMemberViewModel
            {
                Id = newUser.Id,
                ChatId = chat.Id,
                Messages = messages,
                Name = newUser.Name
            });
        }

        private void SendUsersToCallerServerEventHandler(IEnumerable<UserModel> users) 
        {
            foreach (UserModel user in users)
            {
                Users.Add(new ChatMemberViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    ConnectionId = user.ConnectionId,
                    ChatId = user.ChatId,
                    IsAdmin = user.IsAdmin,
                    IsLogin = user.IsLogin,
                    Messages = user.Messages
                });
            }
        }

        private void SendUserToCallerServerEventHandler(FullUserModel user) 
        {
            User.Id = user.Id;
            User.UserName = user.Name;
            User.IsAdmin = user.IsAdmin;
        }
    }
}
