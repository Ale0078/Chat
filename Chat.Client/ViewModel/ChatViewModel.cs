﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using AutoMapper;

using Chat.Client.Services;
using Chat.Client.Commands;
using Chat.Models;
using Chat.Client.Extensions;

namespace Chat.Client.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly IMapper _mapper;
        private readonly ChatService _chatService;
        private readonly RegistrationChatService _registrationChatService;
        private readonly List<ChatMemberViewModel> _blockedUsers;

        private ChatMemberViewModel _currentUser;
        private DispatcherTimer _timer;
        private ObservableCollection<ChatMemberViewModel> _users;

        private ICommand _connect;
        private ICommand _sendMessage;
        private ICommand _blockUser;
        private ICommand _muteUser;
        private ICommand _setBlackListState;
        private ICommand _setMessageToUser;
        private ICommand _typing;
        private ICommand _stopTyping;
        private ICommand _setVisibilityToUserInfoVisibility;

        public ChatViewModel(ChatService chatService, RegistrationChatService registrationChatService, UserViewModel user,
            IMapper mapper)
        {
            _mapper = mapper;
            _chatService = chatService;
            _registrationChatService = registrationChatService;
            User = user;

            UserInfoVisibility = new UserInfoVisibilityViewModel();

            Users = new ObservableCollection<ChatMemberViewModel>();
            _blockedUsers = new List<ChatMemberViewModel>();

            _timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 2)
            };

            SetEvents();
        }

        public UserViewModel User { get; }
        public UserInfoVisibilityViewModel UserInfoVisibility { get; }

        public ObservableCollection<ChatMemberViewModel> Users 
        {
            get => _users;
            set 
            {
                _users = value;

                OnPropertyChanged();
            } 
        }
        
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
            if (CurrentUser.EditMessage is null)
            {
                await ExecuteSendMessage();
            }
            else 
            {
                await ExecuteChangeMessage();
            }

            User.Message = string.Empty;
        }

        private async Task ExecuteSendMessage() 
        {
            ChatMessageModel sendingMessage = await _chatService.ReciveMessageUserAsync(
                chatId: CurrentUser.ChatId,
                fromUserId: User.Id,
                toUserId: CurrentUser.Id,
                message: User.Message.TrimStart().TrimEnd(),
                connectionId: Users.First(user => user.Name == CurrentUser.Name).ConnectionId);
            sendingMessage.IsFromCurrentUser = true;

            CurrentUser.Messages.AddViewModel(
                item: _mapper.Map<ChatMessageViewModel>(sendingMessage),
                handler: OnMessageChanged);

            CurrentUser.LastMessage = _mapper.Map<ChatMessageViewModel>(sendingMessage);

            Users = Users.GetSortedCollectionByLastMessage();
        }

        private async Task ExecuteChangeMessage() 
        {
            await _chatService.SetMessageToChatMessageAsync(
                connectionId: CurrentUser.ConnectionId,
                userId: User.Id,
                messageId: CurrentUser.EditMessage.Id,
                message: User.Message);

            CurrentUser.EditMessage.IsEdit = true;
            CurrentUser.EditMessage.Message = User.Message;
            CurrentUser.EditMessage.IsEditing = false;
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
        }

        public ICommand SetMessageToUser => _setMessageToUser ?? (_setMessageToUser = new RelayCommand(
            execute: ExecuteSetMessageToUser));

        private void ExecuteSetMessageToUser(object parametr) 
        {
            User.Message = CurrentUser.Draft.Message;
        }

        public ICommand Typing => _typing ?? (_typing = new RelayCommandAsync(
            execute: ExecuteTyping));

        private async Task ExecuteTyping(object parametr) 
        {
            await _chatService.SendUserTypingStatusToUserAsync(
                isTyping: true,
                connectionId: CurrentUser.ConnectionId,
                typingUserId: User.Id);

            if (string.IsNullOrEmpty(User.Message))
            {
                await _chatService.SendUserTypingStatusToUserAsync(
                    isTyping: false,
                    connectionId: CurrentUser.ConnectionId,
                    typingUserId: User.Id);
            }
            else
            {
                _timer.Stop();
            }
        }

        public ICommand StopTyping => _stopTyping ?? (_stopTyping = new RelayCommandAsync(
            execute: ExecuteStopTyping));

        private Task ExecuteStopTyping(object parametr) 
        {
            _timer.Start();

            return Task.CompletedTask;
        }

        public ICommand SetVisibilityToUserInfoVisibility => _setVisibilityToUserInfoVisibility ?? (_setVisibilityToUserInfoVisibility = new RelayCommand(
            execute: ExecuteSetVisibilityToUserInfoVisibility));

        private void ExecuteSetVisibilityToUserInfoVisibility(object visibility) 
        {
            UserInfoVisibility.BackgroundInfoVisibility = (Visibility)visibility;
        }

        private void SetEvents()
        {
            _chatService.ConnectUser += ConnectUserEventHandler;
            _chatService.Logout += LogoutEventHandler;
            _chatService.ReciveMessage += ReciveMessageEventHandler;
            _chatService.SendConnectionsIdToCallerEvent += SendConnectionsIdToCallerEventHandler;
            _chatService.SetBlockStateUserToAllUsersExeptBlocked += SetBlockStateUserToAllUsersExeptBlockedEventHandler;
            _chatService.SetMuteStateToUser += SetMuteStateToUser;
            _chatService.SetMuteStateUserToAllUsersExeptMuted += SetMuteStateUserToAllUsersExeptMutedEventHandler;
            _chatService.SendBlackListStateToUserServerHandler += SendBlackListStateToUserServerEventHandler;
            _chatService.SendTypingStatusToUserServerHandler += SendTypingStatusToUserServerEventHandler;
            _chatService.SetNewPhotoToUserServerHandler += SetNewPhotoToUserServerEventHandler;
            _chatService.ChangeMessageToUserServerHandler += ChangeMessageToUserServerEventHandler;

            _registrationChatService.RegisterUserToOthersServerHandler += RegisterUserToOthersServerEventHandler;
            _registrationChatService.SendUsersToCallerServerHandler += SendUsersToCallerServerEventHandler;
            _registrationChatService.SendUserToCallerServerHandler += SendUserToCallerServerEventHandler;
            _registrationChatService.SendBlockersToCallerServerHandler += SendBlockersToCallerServerEventHandler;

            _timer.Tick += OnTick;

            User.PropertyChanged += SetMessageToCurrentUserFromUser;
        }

        private void ConnectUserEventHandler(string userName, string connectionId) 
        {
            ChatMemberViewModel connectedUser = Users.FirstOrDefault(user =>
            {
                return user.Name == userName;
            });

            connectedUser.IsLogin = true;
            connectedUser.ConnectionId = connectionId;
        }

        private void LogoutEventHandler(string userName, DateTime disconnectTime) 
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
            logoutedUser.DisconnectTime = disconnectTime;
            logoutedUser.IsLogin = false;
            logoutedUser.IsTyping = false;
        }

        private void ReciveMessageEventHandler(ChatMessageModel message) 
        {
            ChatMemberViewModel userSender = Users.First(user =>
            {
                return user.ChatId.Equals(message.ChatId);
            });

            userSender.Messages.AddViewModel(
                item: _mapper.Map<ChatMessageViewModel>(message),
                handler: OnMessageChanged);

            userSender.LastMessage = _mapper.Map<ChatMessageViewModel>(message);

            Users = Users.GetSortedCollectionByLastMessage();
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

                Users = Users.GetSortedCollectionByLastMessage();
            }
        }

        private void SetMuteStateToUser(bool isMuted) 
        {
            User.IsMuted = isMuted;

            User.Message = string.Empty;

            foreach (ChatMemberViewModel user in Users)
            {
                user.Draft.Message = null;
            }
        }

        private void SetMuteStateUserToAllUsersExeptMutedEventHandler(string userId, bool isMuted) 
        {
            ChatMemberViewModel member = Users.First(user =>
            {
                return user.Id == userId;
            });

            if (member is null)
            {
                member = _blockedUsers.First(user =>
                {
                    return user.Id == userId;
                });
            }

            member.IsMuted = isMuted; 
        }

        private void SendBlackListStateToUserServerEventHandler(BlockModel block) 
        {
            ChatMemberViewModel member = Users.First(user =>
            {
                return user.Id == block.UserId;
            });

            member.IsClientBlockedByMember = block.DoesBlocked;
            member.Draft.Message = null;

            if (CurrentUser is not null && CurrentUser.Equals(member))
            {
                User.Message = string.Empty;
            }
        }

        private void SendTypingStatusToUserServerEventHandler(bool isTyping, string typingUserId) 
        {
            ChatMemberViewModel user = Users.First(userModel =>
            {
                return userModel.Id == typingUserId;
            });

            user.IsTyping = isTyping;
        }

        private void SetNewPhotoToUserServerEventHandler(string userName, byte[] photo) 
        {
            ChatMemberViewModel user = Users.First(userModel =>
            {
                return userModel.Name == userName;
            });

            user.Photo = photo;
        }

        private void ChangeMessageToUserServerEventHandler(string userId, Guid messageId, string message) 
        {
            ChatMemberViewModel user = Users.First(userModel =>
            {
                return userModel.Id == userId;
            });

            ChatMessageViewModel chatMessage = user.Messages.First(messageModel =>
            {
                return messageModel.Id == messageId;
            });

            chatMessage.Message = message;
            chatMessage.IsEdit = true;
        }

        private void RegisterUserToOthersServerEventHandler(FullUserModel newUser)
        {
            ChatModel chat = newUser.Chats.Find(chatModel =>
            {
                return chatModel.FirstUserId.Equals(User.Id) || chatModel.SecondUserId.Equals(User.Id);
            });

            ChatMemberViewModel user = _mapper.Map<ChatMemberViewModel>(newUser);

            user.ChatId = chat.Id;
            user.Messages = _mapper.Map<ObservableCollection<ChatMessageViewModel>>(chat.Messages);
            user.LastMessage = chat.Messages.Count == 0
                    ? new ChatMessageViewModel()
                    : _mapper.Map<ChatMessageViewModel>(chat.Messages.Last());

            user.Messages.SetPropertyChangedEventHandler(OnMessageChanged);
        }

        private void SendUsersToCallerServerEventHandler(IEnumerable<UserModel> users) 
        {
            foreach (UserModel user in users)
            {
                ChatMemberViewModel member = _mapper.Map<ChatMemberViewModel>(user);

                member.Messages.SetPropertyChangedEventHandler(OnMessageChanged);

                if (user.IsBlocked && !User.IsAdmin)
                {
                    _blockedUsers.Add(member);
                }
                else
                {
                    Users.Add(member);
                }
            }

            Users = Users.GetSortedCollectionByLastMessage();
        }

        private void SendUserToCallerServerEventHandler(FullUserModel user) 
        {
            User.Id = user.Id;
            User.Name = user.Name;
            User.IsAdmin = user.IsAdmin;
            User.IsMuted = user.IsMuted;
            User.Photo = user.Photo;
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

        private async void OnTick(object sender, EventArgs e) 
        {
            await _chatService.SendUserTypingStatusToUserAsync(
                isTyping: false,
                connectionId: CurrentUser.ConnectionId,
                typingUserId: User.Id);

            _timer.Stop();
        }

        private void SetMessageToCurrentUserFromUser(object sender, PropertyChangedEventArgs e) 
        {
            if (CurrentUser is null || e.PropertyName == nameof(UserViewModel.Message))
            {
                return;
            }

            UserViewModel user = sender as UserViewModel;

            if (string.IsNullOrEmpty(user.Message))
            {
                CurrentUser.Draft.Message = null;
            }
            else
            {
                if (CurrentUser.Draft.Message is null)
                {
                    CurrentUser.Draft.StartTypingTime = DateTime.Now;
                }

                CurrentUser.Draft.Message = user.Message;
            }
        }

        private void OnMessageChanged(object sender, PropertyChangedEventArgs e) 
        {
            if (CurrentUser is null || e.PropertyName != nameof(ChatMessageViewModel.IsEditing))
            {
                return;
            }

            ChatMessageViewModel message = sender as ChatMessageViewModel;

            if (message.IsEditing)
            {
                CurrentUser.EditMessage = message;
                User.Message = message.Message;
            }
            else 
            {
                CurrentUser.EditMessage = null;
            }
        }
    }
}
