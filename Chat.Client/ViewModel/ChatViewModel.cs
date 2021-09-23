﻿using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

using Chat.Client.Services;
using Chat.Client.Commands;
using Chat.Models;

namespace Chat.Client.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly ChatService _chatService;
        private readonly RegistrationChatService _registrationChatService;
        private readonly List<ChatMemberViewModel> _blockedUsers;

        private ChatMemberViewModel _currentUser;

        private ICommand _connect;
        private ICommand _sendMessage;
        private ICommand _blockUser;
        private ICommand _muteUser;
        private ICommand _setBlackListState;
        private ICommand _setMessageToUser;

        public ChatViewModel(ChatService chatService, RegistrationChatService registrationChatService)
        {
            _chatService = chatService;
            _registrationChatService = registrationChatService;

            User = new UserViewModel()
            {
                ButtonToSendVisibility = Visibility.Collapsed
            };

            Users = new ObservableCollection<ChatMemberViewModel>();
            _blockedUsers = new List<ChatMemberViewModel>();

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
            ChatMessageModel sendingMessage = await _chatService.ReciveMessageUserAsync(
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

        public ICommand BlockUser => _blockUser ?? (_blockUser = new RelayCommandAsync(
            execute: ExecuteBlockUser));

        public async Task ExecuteBlockUser(object isBlocked) 
        {
            await _chatService.SetBlockStateToUserAsync(
                userId: CurrentUser.Id,
                connectionId: CurrentUser.ConnectionId,
                isBlocked: (bool)isBlocked);
        }

        public ICommand MuteUser => _muteUser ?? (_muteUser = new RelayCommandAsync(
            execute: ExecuteMuteUser));

        private async Task ExecuteMuteUser(object isMuted) 
        {
            await _chatService.SetMuteStateToUserAsync(
                userId: CurrentUser.Id,
                connectionId: CurrentUser.ConnectionId,
                isMuted: (bool)isMuted);
        }

        public ICommand SetBlackListState => _setBlackListState ?? (_setBlackListState = new RelayCommandAsync(
            execute: ExecuteSetBlackListState));

        private async Task ExecuteSetBlackListState(object doesBlock) 
        {
            await _chatService.SendBlackListStateAsync(
                userId: User.Id,
                connectionId: CurrentUser.ConnectionId,
                blockedUserId: CurrentUser.Id,
                doesBlock: (bool)doesBlock);

            if ((bool)doesBlock)
            {
                User.Message = string.Empty;
            }
        }

        public ICommand SetMessageToUser => _setMessageToUser ?? (_setMessageToUser = new RelayCommand(
            execute: ExecuteSetMessageToUser));

        private void ExecuteSetMessageToUser(object parametr) 
        {
            User.Message = CurrentUser.Message;
        }

        private void SetEvents()
        {
            _chatService.ConnectUser += ConnectUserEventHandler;
            _chatService.Logout += LogoutEventHandler;
            _chatService.ReciveMessage += ReciveMessageEventHandler;
            _chatService.SendConnectionsIdToCallerEvent += SendConnectionsIdToCallerEventHandler;
            _chatService.SetBlockStateUserToAllUsersExeptBlocked += SetBlockStateUserToAllUsersExeptBlockedEventHandler;
            _chatService.SetMuteStateToUser += SetMuteStateToUser;
            _chatService.SendBlackListStateToUserServerHandler += SendBlackListStateToUserServerEventHandler;

            _registrationChatService.RegisterUserToOthersServerHandler += RegisterUserToOthersServerEventHandler;
            _registrationChatService.SendUsersToCallerServerHandler += SendUsersToCallerServerEventHandler;
            _registrationChatService.SendUserToCallerServerHandler += SendUserToCallerServerEventHandler;
            _registrationChatService.SendBlockersToCallerServerHandler += SendBlockersToCallerServerEventHandler;

            User.PropertyChanged += SetMessageToCurrentUserFromUser;
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
            ChatMemberViewModel logoutedUser = Users.FirstOrDefault(user =>
            {
                return user.Name == userName;
            });

            if (logoutedUser is null)
            {
                logoutedUser = _blockedUsers.Find(user =>
                {
                    return user.Name == userName;
                });
            }

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
                ChatMemberViewModel user = Users.FirstOrDefault(model =>
                {
                    return model.Name == connection.UserName;
                });

                if (user is null)
                {
                    user = _blockedUsers.Find(user =>
                    {
                        return user.Name == connection.UserName;
                    });
                }

                user.ConnectionId = connection.ConnectionId;
                user.IsLogin = true;
            }
        }

        private void SetBlockStateUserToAllUsersExeptBlockedEventHandler(string userId, bool isBlocked) 
        {
            ChatMemberViewModel chatMember = Users.FirstOrDefault(user =>
            {
                return user.Id == userId;
            });

            if (chatMember is null)
            {
                chatMember = _blockedUsers.Find(user =>
                {
                    return user.Id == userId;
                });
            }

            chatMember.IsBlocked = isBlocked;

            if (User.IsAdmin)
            {
                return;
            }

            if (isBlocked)
            {
                _blockedUsers.Add(chatMember);
                Users.Remove(chatMember);
            }
            else 
            {
                Users.Add(chatMember);
                _blockedUsers.Remove(chatMember);
            }
        }

        private void SetMuteStateToUser(bool isMuted) 
        {
            User.IsMuted = isMuted;
        }

        private void SendBlackListStateToUserServerEventHandler(BlockModel block) 
        {
            ChatMemberViewModel member = Users.First(user =>
            {
                return user.Id == block.UserId;
            });

            member.IsClientBlockedByMember = block.DoesBlocked;
        }

        private void RegisterUserToOthersServerEventHandler(FullUserModel newUser)
        {
            ChatModel chat = newUser.Chats.Find(chatModel =>
            {
                return chatModel.FirstUserId.Equals(User.Id) || chatModel.SecondUserId.Equals(User.Id);
            });

            ObservableCollection<ChatMessageModel> messages = new ObservableCollection<ChatMessageModel>(chat.Messages);

            ChatMemberViewModel user = new()
            {
                Id = newUser.Id,
                ChatId = chat.Id,
                Messages = messages,
                Name = newUser.Name,
                Photo = newUser.Photo,
                IsBlocked = newUser.IsBlocked
            };

            if (newUser.IsBlocked && !User.IsAdmin)
            {
                _blockedUsers.Add(user);
            }
            else
            {
                Users.Add(user);   
            }
        }

        private void SendUsersToCallerServerEventHandler(IEnumerable<UserModel> users) 
        {
            foreach (UserModel user in users)
            {
                ChatMemberViewModel member = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Photo = user.Photo,
                    ConnectionId = user.ConnectionId,
                    ChatId = user.ChatId,
                    IsAdmin = user.IsAdmin,
                    IsLogin = user.IsLogin,
                    Messages = user.Messages,
                    IsBlocked = user.IsBlocked
                };

                if (user.IsBlocked && !User.IsAdmin)
                {
                    _blockedUsers.Add(member);
                }
                else
                {
                    Users.Add(member);
                }
            }
        }

        private void SendUserToCallerServerEventHandler(FullUserModel user) 
        {
            User.Id = user.Id;
            User.UserName = user.Name;
            User.IsAdmin = user.IsAdmin;
            User.IsMuted = user.IsMuted;
        }

        private void SendBlockersToCallerServerEventHandler(IEnumerable<BlockModel> blocks) 
        {
            foreach (BlockModel block in blocks) 
            {
                ChatMemberViewModel member = Users.FirstOrDefault(user =>
                {
                    return user.Id == block.UserId;
                });

                member.IsClientBlockedByMember = block.DoesBlocked;
            }
        }

        private void SetMessageToCurrentUserFromUser(object sender, PropertyChangedEventArgs e) 
        {
            if (CurrentUser is null)
            {
                return;
            }

            UserViewModel user = sender as UserViewModel;

            CurrentUser.Message = user.Message;
        }
    }
}
